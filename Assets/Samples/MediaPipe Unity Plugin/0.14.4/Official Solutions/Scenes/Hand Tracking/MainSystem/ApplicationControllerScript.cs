using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.XR.Cardboard;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample.HandTracking;
using static UnityEngine.ParticleSystem;
using Mediapipe.Unity.Sample;
using Mediapipe;

public class ApplicationControllerScript : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    public void Awake()
    {
        UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep; // Set the parameter to turn off screen timer (turn off time-sleeping)
    }
    // Update is called once per frame
    void Update()
    {
        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }
        if (Api.IsCloseButtonPressed)
        {
            UnityEngine.Application.Quit();
        }
    }
    public void CLEAR_SAVED_PARAMS()
    {
        PlayerPrefs.DeleteAll();
    }
    public void STOP_THE_APP()
    {
        Application.Quit();
    }

    public void Restart_Android()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Application.isEditor) return;

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
#endif
    }

    public void RESTART_THE_APP()
    {
        InvokeRepeating("Restart_Android", 3, 0);
    }
}
