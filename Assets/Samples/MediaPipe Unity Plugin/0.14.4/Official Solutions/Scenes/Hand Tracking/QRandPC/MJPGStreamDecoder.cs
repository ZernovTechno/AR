/*
 * I needed a simple MJPEG Stream Decoder and I couldn't find one that worked for me.
 * 
 * It reads a response stream and when there's a new frame it updates the render texture. 
 * That's it. No authenication or options.
 * It's something stupid simple for readimg a video stream from an equally stupid simple Arduino. 
 * 
 * I fixed most of the large memory leaks, but there's at least one small one left.
 */

using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class MJPEGStreamDecoder : MonoBehaviour
{
    [SerializeField] bool tryOnStart = false;
    [SerializeField] string defaultStreamURL = "";

    [SerializeField] public RawImage OutTexture;

    int MAX_RETRIES = 1;
    int retryCount = 0;
    public bool stop = false;

    byte[] nextFrame = null;

    Thread worker;
    int threadID = 0;

    Texture2D texture2D;

    static System.Random randu; // I use my own System.Random instead of the shared UnityEngine.Random to avoid collisions
    List<BufferedStream> trackedBuffers = new List<BufferedStream>();

    // Start is called before the first frame update
    void Start()
    {
        texture2D = new Texture2D(2, 2);
        randu = new System.Random(Random.Range(0, 65536));
        if (tryOnStart)
            StartStream(defaultStreamURL);
    }

    private void Update()
    {
        if (nextFrame != null)
        {
            SendFrame(nextFrame);
            nextFrame = null;
        }
    }

    private void OnDestroy()
    {
        foreach (var b in trackedBuffers)
        {
            if (b != null)
                b.Close();
        }
    }

    private void OnApplicationQuit()
    {
        stop = true;
    }

    public void StartStream(string url)
    {
        retryCount = 0;
        StopAllCoroutines();
        foreach (var b in trackedBuffers)
            b.Close();

        worker = new Thread(() => ReadMJPEGStreamWorker(threadID = randu.Next(65536), url));
        worker.Start();
    }

    void ReadMJPEGStreamWorker(int id, string url)
    {
        if (!stop)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            List<byte> frameBuffer = new List<byte>();

            int lastByte = 0x00;
            bool addToBuffer = false;

            BufferedStream buffer = null;
            try
            {
                Stream stream = webRequest.GetResponse().GetResponseStream();
                buffer = new BufferedStream(stream);
                trackedBuffers.Add(buffer);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
            int newByte;
            while (buffer != null)
            {
                if (threadID != id) return; // We are no longer the active thread! stop doing things damnit!
                if (!buffer.CanRead)
                {
                    Debug.LogError("Can't read buffer!");
                    break;
                }

                newByte = -1;

                try
                {
                    newByte = buffer.ReadByte();
                }
                catch
                {
                    break; // Something happened to the stream, start a new one
                }

                if (newByte < 0) // end of stream or failure
                {
                    continue; // End of data
                }

                if (addToBuffer)
                    frameBuffer.Add((byte)newByte);

                if (lastByte == 0xFF) // It's a command!
                {
                    if (!addToBuffer) // We're not reading a frame, should we be?
                    {
                        if (IsStartOfImage(newByte))
                        {
                            addToBuffer = true;
                            frameBuffer.Add((byte)lastByte);
                            frameBuffer.Add((byte)newByte);
                        }
                    }
                    else // We're reading a frame, should we stop?
                    {
                        if (newByte == 0xD9)
                        {
                            frameBuffer.Add((byte)newByte);
                            addToBuffer = false;
                            nextFrame = frameBuffer.ToArray();
                            frameBuffer.Clear();
                        }
                    }
                }

                lastByte = newByte;
            }

            if (retryCount < MAX_RETRIES)
            {
                retryCount++;
                Debug.LogFormat("[{0}] Retrying Connection {1}...", id, retryCount);
                foreach (var b in trackedBuffers)
                    b.Dispose();
                trackedBuffers.Clear();
                worker = new Thread(() => ReadMJPEGStreamWorker(threadID = randu.Next(65536), url));
                worker.Start();
            }
            else stop = true;
        }
    }

    bool IsStartOfImage(int command)
    {
        switch (command)
        {
            case 0x8D:
               // Debug.Log("Command SOI");
                return true;
            case 0xC0:
                //Debug.Log("Command SOF0");
                return true;
            case 0xC2:
                //Debug.Log("Command SOF2");
                return true;
            case 0xC4:
                //Debug.Log("Command DHT");
                break;
            case 0xD8:
                //Debug.Log("Command DQT");
                return true;
            case 0xDD:
                //Debug.Log("Command DRI");
                break;
            case 0xDA:
                //Debug.Log("Command SOS");
                break;
            case 0xFE:
                //Debug.Log("Command COM");
                break;
            case 0xD9:
                //Debug.Log("Command EOI");
                break;
        }
        return false;
    }

    void SendFrame(byte[] bytes)
    {
        texture2D.LoadImage(bytes);

        if (texture2D.width == 2)
            return; // Failure!

        OutTexture.texture = texture2D;
    }
}