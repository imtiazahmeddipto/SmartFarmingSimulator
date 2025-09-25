using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

public class WeatherManager : MonoBehaviour
{
    public TMP_InputField latitudeInput;
    public TMP_InputField longitudeInput;
    public TMP_InputField locationNameInput;

    public Transform contentParent; // where prefabs appear
    public GameObject locationItemPrefab;

    public GameObject inputPanel;

    private List<GameObject> savedLocations = new List<GameObject>();

    // Single button: Add + Save
    public void OnAddAndSaveLocationButton()
    {
        if (savedLocations.Count >= 4)
        {
            Debug.LogWarning("Maximum of 4 locations allowed.");
            return;
        }

        float lat, lon;
        if (float.TryParse(latitudeInput.text, out lat) && float.TryParse(longitudeInput.text, out lon))
        {
            string locName = string.IsNullOrEmpty(locationNameInput.text) ? "Unknown" : locationNameInput.text;
            StartCoroutine(AddAndSaveLocation(locName, lat, lon));
        }
        else
        {
            Debug.LogError("Invalid coordinates");
        }
    }

    IEnumerator AddAndSaveLocation(string locationName, float latitude, float longitude)
    {
        // Prevent duplicates
        foreach (var item in savedLocations)
        {
            var itemUI = item.GetComponent<LocationItemUI>();
            if (itemUI != null && Mathf.Approximately(itemUI.Latitude, latitude) && Mathf.Approximately(itemUI.Longitude, longitude))
            {
                Debug.LogWarning("This location is already added!");
                yield break;
            }
        }

        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                var jsonText = www.downloadHandler.text;
                var root = JSON.Parse(jsonText);
                var current = root["current_weather"];
                float temp = current["temperature"].AsFloat;
                float wind = current["windspeed"].AsFloat;
                string raining = current["weathercode"].AsInt == 0 ? "No" : "Yes";
                string season = GetSeason(latitude);

                // Instantiate prefab
                GameObject newItem = Instantiate(locationItemPrefab, contentParent);
                var itemUI = newItem.GetComponent<LocationItemUI>();
                itemUI.Latitude = latitude;
                itemUI.Longitude = longitude;
                itemUI.UpdateInfo(locationName, temp, wind, raining, season);
                itemUI.Init(this);

                // Save immediately
                savedLocations.Add(newItem);

                Debug.Log($"Location '{locationName}' added and saved.");

                // Disable input panel
                if (inputPanel != null)
                    inputPanel.SetActive(false);
            }
        }
    }

    public void RemoveLocation(GameObject locationItem)
    {
        if (savedLocations.Contains(locationItem))
            savedLocations.Remove(locationItem);

        Destroy(locationItem);
        Debug.Log("Location removed.");

        // Optional: re-enable input panel if less than 4 locations
        if (inputPanel != null && savedLocations.Count < 4)
            inputPanel.SetActive(true);
    }

    string GetSeason(float latitude)
    {
        int month = System.DateTime.Now.Month;
        if (month >= 3 && month <= 5) return "Spring";
        if (month >= 6 && month <= 8) return "Summer";
        if (month >= 9 && month <= 11) return "Autumn";
        return "Winter";
    }
}
