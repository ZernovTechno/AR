using NetMQ.Sockets;
using NetMQ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using ZXing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Rendering;
using static Mediapipe.TfLiteInferenceCalculatorOptions.Types.Delegate.Types;

namespace Mediapipe.Unity.Sample.HandTracking
{
    public class QRReaderDEPRECATED : MonoBehaviour
    {
        [SerializeField] private Text QRResult;
        [SerializeField] private RawImage CameraImage;
        [SerializeField] private GameObject BarCodeReader;
        [SerializeField] private GameObject ScreenCaster;
        [SerializeField] public string host;
        [SerializeField] private string port;
        [SerializeField] public RawImage ScreenCastScreen;
        [SerializeField] public Camera ScreenShotCamera;
        [SerializeField] private RawImage Test;

        public Vector3 Finger;

        private Texture2D IncomeImage;
        private Thread RequestLoop;
        private bool active, stop;
        private string messageB64;

        private BarcodeReader reader = new BarcodeReader();
        private Texture2D QRTexture;
        private Result Result;
        private IPAddress IPChecker;
        private bool messageReceived = false;
        private TimeSpan timeout = new TimeSpan(0, 0, 1);
        private RenderTexture renderTexture;
        private Texture2D texture2D;
        private string ScreenShotInString = "NowNull";

        public bool DoCastPhone = false;

        public void SetCastPhone(bool state)
        {
            DoCastPhone = state;
        }
        void ProcessScreenshotOld()
        {
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            byte[] bytearr = texture2D.EncodeToJPG();
            ScreenShotInString = Convert.ToBase64String(bytearr);
        }
        private void Start()
        {
            QRTexture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            IncomeImage = new Texture2D(1, 1);
            texture2D = new Texture2D(640, 360, TextureFormat.ARGB32, false);
            renderTexture = new RenderTexture(640, 360, 24);
            ScreenShotCamera.targetTexture = renderTexture;
            ThreadStart();
        }

        public void ThreadStart()
        {
            active = ScreenCastScreen.gameObject.activeSelf;
            RequestLoop = new Thread(OnClientRequest);
            RequestLoop.Start();
        }

        private void OnApplicationQuit()
        {
            stop = true;
        }

        private void Update()
        {
            active = ScreenCastScreen.gameObject.activeSelf;
            if (BarCodeReader.activeSelf)
            {
                try
                {
                    if (QRTexture.height != CameraImage.texture.height && QRTexture.width != CameraImage.texture.width)
                    {
                        QRTexture = new Texture2D(CameraImage.texture.width, CameraImage.texture.height, TextureFormat.RGBA32, false);
                    }   
                    Graphics.CopyTexture(CameraImage.texture, QRTexture);
                    Result = reader.Decode(QRTexture.GetPixels32(), QRTexture.width, QRTexture.height);

                    if (Result != null && IPAddress.TryParse(Result.Text, out IPChecker))
                    {
                        Debug.Log(Result.Text);
                        host = Result.Text;
                        BarCodeReader.gameObject.SetActive(false);
                        ScreenCaster.gameObject.SetActive(true);
                        ThreadStart();
                    }
                    else if (Result != null)
                    {
                        QRResult.text = $"QR не €вл€етс€ трансл€цией. —одержание QR:{Result.Text}";
                    }
                }
                finally { }
            }

            if (RequestLoop.ThreadState == ThreadState.Running && active && messageB64 != null)
            {
                try
                {
                    if (DoCastPhone) ProcessScreenshotOld();
                    else ScreenShotInString = "NowNull";
                    IncomeImage.LoadImage(Convert.FromBase64String(messageB64));
                    ScreenCastScreen.texture = IncomeImage;
                }
                catch { }
            }
            
            if (RequestLoop.ThreadState == ThreadState.Stopped && active)
            {
                qragain();
            }
        }

        private void OnClientRequest()
        {
            AsyncIO.ForceDotNet.Force();
            while (active && !stop)
            {
                using (var socket = new RequestSocket())
                {
                    socket.Connect($"tcp://{host}:{port}");
                    if (socket.TrySendFrame(ScreenShotInString))
                    {
                        messageReceived = socket.TryReceiveFrameString(timeout, out messageB64);
                    }
                }

                NetMQConfig.Cleanup();
                if (!messageReceived)
                    break;
            }
        }
        public void qragain()
        {
            ScreenCaster.SetActive(false);
            BarCodeReader.SetActive(true);
        }
    }
}
