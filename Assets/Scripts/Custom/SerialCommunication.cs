using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

[System.Serializable]
public class StringEvent : UnityEvent<string> {}
public class SerialCommunication : MonoBehaviour
{
    public static StringEvent OnNewMessageReceived = new StringEvent();

    // Arduino Sketch Servo: sketch_nov25c
    // Arduino Distance & NFC Reader: sketch_nov28b
    // Arduino Heart Sensor: sketch_dec06c

    // IOThread initialisation
    private Thread IOThread1;
    private Thread IOThread2;
    private Thread IOThread3;
    private Thread IOThread4;

    // SerialPort initialisation
    private static SerialPort sp1;
    private static SerialPort sp2;
    private static SerialPort sp3;
    private static SerialPort sp4;

    // Port & baudrate variables
    public string portName1 = "/dev/cu.usbmodem1101";
    public int baudRate1 = 115200;
    public string portName2 = "/dev/cu.usbserial-0001";
    public int baudRate2 = 115200;
    public string portName3 = "/dev/cu.usbserial-0005";
    public int baudRate3 = 115200;
    public string portName4 = "/dev/cu.usbserial-0005";
    public int baudRate4 = 115200;

    // Buffer & messagequeue initialisation
    private static string buffer1 = "";
    private static string buffer2 = "";
    private static string buffer3 = "";
    private static string buffer4 = "";
    private static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    // Data handling variables
    private static string currentData = "";
    private static string incomingData1 = "";
    private static string incomingData2 = "";
    private static string incomingData3 = "";
    private static string incomingData4 = "";
    private static bool continueRunning = false;
    public static string latestMessage;
    public static string latestMessageSp1;
    public static string latestMessageSp2;
    public static string latestMessageSp3;
    public static string latestMessageSp4;
    private string previousMessage = "";

    void Start()
    {   
        if (OnNewMessageReceived == null)
        {
            OnNewMessageReceived = new StringEvent();
        }

        continueRunning = true;

        // Launch IOThread1
        IOThread1 = new Thread(() => DataThread1(portName1, baudRate1));
        IOThread1.Start();

        // Launch IOThread2
        IOThread2 = new Thread(() => DataThread2(portName2, baudRate2));
        IOThread2.Start();

        // Launch IOThread3
        IOThread3 = new Thread(() => DataThread3(portName3, baudRate3));
        IOThread3.Start();

        // Launch IOThread4
        IOThread4 = new Thread(() => DataThread4(portName4, baudRate4));
        IOThread4.Start();
    }

