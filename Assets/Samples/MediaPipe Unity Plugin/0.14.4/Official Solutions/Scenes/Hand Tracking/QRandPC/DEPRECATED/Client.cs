using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using NetMQ.Sockets;
using NetMQ;
using ZXing;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class Client : MonoBehaviour
    {
        public enum ClientStatus
        {
            Inactive,
            Activating,
            Active,
            Deactivating
        }
    
        [SerializeField] public string host;
        [SerializeField] private string port;
        [SerializeField] public RawImage screen;
        [SerializeField] private GameObject ScreenCaster;
        [SerializeField] private GameObject BarCodeReader;
        private Texture2D tex;
        public Vector3 Finger;
        private Thread RequestLoop;
        private bool active;
        string messageB64;

        private void Start()
        {
            tex = new Texture2D(1, 1);
            ThreadStart();
        }

        public void ThreadStart()
        {
            active = screen.gameObject.activeSelf;
            RequestLoop = new Thread(OnClientRequest);
            RequestLoop.Start();
        }
        public void Update()
        {
            active = screen.gameObject.activeSelf;
            if (RequestLoop.ThreadState == ThreadState.Running && active)
            {
                tex.LoadImage(Convert.FromBase64String(messageB64));
                screen.texture = tex;
            }
            else if (RequestLoop.ThreadState== ThreadState.Stopped && active) 
            { 
                qragain(); 
            }
        }

        private void OnClientRequest()
        {
            while (active)
            {
                var messageReceived = false;
                var message = "";
                AsyncIO.ForceDotNet.Force();
                var timeout = new TimeSpan(0, 0, 1);
                using (var socket = new RequestSocket())
                {
                    socket.Connect($"tcp://{host}:{port}");
                    if (socket.TrySendFrame(Finger.ToString()))
                    {
                        messageReceived = socket.TryReceiveFrameString(timeout, out message);
                    }
                }

                NetMQConfig.Cleanup();
                if (!messageReceived)
                {
                    break;
                }
                messageB64 = message;
            }
        }

        public void qragain()
        {
            ScreenCaster.SetActive(false);
            BarCodeReader.SetActive(true);
        }
    }
}