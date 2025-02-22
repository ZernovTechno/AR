using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionUpdaterScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI VersionText; // Debug text panel (Above the screen)
    // Start is called before the first frame update
    void Start()
    {
        VersionText.text = "Reality Fusion " + Application.version;
    }
}
