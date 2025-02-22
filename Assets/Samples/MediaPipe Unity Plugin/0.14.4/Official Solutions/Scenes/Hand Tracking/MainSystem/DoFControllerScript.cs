using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoFControllerScript : MonoBehaviour
{
    [SerializeField] MessagingSystemScript messagingSystemScript;
    [SerializeField] GameObject Interface;
    [SerializeField] GameObject Player;

    string DoFType = "3DoF";
    // Start is called before the first frame update

    public void SetDoFType(string _DoFType)
    {
        messagingSystemScript.SetMessage("Ошибка установки DoF - " + _DoFType, Color.red);
        DoFType = _DoFType;
        switch (DoFType)
        {
            case "0DoF":
                Interface.transform.SetParent(Player.transform);
                break;
            case "3DoF":
                Interface.transform.SetParent(null);
                break;
            case "6Dof":
                break;
        }
        messagingSystemScript.SetMessage("Тип пространственного трекинга - " + DoFType, Color.green);
    }

}
