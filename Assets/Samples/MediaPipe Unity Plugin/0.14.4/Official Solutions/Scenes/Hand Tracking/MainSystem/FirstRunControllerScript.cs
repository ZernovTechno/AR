using Mediapipe.Unity.Sample.HandTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class FirstRunControllerScript : MonoBehaviour
{

    [SerializeField] MessagingSystemScript MessagingSystem;
    [SerializeField] GameObject FirstSet;
    [SerializeField] HandTrackingSolution Solution;

    [SerializeField] GameObject CenterPoint; // Menu
    // Start is called before the first frame update


    public void HandTracking()
    {
        PlayerPrefs.SetInt("TrackingType", 0);
        PlayerPrefs.Save();
        MessagingSystem.SetMessage("Включается трекинг рук...", Color.blue);
        CenterPoint.gameObject.SetActive(false);
    }

    public void CenterTracking()
    {
        PlayerPrefs.SetInt("TrackingType", 1);
        PlayerPrefs.Save();
        MessagingSystem.SetMessage("Включается трекинг головой...", Color.blue);
        CenterPoint.gameObject.SetActive(true);
    }

    public void SolutionOFF()
    {
        Solution.gameObject.SetActive(false);
    }


    void Start()
    {
        if (PlayerPrefs.HasKey("TrackingType"))
        {
            switch (PlayerPrefs.GetInt("TrackingType"))
            {
                case 0:
                    HandTracking();
                    break;
                case 1:
                    CenterTracking();
                    InvokeRepeating("SolutionOFF", 5, 0);
                    break;
            }
        }
        else
        {
            FirstSet.SetActive(true);
        }
    }
}
