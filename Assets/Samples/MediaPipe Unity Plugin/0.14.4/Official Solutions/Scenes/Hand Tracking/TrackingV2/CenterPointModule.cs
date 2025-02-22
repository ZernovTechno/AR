using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class CenterPointModuleDEPRECATED : MonoBehaviour
    {
        [SerializeField] GameObject centerPointCanvas;
        [SerializeField] GameObject centerPoint;
        [SerializeField] GameObject PlayerObject;

        private Vector3 CenterPoint = new Vector3(0, 0, 0);
        private Vector3 CenterPointOld = new Vector3(0, 0, 0);
        private int WasOnPoint = 0;


        public float horizontalSpeed = 4.0f;
        public float verticalSpeed = 4.0f;

        private float h, v;

        private bool DoUpdateTimer = false;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("Update_Every_1s", 2, 1f);
            InvokeRepeating("Update_Every_400ms", 3, 0.4f);
        }
        void Update_Every_1s()
        {
            if (!DoUpdateTimer)
            {
                DoUpdateTimer = true;
                Debug.Log("Clicking timer was updated");
            }
        }

        private void Update()
        {
            h = horizontalSpeed * Input.GetAxis("Vertical");
            v = verticalSpeed * Input.GetAxis("Horizontal");
            h += horizontalSpeed * Input.GetAxis("Mouse Y");
            v += verticalSpeed * Input.GetAxis("Mouse X");
            if (centerPoint.transform.localPosition.x + v < 1200 && centerPoint.transform.localPosition.x + v > -1200 && centerPoint.transform.localPosition.y + h < 1200 && centerPoint.transform.localPosition.y + h > -1200)
            {
                centerPoint.transform.localPosition = new Vector3(centerPoint.transform.localPosition.x + v, centerPoint.transform.localPosition.y + h, centerPoint.transform.localPosition.z);
            }

            centerPointCanvas.transform.localPosition = new Vector3(centerPointCanvas.transform.localPosition.x + v, centerPointCanvas.transform.localPosition.y + h, centerPointCanvas.transform.localPosition.z);
        }

        private void DoClick()
        {
            if (DoUpdateTimer)
            {
                Ray ray = new Ray(CenterPoint, PlayerObject.transform.position - CenterPoint); // Prepare raycast from point to the camera

                float maxDistance = Vector3.Distance(CenterPoint, PlayerObject.transform.position); // Calculate distance to camera
                RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance); // Throw raycast from point to the camera
                Debug.DrawRay(CenterPoint, PlayerObject.transform.position - CenterPoint);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.CompareTag("StartThatPane")) // If there is a raycast...
                    {
                        if (!hit.collider.GetComponentInChildren<Canvas>(true).gameObject.activeSelf) hit.collider.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
                    }
                    if (hit.collider.CompareTag("Interface")) // If there interface on a way
                    {
                        //if (!hit.collider.gameObject.GetComponentInParent<Canvas>().CompareTag("SafePane")) LastContactedMeun = hit.collider.gameObject.GetComponentInParent<Canvas>();
                        ExecuteEvents.Execute(hit.collider.gameObject.GetComponent<UnityEngine.UI.Button>().gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler); // Do clicking on a button (If its a button) by emulating mouse event
                        break;
                    }
                    /*else if (hit.collider.CompareTag("ScreenCast")) // If there is a raycast...
                    {
                        //ClientScript.Finger = ScreenCaster.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[8]);
                    }
                    else if (hit.collider.CompareTag("Browser") && UseInteractionWithBrowser) // If there is a raycast...
                    {
                        BrowserPointer.transform.position = hit.point;
                        Communicator.DoClickBrowser(new Vector2((360 + BrowserPointer.transform.localPosition.x) / 720, (-BrowserPointer.transform.localPosition.y) / 350));
                        DebugText.text = "" + Communicator.pointBrowser;
                        _KeyboardScript.SetMode("Browser");
                    }
                    else if (hit.collider.CompareTag("KeyboardInteractive")) // If there is a raycast...
                    {
                        if (!hit.collider.gameObject.GetComponentInParent<Canvas>().CompareTag("SafePane")) LastContactedMenu = hit.collider.gameObject.GetComponentInParent<Canvas>();
                        _KeyboardScript.SetMode(hit.collider.gameObject.GetComponent<TextMeshProUGUI>());
                    }
                    else if (hit.collider.CompareTag("StartThatPane")) // If there is a raycast...
                    {
                        if (!hit.collider.gameObject.GetComponentInParent<Canvas>().CompareTag("SafePane")) LastContactedMenu = hit.collider.gameObject.GetComponentInParent<Canvas>();
                        _KeyboardScript.SetMode(hit.collider.gameObject.GetComponent<TextMeshProUGUI>());
                    }*/
                }
                DoUpdateTimer = false;
            }
        }

        // Update is called once per frame
        void Update_Every_400ms()
        {
            CenterPoint = centerPoint.transform.position;
            if (true)
            {

                if (Input.GetAxis("Submit") > 0 || Input.GetAxis("Cancel") > 0 || Input.GetAxis("Fire1") > 0 || Input.GetAxis("Jump") > 0 || Input.GetAxis("Fire2") > 0 || Input.GetAxis("Fire3") > 0)
                {
                    DoClick();
                }
                if (Vector3.Distance(CenterPoint, CenterPointOld) < 50)
                {
                    if (WasOnPoint >=3)
                    {
                        WasOnPoint = 0;
                        DoClick();
                    }
                    else WasOnPoint += 1;
                }
                else WasOnPoint = 0;
                CenterPointOld = CenterPoint;
            }
        }

    }
}
