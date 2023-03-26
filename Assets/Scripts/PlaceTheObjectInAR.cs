using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceTheObjectInAR : MonoBehaviour
{
    ARRaycastManager aRRaycastManager;
    [SerializeField] GameObject smartDevice;
    [SerializeField] TextMeshProUGUI textBox;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        textBox.text = "Hover around your device to detect planes";
    }

    void Update()
    {
        if (aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes) &&
            ShowDataOnDevice.instance == null)
            textBox.text = "Tap here to Place";

        Vector2 pos = Input.GetTouch(0).position;
        if (Input.touchCount <= 0 || 
            EventSystem.current.currentSelectedGameObject != null ||
            pos.x < 0.1f*Screen.width ||
            pos.x > 0.9f*Screen.width ||
            pos.y < 0.1f*Screen.height ||
            pos.y > 0.9f*Screen.height)
            return;

        if (aRRaycastManager.Raycast(new Vector2(Screen.width/2, Screen.height/2), hits, TrackableType.Planes))
            Instantiate(smartDevice, hits[0].pose.position, hits[0].pose.rotation);
        textBox.text = "";
    }
}
