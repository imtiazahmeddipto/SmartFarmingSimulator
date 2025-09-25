using UnityEngine;

public class Fertilizer : MonoBehaviour
{
    public ParticleSystem fertilizerParticles;
    public float applyInterval = 1f; // every 1 second
    private float applyTimer = 0f;

    private PlayerMovement movement;
    private Animator animator;
    private Hotbar hotbar;

    void Start()
    {
        movement = FindObjectOfType<PlayerMovement>();
        if (movement != null)
            animator = movement.GetComponent<Animator>();

        hotbar = FindObjectOfType<Hotbar>();
    }

    void Update()
    {
        if (movement == null || animator == null || hotbar == null) return;

        // Always fetch current slot
        InventorySlot currentSlot = GetCurrentFertilizerSlot();
        int currentCount = currentSlot != null ? currentSlot.count : 0;

        // Continuously update count display
        if (currentSlot != null && currentSlot.countText != null)
            currentSlot.countText.text = currentCount > 1 ? currentCount.ToString() : "";

        if (currentSlot == null || currentSlot.IsEmpty || movement.IsRunning)
        {
            ResetFertilizing();
            return;
        }

        bool fertilizingNow = Input.GetMouseButton(1);

        if (fertilizingNow)
        {
            animator.SetBool("Fertilizing", true);
            if (fertilizerParticles != null && !fertilizerParticles.isPlaying)
                fertilizerParticles.Play();

            applyTimer += Time.deltaTime;
            if (applyTimer >= applyInterval)
            {
                currentSlot = GetCurrentFertilizerSlot(); // fetch latest
                ApplyFertilizer(currentSlot);
                applyTimer = 0f;
            }
        }
        else
        {
            ResetFertilizing();
        }
    }


    void ApplyFertilizer(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty) return;

        slot.count = Mathf.Max(slot.count - 1, 0);

        if (slot.count == 0)
        {
            slot.Clear();
            hotbar.SelectSlot(0);
            animator.SetBool("Fertilizing", false);
        }
        else
        {
            if (slot.countText != null)
                slot.countText.text = slot.count > 1 ? slot.count.ToString() : "";
        }

        Debug.Log("Used 1 Fertilizer, remaining: " + slot.count);
    }

    void ResetFertilizing()
    {
        animator.SetBool("Fertilizing", false);
        applyTimer = 0f;

        if (fertilizerParticles != null && fertilizerParticles.isPlaying)
            fertilizerParticles.Stop();
    }

    private InventorySlot GetCurrentFertilizerSlot()
    {
        if (hotbar == null) return null;

        InventorySlot selected = hotbar.GetSelectedSlot();
        if (selected != null && !selected.IsEmpty && selected.itemName.ToLower().Contains("fartilizier"))
            return selected;

        return null;
    }
}
