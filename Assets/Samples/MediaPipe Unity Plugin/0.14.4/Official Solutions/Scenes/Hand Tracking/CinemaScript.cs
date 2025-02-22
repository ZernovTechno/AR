using Mediapipe.Unity;
using Mediapipe.Unity.Sample.HandTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinemaScript : MonoBehaviour
{
    [SerializeField] private RawImage LeftCanvas; // Image for screen-transmission from pc
    [SerializeField] private RawImage RightCanvas; // Image for screen-transmission from pc
    [SerializeField] private RawImage TheaterScreen; // Image for screen-transmission from pc
    [SerializeField] public RawImage ScreenCastScreen;

    [SerializeField] private GameObject Theater;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject InterfaceObject;
    [SerializeField] private GameObject TheaterController;

    [SerializeField] private GameObject ScreenCast;

    [SerializeField] private GameObject ScreenCaster;

    [SerializeField] private Mediapipe.Unity.Screen CameraScript;
    [SerializeField] private MJPEGStreamDecoder MJPEGSD;

    private Vector3 CameraPosition = new Vector3(4.5f, -40, -65);
    private Vector3 InterfacePosition = new Vector3(4.5f, -40, -65);
    private Vector3 DefaultPosition = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
    }
    public void Update()
    {
        if (!ScreenCast.activeSelf && !LeftCanvas.gameObject.activeSelf)
        {
            TheaterScreen.texture = CameraScript.texture;
        }
    }

    public void TurnCinemaOn()
    {
        if (Theater.activeSelf)
        {
            Theater.SetActive(false);
            MJPEGSD.OutTexture = ScreenCastScreen;
            ScreenCaster.SetActive(true);
            PlayerObject.transform.position = DefaultPosition;
            InterfaceObject.transform.position = DefaultPosition;
            LeftCanvas.gameObject.SetActive(true);
            RightCanvas.gameObject.SetActive(true);
            //TheaterController.gameObject.SetActive(false);
        }
        else
        {
            Theater.SetActive(true);
            MJPEGSD.OutTexture = TheaterScreen;
            ScreenCaster.SetActive(false);
            PlayerObject.transform.position = CameraPosition;
            InterfaceObject.transform.position = InterfacePosition;
            LeftCanvas.gameObject.SetActive(false);
            RightCanvas.gameObject.SetActive(false);
            //TheaterController.gameObject.SetActive(true);
        }
    }

    public void goLeft()
    {
        CameraPosition.x = CameraPosition.x - 10;
        InterfacePosition.x = InterfacePosition.x - 10;
        PlayerObject.transform.position = CameraPosition;
        InterfaceObject.transform.position = InterfacePosition;
    }

    public void goRight()
    {
        CameraPosition.x = CameraPosition.x + 10;
        InterfacePosition.x = InterfacePosition.x + 10;
        PlayerObject.transform.position = CameraPosition;
        InterfaceObject.transform.position = InterfacePosition;
    }

    public void goDown()
    {
        CameraPosition.z = CameraPosition.z + 20;
        CameraPosition.y = CameraPosition.y - 12;
        InterfacePosition.z = InterfacePosition.z + 20;
        InterfacePosition.y = InterfacePosition.y - 12;
        PlayerObject.transform.position = CameraPosition;
        InterfaceObject.transform.position = InterfacePosition;
    }

    public void goUp()
    {
        CameraPosition.z = CameraPosition.z - 20;
        CameraPosition.y = CameraPosition.y + 12;
        InterfacePosition.z = InterfacePosition.z - 20;
        InterfacePosition.y = InterfacePosition.y + 12;
        PlayerObject.transform.position = CameraPosition;
        InterfaceObject.transform.position = InterfacePosition;
    }
}
