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


public class MenuOpenerController : MonoBehaviour
{
    [SerializeField] HandInstrument HI;
    [SerializeField] Canvas MenuOpenerCanvas;
    [SerializeField] UnityEngine.UI.Image MenuOpener;
    [SerializeField] GameObject PlayerObject;
    [SerializeField] GameObject MenuParent;
    public bool Timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Timer = true;
    }
    public void ResetTimer()
    {
        Timer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (HI.TrackingIsWorking)
        {
            float DistanceBetweenIndexAndThumb = Vector3.Distance(HI.GetAnnotation(8, 0).transform.position, HI.GetAnnotation(4, 0).transform.position);
            float DistanceBetweenIndexAndMiddle = Vector3.Distance(HI.GetAnnotation(8, 0).transform.position, HI.GetAnnotation(12, 0).transform.position);
            float DistanceBetweenIndexAndHandBottom = Vector3.Distance(HI.GetAnnotation(0, 0).transform.position, HI.GetAnnotation(8, 0).transform.position);

            if (DistanceBetweenIndexAndThumb > 1.8 * DistanceBetweenIndexAndMiddle && DistanceBetweenIndexAndHandBottom > 2 * DistanceBetweenIndexAndThumb)
            {
                Vector3 ConfigureLocation =
                Vector3.Lerp(
                    Vector3.Lerp(
                        HI.GetAnnotation(0, 0).transform.position,
                        Vector3.Lerp(
                           HI.GetAnnotation(9, 0).transform.position,
                           HI.GetAnnotation(13, 0).transform.position,
                        0.5f),
                    0.7f),
                    PlayerObject.transform.position,
                    0.1f);
                MenuOpenerCanvas.gameObject.SetActive(true);
                MenuOpenerCanvas.gameObject.transform.position = ConfigureLocation;
                MenuOpenerCanvas.gameObject.transform.rotation = PlayerObject.transform.rotation;
                AddValue();
            }
            else RemoveValue();
        }
        else MenuOpenerCanvas.gameObject.SetActive(false);
    }

    public void AddValue()
    {
        if (MenuOpener.fillAmount < 1)
            MenuOpener.fillAmount += 0.01f;
        if (MenuOpener.fillAmount == 1)
        {
            MenuOpener.fillAmount = 0;
            MenuParent.transform.rotation = PlayerObject.transform.rotation;
            Timer = false;
            InvokeRepeating("ResetTimer", 5, 0);
            Debug.Log("Opening actual menu...");
        }

    }

    public void RemoveValue()
    {
        if (MenuOpener.fillAmount > 0)
            MenuOpener.fillAmount -= 0.01f;
        if (MenuOpener.fillAmount == 0) MenuOpenerCanvas.gameObject.SetActive(false);
    }
}
