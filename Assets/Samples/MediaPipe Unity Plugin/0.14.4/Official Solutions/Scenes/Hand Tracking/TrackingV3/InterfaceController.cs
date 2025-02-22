using Mediapipe.Unity.CoordinateSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    void CreateBoxCollider()
    {
        BoxCollider ColliderBoxCollider = this.gameObject.AddComponent<BoxCollider>();
        ColliderBoxCollider.isTrigger= true;
        ColliderBoxCollider.size = new Vector3(this.GetComponent<RectTransform>().rect.width, this.GetComponent<RectTransform>().rect.height, 5);
    }
    void Start()
    {
        CreateBoxCollider();
    }
}
