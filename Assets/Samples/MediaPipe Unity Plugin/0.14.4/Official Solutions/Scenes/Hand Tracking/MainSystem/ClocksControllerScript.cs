using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class ClocksControllerScript : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ClocksText;
        void Start()
        {
            InvokeRepeating("UpdateDateTime", 1, 1);
        }

        // Update is called once per frame
        public void UpdateDateTime()
        {
            ClocksText.text = DateTime.Now.ToLongTimeString();
        }
    }
}