    // DataThread1 method
    private static void DataThread1(string port, int rate)
    {
        sp1 = new SerialPort(port, rate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };

        if (sp1.IsOpen) {
            sp1.Close();
        }

        sp1.Open();

        while (continueRunning)
        {
            if (sp1.IsOpen)
            {
                if (incomingData1 != "")
                {
                    sp1.Write(incomingData1);
                    incomingData1 = "";
                }

                try
                {
                    buffer1 += sp1.ReadExisting();
                    int endMessageIndex;
                    while ((endMessageIndex = buffer1.IndexOf('\n')) >= 0)
                    {
                        string completeMessage = buffer1.Substring(0, endMessageIndex);
                        buffer1 = buffer1.Substring(endMessageIndex + 1);
                        latestMessageSp1 = completeMessage;
                        messageQueue.Enqueue(completeMessage);
                    }
                }
                catch (TimeoutException) 
                {
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
            Thread.Sleep(50);
        }

        if (sp1.IsOpen) {
            sp1.Close();
        }
    }

    // DataThread2 method
    private static void DataThread2(string port, int rate)
    {
        sp2 = new SerialPort(port, rate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };

        if (sp2.IsOpen) {
            sp2.Close();
        }
       sp2.Open();

        while (continueRunning)
        {
            if (sp2.IsOpen)
            {
                if (incomingData2 != "")
                {
                    sp2.Write(incomingData2);
                    incomingData2 = "";
                }

                try
                {
                    buffer2 += sp2.ReadExisting();
                    int endMessageIndex;
                    while ((endMessageIndex = buffer2.IndexOf('\n')) >= 0)
                    {
                        string completeMessage = buffer2.Substring(0, endMessageIndex);
                        buffer2 = buffer2.Substring(endMessageIndex + 1);
                        latestMessageSp2 = completeMessage;
                        messageQueue.Enqueue(completeMessage);
                    }
                }
                catch (TimeoutException) 
                {
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
            Thread.Sleep(50);
        }

        if (sp2.IsOpen) {
            sp2.Close();
        }
    }

    private static void DataThread3(string port, int rate)
    {
        sp3 = new SerialPort(port, rate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };

        if (sp3.IsOpen) {
            sp3.Close();
        }
        sp3.Open();

        while (continueRunning)
        {
            if (sp3.IsOpen)
            {
                if (incomingData3 != "")
                {
                    sp3.Write(incomingData3);
                    incomingData3 = "";
                }

                try
                {
                    buffer3 += sp3.ReadExisting();
                    int endMessageIndex;
                    while ((endMessageIndex = buffer3.IndexOf('\n')) >= 0)
                    {
                        string completeMessage = buffer3.Substring(0, endMessageIndex);
                        buffer3 = buffer3.Substring(endMessageIndex + 1);
                        latestMessageSp3 = completeMessage;
                        messageQueue.Enqueue(completeMessage);
                    }
                }
                catch (TimeoutException) 
                {
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
            Thread.Sleep(50);
        }

        if (sp3.IsOpen) {
            sp3.Close();
        }
    }

    private static void DataThread4(string port, int rate)
    {
        sp4 = new SerialPort(port, rate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };

        if (sp4.IsOpen) {
            sp4.Close();
        }
         sp4.Open();

        while (continueRunning)
        {
            if (sp4.IsOpen)
            {
                if (incomingData4 != "")
                {
                    sp4.Write(incomingData4);
                    incomingData4 = "";
                }

                try
                {
                    buffer4 += sp4.ReadExisting();
                    int endMessageIndex;
                    while ((endMessageIndex = buffer4.IndexOf('\n')) >= 0)
                    {
                        string completeMessage = buffer4.Substring(0, endMessageIndex);
                        buffer4 = buffer4.Substring(endMessageIndex + 1);
                        latestMessageSp4 = completeMessage;
                        messageQueue.Enqueue(completeMessage);
                    }
                }
                catch (TimeoutException) 
                {
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
            Thread.Sleep(50);
        }

        if (sp4.IsOpen) {
            sp4.Close();
        }
    }

#if UNITY_EDITOR
    void OnEnable() {
        EditorApplication.playModeStateChanged += StopThreadOnExit;
    }

    void OnDisable() {
        EditorApplication.playModeStateChanged -= StopThreadOnExit;
    }

    private void StopThreadOnExit(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingPlayMode) {
            StopThread1();
            StopThread2();
            StopThread3();
            StopThread4();
        }
    }
#endif

    private void OnDestroy()
    {
        StopThread1();
        StopThread2();
        StopThread3();
        StopThread4();
    }

    //Stopping threads
    private void StopThread1() {
        continueRunning = false;
        if (IOThread1 != null) {
            IOThread1.Join();
            IOThread1 = null;
        }
    }
    private void StopThread2() {
        continueRunning = false;
        if (IOThread2 != null) {
            IOThread2.Join();
            IOThread2 = null;
        }
    }
    private void StopThread3() {
        continueRunning = false;
        if (IOThread3 != null) {
            IOThread3.Join();
            IOThread3 = null;
        }
    }
    private void StopThread4() {
        continueRunning = false;
        if (IOThread4 != null) {
            IOThread4.Join();
            IOThread4 = null;
        }
    }
    void Update()
    {
        while (!messageQueue.IsEmpty)
        {
            if (messageQueue.TryDequeue(out string message))
            {
                message = message.Trim();
                Debug.Log(message);
                if (!string.IsNullOrWhiteSpace(message) && message != previousMessage)
                {   
                    previousMessage = message;
                    latestMessage = message;
                    currentData = message;
                    OnNewMessageReceived.Invoke(latestMessage);
                }
            }
        }
    }

    public static void SendSerialMessageArduino(string message)
    {
        incomingData1 = message;
        Debug.Log("Sent back message to Arduino: " + incomingData1);
    }
    public static void SendSerialMessageHapticlabs(string message)
    {
        incomingData2 = message;
        Debug.Log("Sent back message to Hapticlabs: " + incomingData2);
    }

}