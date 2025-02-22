// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

/* Hey! This is me - developer, who write this code in august 2024. My name is Zernov, I'm from Yekaterinburg, Russia. 
 * My Youtube channel - https://www.youtube.com/@zernovtech
 * My GitHub - https://github.com/ZernovTechno
 * My telegram channel - https://t.me/zernovyt
 * And my VK (Russian social network) - https://vk.com/zernovyt
 * 
 * You can edit whatever you want, so I doesn't promise that should work :)
 * Please contact me, if you doesn't understand something of this.
 * 
 * If it's 2030, or later - please, remember me what is this)
 */

using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Mediapipe.Unity.Sample.HandTracking
{
    internal class TrackingByLandmarksListController : MonoBehaviour
    {
        private IReadOnlyList<NormalizedLandmarkList> _currentHandLandmarkLists; // Main list with all hand landmarks
        private IReadOnlyList<NormalizedRect> _currentRectsLists; // Main list with all rectangles for hands
        [SerializeField] private RawImage _screen; // Var for main screen (Used for coordinates-converting
        [SerializeField] private GameObject _annotationLayer; // Var for layers with hand prefabs
        [SerializeField] private Text DebugText; // Debug text panel (Above the screen)

        [SerializeField] private RawImage ScreenCaster; // Image for screen-transmission from pc

        [SerializeField] public Camera leftCamera;
        [SerializeField] public Camera rightCamera;
        [SerializeField] public GameObject PlayerObject;

        [SerializeField] public Canvas LastContactedMenu; // I dont know what the fuck is it

        [SerializeField] public Canvas menu; // Menu
        [SerializeField] public Canvas menuCaller; // Button for show menu
        [SerializeField] public GameObject menuParent;

        [SerializeField] public GameObject indexPointer; // Pointer for index finger
        [SerializeField] public GameObject MiddlePointer; // for middle finger
        [SerializeField] public GameObject thumbPoint; // thumb finger
        [SerializeField] public GameObject centerPoint; // for center of screen
        // Pointers is used for coordinates-converting (from local to world)

        [SerializeField] public GameObject Cylinder; // Circle in the point when you changing position of panel

        [SerializeField] public Client ClientScript; // Script, used for screencast from pc

        [SerializeField] CommunicatorWithBrowser Communicator;

        [SerializeField] public GameObject BrowserPointer; // thumb finger
        [SerializeField] public GameObject KeyboardPointer; // for center of screen


        private bool ActiveCenterPoint = true; // Is center point used for system 
        private bool ActiveHandTracking = true; // Is hands used for system
        private bool IsListActive = false; // Is landmarks list configured to work (and exist)

        // timers
        private int timer = 0; 
        private int timerthumb = 0;
        private int timercenter = 0;

        // Points
        private Vector3 IndexFingerPoint = new Vector3(0, 0, 0);
        private Vector3 ThumbFingerPoint = new Vector3(0, 0, 0);
        private Vector3 BottomHandPoint = new Vector3(0, 0, 0);
        private Vector3 CenterPoint = new Vector3(0, 0, 0);
        private Vector3 MiddlePoint = new Vector3(0, 0, 0);

        private Vector3 SecondHand_BottomPoint = new Vector3(0, 0, 0);
        private Vector3 SecondHand_TopPoint = new Vector3(0, 0, 0);

        private Vector3 IndexFingerPointOld = new Vector3(0, 0, 0);
        private Vector3 CenterPointOld = new Vector3(0, 0, 0);
        public void Awake()
        {
            UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep; // Set the parameter to turn off screen timer (turn off time-sleeping)
        }

        public void Start()
        {
            Debug.Log("Tracking machine started!");
            DebugText.text = "Машина трекинга пущена!";
            _annotationLayer.transform.localPosition = new Vector3(0,0,-650); // Set position of annotation screen (hand tracking)
        }

        public void HandTracking()
        {
            Debug.Log("Hand tracking is now set.");
            DebugText.text = "Трекинг рук выбран!";
            ActiveCenterPoint = false;
            ActiveHandTracking = true;
        }

        public void CenterTracking()
        {
            Debug.Log("Center tracking is now set.");
            DebugText.text = "Трекинг центральной точкой выбран!";
            ActiveCenterPoint = true;
            ActiveHandTracking = false;
        }

        public void Open_Menu()
        {
            Debug.Log("Menu opened.");
            DebugText.text = "Меню открыто!";
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
            menuParent.transform.rotation = PlayerObject.transform.rotation;
        }

        private void LineArrow() // Display the arrow, that needed to change position of panel
        {
            if (Vector3.Distance(IndexFingerPoint, ThumbFingerPoint) <= 200 && Vector3.Distance(IndexFingerPoint, BottomHandPoint) >= 350 && IsListActive)
            {
                var start = MiddlePointer.transform.position;
                var end = Vector3.Lerp(indexPointer.transform.position, thumbPoint.transform.position, 0.5f);
                Cylinder.transform.position = Vector3.Lerp((end + (end - start).normalized * 50), leftCamera.transform.position, 0.07f);
                Cylinder.SetActive(true);
            } 
            else
            {
                Cylinder.SetActive(false);
            }
        }

        private void SetCoordinates() // Set all coordinates to their variables
        {
            if (IsListActive)
            {
                BottomHandPoint = _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[0]);
                MiddlePoint = Vector3.Lerp(_screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[6]), _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[3]), 0.5f);
                MiddlePointer.gameObject.transform.localPosition = new Vector3(0 - MiddlePoint.x, MiddlePoint.y, MiddlePointer.gameObject.transform.localPosition.z);
                IndexFingerPoint = _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[8]);
                ThumbFingerPoint = _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[4]);
                indexPointer.transform.localPosition = new Vector3(0 - IndexFingerPoint.x, IndexFingerPoint.y, indexPointer.transform.localPosition.z);
                thumbPoint.transform.localPosition = new Vector3(0 - ThumbFingerPoint.x, ThumbFingerPoint.y, thumbPoint.transform.localPosition.z);
            }
        }
        private void DistanceChecker() // Check the distance between points (And their old states), and count the timers
        {
            if (IsListActive)
            {

                if (Vector3.Distance(IndexFingerPoint, IndexFingerPointOld) <= 50)
                {
                    timer++;
                }
                else if (timer >= 2 && timer <= 50) timer -= 2;

                if (Vector3.Distance(IndexFingerPoint, ThumbFingerPoint) <= 100 && Cylinder.activeSelf)
                {
                    timerthumb++;
                }
                else if (timerthumb >= 2 && timerthumb <= 50) timerthumb -= 1;
            }

            if (ActiveCenterPoint)
            {
                CenterPoint = centerPoint.transform.position;
                if (Vector3.Distance(CenterPoint, CenterPointOld) < 50)
                {
                    timercenter++;
                }
                else if (timercenter >= 2 && timercenter <= 50) timercenter -= 2;

                CenterPointOld = CenterPoint;
            }
            IndexFingerPointOld = IndexFingerPoint;
        }

        private void Call_Menu() // Call menu button. Calls button if there two hands. 
        {
            if (IsListActive)
            {
                if (_currentHandLandmarkLists.Count > 1)
                {
                    SecondHand_TopPoint = _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[1].Landmark[9]);
                    SecondHand_BottomPoint = _screen.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[1].Landmark[0]);
                    var middlePoint = Vector3.Lerp(SecondHand_TopPoint, SecondHand_BottomPoint, 0.7f);
                    MiddlePointer.gameObject.transform.localPosition = new Vector3(0 - middlePoint.x, middlePoint.y, middlePoint.z);
                    menuCaller.gameObject.transform.position = Vector3.Lerp(MiddlePointer.gameObject.transform.position, leftCamera.transform.position, 0.65f);
                    menuCaller.gameObject.transform.rotation = PlayerObject.transform.rotation;
                    menuCaller.gameObject.SetActive(true);
                }
                else menuCaller.gameObject.SetActive(false);
            }
            else menuCaller.gameObject.SetActive(false);
        }

        private void CheckHandSize() // Check, if hand small (Maybe its a leg?)
        {
            if (IsListActive)
            {
                if ((_currentRectsLists[0].Height * _currentRectsLists[0].Width) < 0.2)
                {
                    IsListActive = false;
                    _annotationLayer.gameObject.SetActive(false);
                }
                else _annotationLayer.gameObject.SetActive(true);
            }
        }

        private void CheckTimersAndClick() // Checking, if timers overfilled and just do click
        {
            if (IsListActive)
            {
                if (timerthumb >= 50 && LastContactedMenu.gameObject.activeSelf)
                {
                    Debug.Log("Changing position of panel");
                    DebugText.text = "Перемещение окна. Новое положение: " + Cylinder.transform;
                    timerthumb = 0;
                    LastContactedMenu.gameObject.transform.position = Cylinder.transform.position;
                    LastContactedMenu.gameObject.transform.rotation = PlayerObject.transform.rotation;
                }

                if (timer >= 50)
                {
                    Debug.Log("Click");
                    DebugText.text = "Клик";
                    timer = 0;
                    EmulateClick(indexPointer.transform.position);
                }
            }

            if (timercenter >= 100 && ActiveCenterPoint)
            {
                Debug.Log("Click by center point");
                DebugText.text = "Клик центральной точкой";
                timercenter = 0;
                EmulateClick(CenterPoint);
            }
        }

        void Update()
        {
            IsListActive = ((_currentHandLandmarkLists != null) && (ActiveHandTracking)); // Check if Landmarks list exists and not null, and hand tracking is active.

            CheckHandSize(); // Run size checking
            SetCoordinates(); // Run coords setting
            DistanceChecker(); // Run distance checker

            LineArrow(); // Run arrow draw
            Call_Menu(); // Run menu call button
            CheckTimersAndClick(); 
        }

        public void EmulateClick(Vector3 position) // Do click just by coords on a screen.
        {
            Ray ray = new Ray(position, leftCamera.transform.position - position); // Prepare raycast from point to the camera
            float maxDistance = Vector3.Distance(position, leftCamera.transform.position); // Calculate distance to camera
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance); // Throw raycast from point to the camera
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Interface")) // If there interface on a way
                {
                    if (!hit.collider.gameObject.GetComponentInParent<Canvas>().CompareTag("SafePane")) LastContactedMenu = hit.collider.gameObject.GetComponentInParent<Canvas>();
                    ExecuteEvents.Execute(hit.collider.gameObject.GetComponent<Button>().gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler); // Do clicking on a button (If its a button) by emulating mouse event
                    break;
                }
                else if (hit.collider.CompareTag("ScreenCast")) // If there is a raycast...
                {
                    //ClientScript.Finger = ScreenCaster.GetComponent<RectTransform>().rect.GetPoint(_currentHandLandmarkLists[0].Landmark[8]);
                }
                else if (hit.collider.CompareTag("Browser")) // If there is a raycast...
                {
                    BrowserPointer.transform.position = hit.point;
                    Communicator.DoClickBrowser(BrowserPointer.transform.position.normalized);
                    DebugText.text = "" + Communicator.pointBrowser;
                }
                else if (hit.collider.CompareTag("BrowserKeyboard")) // If there is a raycast...
                {
                    //KeyboardPointer.transform.position = hit.point;
                    //Communicator.DoClickKeyboard(KeyboardPointer.transform.position.normalized);
                    //DebugText.text = "" + Communicator.pointKeyboard;
                }
            }
        }

        public void updateList(List<NormalizedLandmarkList> list) // Update list with hands landmarks
        {
            _currentHandLandmarkLists= list;
        }
        public void updateListRects(List<NormalizedRect> rectlist) // Update list with hands rects
        {
            _currentRectsLists = rectlist;
        }
    }
}