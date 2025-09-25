using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using System.Collections;

public class LocationDetailUI : MonoBehaviour
{
    public TMP_Text locationNameText;
    public TMP_Text tempText;
    public TMP_Text windText;
    public TMP_Text rainingText;
    public TMP_Text seasonText;

    public GameObject SnowFall;
    public GameObject RainFall;
    public float CurrentTemperature { get; private set; }
    public bool IsRaining { get; private set; }
    public string CurrentSeason { get; private set; }
    void Start()
    {
        locationNameText.text = "Location: " + LocationDetailData.LocationName;
        StartCoroutine(FetchWeather(LocationDetailData.Latitude, LocationDetailData.Longitude));
    }

    IEnumerator FetchWeather(float latitude, float longitude)
    {
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
                var root = JSON.Parse(www.downloadHandler.text);
                var current = root["current_weather"];
                float temp = current["temperature"].AsFloat;
                float wind = current["windspeed"].AsFloat;
                int weatherCode = current["weathercode"].AsInt;

                bool raining = (weatherCode >= 50 && weatherCode <= 67) || (weatherCode >= 80 && weatherCode <= 82);

                string season = GetSeason(latitude, temp);

                tempText.text = "Temp: " + temp + "°C";
                windText.text = "Wind: " + wind + " km/h";
                rainingText.text = "Raining: " + (raining ? "Yes" : "No");
                seasonText.text = "Season: " + season;

                // Visuals
                HandleWeatherVisuals(weatherCode, temp);
            }
        }
    }

    void HandleWeatherVisuals(int weatherCode, float temp)
    {
        SnowFall.SetActive(false);
        RainFall.SetActive(false);

        // Snow codes or below-freezing winter
        if (weatherCode == 71 || weatherCode == 73 || weatherCode == 75 ||
            weatherCode == 77 || weatherCode == 85 || weatherCode == 86 || temp <= 0f)
        {
            SnowFall.SetActive(true);
        }
        // Rain codes
        else if ((weatherCode >= 51 && weatherCode <= 67) || (weatherCode >= 80 && weatherCode <= 82))
        {
            RainFall.SetActive(true);
        }
    }

    string GetSeason(float latitude, float temperature)
    {
        int month = System.DateTime.Now.Month;
        bool southern = latitude < 0;

        string season;
        if (southern)
        {
            // Southern Hemisphere
            if (month >= 3 && month <= 5) season = "Autumn";
            else if (month >= 6 && month <= 8) season = "Winter";
            else if (month >= 9 && month <= 11) season = "Spring";
            else season = "Summer";
        }
        else
        {
            // Northern Hemisphere
            if (month >= 3 && month <= 5) season = "Spring";
            else if (month >= 6 && month <= 8) season = "Summer";
            else if (month >= 9 && month <= 11) season = "Autumn";
            else season = "Winter";
        }

        // Add snowfall if temperature <= 0°C in winter
        if (season == "Winter" && temperature <= 0f)
            return "Snowfall";

        return season;
    }
}
