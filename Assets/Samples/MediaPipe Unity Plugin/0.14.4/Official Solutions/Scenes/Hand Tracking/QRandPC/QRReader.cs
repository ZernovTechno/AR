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
using UnityEngine.Experimental.Rendering;

namespace Mediapipe.Unity
{
    public class QRReader : MonoBehaviour
    {
        [SerializeField] private MJPEGStreamDecoder MJPEGSD;
        [SerializeField] private Text QRResult;
        [SerializeField] private GameObject BarCodeReader;
        [SerializeField] private GameObject ScreenCaster;
        [SerializeField] private MessagingSystemScript MSS;

        private BarcodeReader reader = new BarcodeReader();
        public WebCamTexture CameraImage;
        private Result Result;
        private IPAddress IPChecker;

        public bool DoCastPhone = false;

        private void Awake()
        {
            qragain();
        }

        private void Update()
        {
            if (BarCodeReader.activeSelf)
            {
                try
                {
                    Result = reader.Decode(CameraImage.GetPixels32(), CameraImage.width, CameraImage.height);
                    if (Result != null)
                    {
                        MJPEGSD.stop = false;
                        QRResult.text = "QR декодирован.";
                        Debug.Log(Result.Text);
                        MJPEGSD.StartStream(Result.Text);
                        BarCodeReader.gameObject.SetActive(false);
                        ScreenCaster.gameObject.SetActive(true);
                    }
                    else if (Result != null)
                    {
                        QRResult.text = $"QR не является трансляцией. Содержание QR:{Result.Text}";
                    }
                }
                catch (Exception Ex) { }
            }
            if (MJPEGSD.stop)
            {
                QRResult.text = "Сбой подключения :(";
                BarCodeReader.gameObject.SetActive(true);
                ScreenCaster.gameObject.SetActive(false);
            }
        }

        public void qragain()
        {
            ScreenCaster.SetActive(false);
            BarCodeReader.SetActive(true);
        }
    }
}
