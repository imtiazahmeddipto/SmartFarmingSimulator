using UnityEngine;
using UnityEngine.UI;

public class PlantGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public int growthStages = 3;
    public Sprite[] growthSprites;
    public float currentGrowth = 0f;
    public float maxGrowth = 3f;

    public float baseGrowthRate = 0.1f;
    public float boostedGrowthRate = 0.3f;

    [Header("Needs System")]
    public float waterLevel = 0f;
    public float fertilizerLevel = 0f;

    public float waterGoodMin = 30f, waterGoodMax = 70f;
    public float fertGoodMin = 30f, fertGoodMax = 70f;

    public float waterMax = 100f;
    public float fertMax = 100f;

    [Header("UI Bars")]
    public Image waterBar;
    public Image fertilizerBar;

    [Header("Collect Settings")]
    public float collectRange = 2f;
    public GameObject collectIndicator;
    public GameObject CollectEffect;

    [Header("Environment")]
    public LocationDetailUI locationDataUI;

    private SpriteRenderer sr;
    private int currentStage = 0;
    private bool isDead = false;
    private bool canCollect = false;

    private Transform player;
    private Vector3Int plantedCell;
    private Inventory inventory;

    public void SetPlantedCell(Vector3Int cellPos)
    {
        plantedCell = cellPos;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        inventory = FindObjectOfType<Inventory>();
        sr = GetComponent<SpriteRenderer>();
        locationDataUI = FindObjectOfType<LocationDetailUI>();
        if (growthSprites.Length > 0)
            sr.sprite = growthSprites[0];

        if (collectIndicator != null)
            collectIndicator.SetActive(false);

        // Initial environment adjustment
        if (locationDataUI != null)
        {
            AdjustNeeds(locationDataUI.CurrentTemperature, locationDataUI.IsRaining, locationDataUI.CurrentSeason);
        }
    }

    void Update()
    {
        if (isDead) return;

        // Continuously update needs based on environment
        if (locationDataUI != null)
        {
            AdjustNeeds(locationDataUI.CurrentTemperature, locationDataUI.IsRaining, locationDataUI.CurrentSeason);
        }

        UpdateBars();

        // Grow plant
        if (currentGrowth < maxGrowth)
        {
            if (IsInGoodRange())
                currentGrowth += boostedGrowthRate * Time.deltaTime;
            else
                currentGrowth += baseGrowthRate * Time.deltaTime;

            UpdateGrowthStage();
        }

        if (currentGrowth >= maxGrowth && !isDead)
        {
            canCollect = true;
            HandleCollect();
        }

        if (waterLevel > waterMax || fertilizerLevel > fertMax)
        {
            KillPlant();
        }
    }

    void UpdateBars()
    {
        if (waterBar != null)
        {
            float targetFill = Mathf.Clamp01(waterLevel / waterMax);
            waterBar.fillAmount = Mathf.Lerp(waterBar.fillAmount, targetFill, Time.deltaTime * 5f);

            Color targetColor = (waterLevel >= waterGoodMin && waterLevel <= waterGoodMax) ? Color.green : Color.red;
            waterBar.color = Color.Lerp(waterBar.color, targetColor, Time.deltaTime * 5f);
        }

        if (fertilizerBar != null)
        {
            float targetFill = Mathf.Clamp01(fertilizerLevel / fertMax);
            fertilizerBar.fillAmount = Mathf.Lerp(fertilizerBar.fillAmount, targetFill, Time.deltaTime * 5f);

            Color targetColor = (fertilizerLevel >= fertGoodMin && fertilizerLevel <= fertGoodMax) ? Color.green : Color.red;
            fertilizerBar.color = Color.Lerp(fertilizerBar.color, targetColor, Time.deltaTime * 5f);
        }
    }

    bool IsInGoodRange()
    {
        return (waterLevel >= waterGoodMin && waterLevel <= waterGoodMax) &&
               (fertilizerLevel >= fertGoodMin && fertilizerLevel <= fertGoodMax);
    }

    public void AddWater(float amount)
    {
        waterLevel += amount;
    }

    public void AddFertilizer(float amount)
    {
        fertilizerLevel += amount;
    }

    void KillPlant()
    {
        if (isDead) return; // prevent double calls
        isDead = true;

        sr.color = Color.black;
        Debug.Log("Plant died from overdose!");

        if (collectIndicator != null)
            collectIndicator.SetActive(false);

        // Start delayed destruction
        StartCoroutine(DestroyAfterDelay(2f));
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Remove from plantedTiles so the cell is usable again
        if (PlantingScript.plantedTiles.ContainsKey(plantedCell))
            PlantingScript.plantedTiles.Remove(plantedCell);

        Destroy(gameObject);
    }


    void UpdateGrowthStage()
    {
        int newStage = Mathf.FloorToInt((currentGrowth / maxGrowth) * growthStages);
        newStage = Mathf.Clamp(newStage, 0, growthStages - 1);

        if (newStage != currentStage)
        {
            currentStage = newStage;
            if (growthSprites.Length > currentStage)
                sr.sprite = growthSprites[currentStage];
        }
    }

    void HandleCollect()
    {
        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (collectIndicator != null)
            collectIndicator.SetActive(distance <= collectRange);

        if (distance <= collectRange && Input.GetKeyDown(KeyCode.E))
        {
            if (PlantingScript.plantedTiles.ContainsKey(plantedCell))
                PlantingScript.plantedTiles.Remove(plantedCell);

            Destroy(gameObject);
            Debug.Log("Plant collected!");

            if (CollectEffect != null)
                Destroy(Instantiate(CollectEffect, transform.position, Quaternion.identity), 2f);

            // --- Add plant to inventory ---
            if (inventory != null)
            {
                string harvestedItem = "crops"; // <-- change to actual crop name like "Rice" or "Corn"
                Sprite harvestedSprite = sr.sprite; // use final stage sprite as icon
                bool added = inventory.AddItem(harvestedItem, 1, harvestedSprite);

                if (added)
                    Debug.Log($"{harvestedItem} +1 added to inventory");
                else
                    Debug.Log("Inventory full! Could not add item.");
            }
        }
    }


    void AdjustNeeds(float temperature, bool raining, string season)
    {
        // Set dynamic min/max based on season, rain, temp
        if (raining)
        {
            waterGoodMin = 20f;
            waterGoodMax = 50f;
        }
        else
        {
            if (season == "Summer")
            {
                waterGoodMin = 50f;
                waterGoodMax = 80f;
            }
            else if (season == "Winter")
            {
                waterGoodMin = 30f;
                waterGoodMax = 60f;
            }
            else // Spring/Autumn
            {
                waterGoodMin = 35f;
                waterGoodMax = 65f;
            }
        }

        // Fertilizer needs lower in summer, higher in spring/autumn
        if (season == "Summer")
        {
            fertGoodMin = 20f;
            fertGoodMax = 50f;
        }
        else
        {
            fertGoodMin = 30f;
            fertGoodMax = 70f;
        }

        // --- Debug logs ---
        //Debug.Log($"[AdjustNeeds] Season: {season}, Raining: {raining}, Temp: {temperature}");
        //Debug.Log($"[AdjustNeeds] Water range: {waterGoodMin} - {waterGoodMax}");
        //Debug.Log($"[AdjustNeeds] Fertilizer range: {fertGoodMin} - {fertGoodMax}");
    }

}
