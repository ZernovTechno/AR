using Mediapipe.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Experimental.GlobalIllumination;
using Mediapipe.Unity.Sample.HandTracking;
using Mediapipe.Unity.Sample;
using Mediapipe;

public class WindowPositionController : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image positionChangerPointer;
    [SerializeField] HandInstrument HI;
    [SerializeField] GameObject PlayerObject;
    public bool TriggerWithThumb;
    public bool Timer;
    public Canvas LastContactedMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Timer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerWithThumb && Timer && HI.TrackingIsWorking)
        {
            AddValue();
        }
        else RemoveValue();
    }

    public void ResetTimer()
    {
        Timer = true;
    }

    public void AddValue()
    {
        if (positionChangerPointer.fillAmount < 1) 
            positionChangerPointer.fillAmount += 0.01f;
        if (positionChangerPointer.fillAmount == 1)
        {
            positionChangerPointer.fillAmount = 0;
            LastContactedMenu.transform.position = positionChangerPointer.transform.position;
            LastContactedMenu.transform.rotation = PlayerObject.transform.rotation;
            Timer = false;
            InvokeRepeating("ResetTimer", 3, 0);
            Debug.Log("Recentering last actual menu...");
        }

    }

    public void RemoveValue()
    {
        if (positionChangerPointer.fillAmount > 0)
            positionChangerPointer.fillAmount -= 0.01f;
    }
}
