using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocationItemUI : MonoBehaviour
{
    public TMP_Text locationNameText;
    public TMP_Text tempText;
    public TMP_Text windText;
    public TMP_Text rainingText;
    public TMP_Text seasonText;

    public Button deleteButton;
    public Button mainButton; // assign this to the entire prefab panel or overlay button

    public float Latitude;
    public float Longitude;
    private string locationName;

    private WeatherManager manager;

    public void Init(WeatherManager wm)
    {
        manager = wm;
        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteThisItem);
        if (mainButton != null)
            mainButton.onClick.AddListener(OpenDetailScene);
    }

    public void UpdateInfo(string locationName, float temp, float wind, string raining, string season)
    {
        this.locationName = locationName;
        locationNameText.text = "Location: " + locationName;
        tempText.text = "Temp: " + temp + "°C";
        windText.text = "Wind: " + wind + " km/h";
        rainingText.text = "Raining: " + raining;
        seasonText.text = "Season: " + season;
    }

    void DeleteThisItem()
    {
        if (manager != null)
            manager.RemoveLocation(this.gameObject);
    }

    void OpenDetailScene()
    {
        // Save location data for the detail scene
        LocationDetailData.Latitude = Latitude;
        LocationDetailData.Longitude = Longitude;
        LocationDetailData.LocationName = locationName;

        // Load the detail scene
        SceneManager.LoadScene("Game");
    }
}
