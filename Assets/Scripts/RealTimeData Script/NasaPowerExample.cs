using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON; // or use Unity's JsonUtility if you prefer

public class NasaPowerExample : MonoBehaviour
{
    // Dhaka coordinates
    private float latitude = 23.8103f;
    private float longitude = 90.4125f;

    void Start()
    {
        StartCoroutine(GetRealtimeTemperature());
    }

    IEnumerator GetRealtimeTemperature()
    {
        // Open-Meteo API endpoint for current weather
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
                string jsonText = www.downloadHandler.text;
                JSONNode root = JSON.Parse(jsonText);

                // Access temperature in °C
                float temp = root["current_weather"]["temperature"].AsFloat;
                float windSpeed = root["current_weather"]["windspeed"].AsFloat;
                string time = root["current_weather"]["time"];

                Debug.Log($"Time: {time}, Temperature: {temp}°C, Wind Speed: {windSpeed} km/h");
            }
        }
    }
}
