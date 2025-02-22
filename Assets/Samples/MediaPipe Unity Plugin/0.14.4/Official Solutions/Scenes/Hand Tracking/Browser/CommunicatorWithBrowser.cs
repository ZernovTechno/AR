using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CommunicatorWithBrowser : MonoBehaviour
{
    public bool ReadyToClickBrowser = false;
    public Vector2 pointBrowser;

    public bool DoClickBack;
    public bool DoClickForward;
    public bool DoClickGoogle;
    public bool DoClickReload;

    public string Command;
    public bool DoSendCommand;

    public string URL;
    public bool DoOpenURL;

    public void DoClickBrowser(Vector2 point)
    {
        pointBrowser = point;
        ReadyToClickBrowser = true;
    }

    public void DoClickBackMethod() { DoClickBack = true; }
    public void DoClickForwardMethod() { DoClickForward = true; }
    public void DoClickGoogleMethod() { DoClickGoogle = true; }
    public void DoClickReloadMethod() { DoClickReload = true; }
    public void DoOpenURLMethod(TextMeshProUGUI TextInput)
    {
        URL = TextInput.text;
        DoOpenURL = true;
    }
    public void DoSendCommandMethod(string _Command) 
    { 
        Command = _Command;
        DoSendCommand = true;
        Debug.Log("Key input worked on a Communicator:" + _Command);
    }
}
