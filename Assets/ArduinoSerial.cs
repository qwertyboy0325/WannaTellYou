using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;

public class ArduinoSerial : MonoBehaviour
{
    // Props for serial port dropdown list.
    [HideInInspector]
    public string selectedPort;
    [HideInInspector]
    public string[] availablePorts;

    //public GameObject testCube;
    SerialPort serialPort;

    Thread readThread;

    bool isNewMessage;
    string readMessage;



    void Start()
    {
        serialPort = new SerialPort();
        serialPort.ReadTimeout = 10;
        serialPort.PortName = "COM3";
        serialPort.BaudRate = 9600;
        serialPort.Open();
    }


    private void Update()
    {
        // 檢查串口是否打開
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // 讀取串口數據
                string data = serialPort.ReadLine();
                Debug.Log("Received data: " + data);
                isNewMessage = true;
                char firstState = data[0];
                char secondState = data[1];
                int status;
                status = int.Parse(firstState.ToString()) << 1;

                status |= int.Parse(secondState.ToString());
                Debug.Log("Arduino status: " + status);
                if (GameManager.Instance.state == EGameState.AwaitAnswer)
                    QuestionManager.Instance.RecievePhoneStatus(status);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("SerialPort error: " + e.Message);
            }
        }
    }
    //readData();


    void readData()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                readMessage = serialPort.ReadLine();
                Debug.Log("Arduino send: " + readMessage);
                isNewMessage = true;
                int status = int.Parse(readMessage);
                QuestionManager.Instance.RecievePhoneStatus(status);

            }
            catch (System.Exception e)
            {
                if (e.Message != "The operation has timed out.")
                    Debug.Log(e.Message);
            }
        }
    }

    public void writeData(string message)
    {
        Debug.Log(message);
        try
        {
            serialPort.WriteLine(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            //Open the serial serialPort
            serialPort.Close();
        }
    }

}