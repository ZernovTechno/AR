using Mediapipe.Unity;
using Mediapipe.Unity.Sample.HandTracking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    public AudioSource ClickSound;
    public GameObject AnnotationLayer;
    public WindowPositionController WPC;

    private bool Cooldown;

    void Start()
    {
        InvokeRepeating("Update_Every_1000ms", 2, 1f);
    }
    void Update_Every_1000ms ()
    {
        if (Cooldown) Cooldown = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.CompareTag("Interface") && AnnotationLayer.activeSelf)
        {
            Debug.Log($"Trigger with {other.name}.");
        }
        if (other.CompareTag("ThumbFinger"))
        {
            WPC.TriggerWithThumb = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (AnnotationLayer.activeSelf && !Cooldown)
        {
            if (other.CompareTag("Interface")) // If there interface on a way
            {
                //if (!other.gameObject.GetComponentInParent<Canvas>().CompareTag("SafePane")) _MainTrackingModule.LastContactedMenu = other.gameObject.GetComponentInParent<Canvas>();
                ExecuteEvents.Execute(other.gameObject.GetComponent<Button>().gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler); // Do clicking on a button (If its a button) by emulating mouse event
                ClickSound.Play();
                Cooldown = true;
            }
            else if (other.CompareTag("ScreenCast")) // If there is a raycast...
            {
                //ClientScript.Finger = ScreenCaster.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[8]);
            }
            if (other.CompareTag("ThumbFinger"))
            {
                WPC.TriggerWithThumb = false;
            }
        }
    }
}
