using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ShowDataOnDevice : MonoBehaviour
{
    [SerializeField] Transform textFields;
    [SerializeField] TMP_Dropdown dropdown;
    string IP;
    LocationInfo info;
    Weather weather;
    public static ShowDataOnDevice instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(GetIP());
        yield return StartCoroutine(GetCoordinates());
        yield return StartCoroutine(GetWeather());

        StartCoroutine(SetTime());
        textFields.GetChild(5).GetComponent<TextMeshProUGUI>().text = weather.weatherInfo.temp + " K";
        textFields.GetChild(6).GetComponent<TextMeshProUGUI>().text = info.geoplugin_city;
    }

    IEnumerator GetIP()
    {
        UnityWebRequest www = new UnityWebRequest("https://api.ipify.org/")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.ConnectionError)
            IP = www.downloadHandler.text;
    }
    IEnumerator GetCoordinates()
    {
        UnityWebRequest www = new UnityWebRequest("http://www.geoplugin.net/json.gp?ip="+IP)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ConnectionError)
            info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
    }
    IEnumerator GetWeather()
    {
        string lat = info.geoplugin_latitude;
        string lon = info.geoplugin_longitude;
        string API_Key = "5860eb947c1840aa92189ce932d29f5c";
        UnityWebRequest www = new UnityWebRequest(
            $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&APPID={API_Key}")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return www.SendWebRequest();
        weather = JsonConvert.DeserializeObject<Weather>(www.downloadHandler.text);
    }

    IEnumerator SetTime()
    {
        string date = System.DateTime.Now.Date.ToString("dd");
        string month = System.DateTime.Now.Date.ToString("MMM");
        string year = System.DateTime.Now.Date.ToString("yyyy");
        textFields.GetChild(0).GetComponent<TextMeshProUGUI>().text = date;
        textFields.GetChild(1).GetComponent<TextMeshProUGUI>().text = month + "\n" + year;

        while (true)
        {
            string hour = System.DateTime.Now.ToLocalTime().ToString("HH");
            string amOrPm = "AM";
            if (int.Parse(hour) > 13)
                amOrPm = "PM";
            string time = System.DateTime.Now.ToLocalTime().ToString("hh:mm");
            string sec = System.DateTime.Now.ToLocalTime().ToString("ss");
            textFields.GetChild(2).GetComponent<TextMeshProUGUI>().text = time;
            textFields.GetChild(3).GetComponent<TextMeshProUGUI>().text = amOrPm;
            textFields.GetChild(4).GetComponent<TextMeshProUGUI>().text = sec;
            yield return new WaitForSeconds(1);
        }
    }

    class LocationInfo
    {
        public string geoplugin_city;
        public string geoplugin_latitude;
        public string geoplugin_longitude;
    }
    class Weather
    {
        [JsonProperty("main")] public WeatherInfo weatherInfo { get; set; }
    }
    class WeatherInfo
    {
        public string temp;
    }

    public void ChangeTempScale()
    {
        float kelvin = float.Parse(weather.weatherInfo.temp);
        switch (dropdown.value)
        {
            case 0:
                textFields.GetChild(5).GetComponent<TextMeshProUGUI>().text =  kelvin.ToString() + " K";
                break;
            case 1:
                textFields.GetChild(5).GetComponent<TextMeshProUGUI>().text = (kelvin - 273.15).ToString() + " <sup>O</sup>C";
                break;
            case 2:
                float fahrenheit = ((kelvin - 273.15f) * 1.8f) + 32;
                textFields.GetChild(5).GetComponent<TextMeshProUGUI>().text = fahrenheit.ToString() + " <sup>O</sup>F";
                break;
            default:
                break;
        }
    }
}
