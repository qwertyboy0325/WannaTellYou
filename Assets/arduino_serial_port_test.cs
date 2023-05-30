using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;

public class arduino_serial_port_test : MonoBehaviour
{
    // Props for serial port dropdown list.
    [HideInInspector]
    public string selectedPort;
    [HideInInspector]
    public string[] availablePorts;

    public GameObject testCube;
    SerialPort serialPort;
    
    Thread readThread;

    bool isNewMessage;
    string readMessage;
    public Toggle ledToggle;

    private void OnEnable()
    {
        RefreshSerialPorts();
    }

    void RefreshSerialPorts()
    {
        availablePorts = SerialPort.GetPortNames();
    }

    void Start()
    {
        // serialPort = new SerialPort("/dev/cu.usbmodem11301", 9600);
        serialPort = new SerialPort();
        serialPort.ReadTimeout = 10;
        try
        {
            // Open the serial serialPort
            serialPort.Open();
            readThread = new Thread(new ThreadStart(readData));
            readThread.Start();
            Debug.Log("SerialPort start to connect.");
        }
        catch
        {
            Debug.Log("SerialPort connct error.");
        }
        
        Debug.Log("Open port success");

        // 清空串口緩衝區
        serialPort.DiscardInBuffer();

        // Led toggle function
        ledToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void Update()
    {
        if (isNewMessage)
        {
            if (readMessage.Trim() == "1")
            {
                testCube.SetActive(true);
            }
            else
            {
                testCube.SetActive(false);
            }
        }
        isNewMessage = false;
    }

    void readData()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                readMessage = serialPort.ReadLine();
                isNewMessage = true;
            }
            catch(System.Exception e)
            {
                if(e.Message != "The operation has timed out.")
                    Debug.Log(e.Message);
            }
        }
    }

    void writeData(string message)
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

    /// Toggle the Led
    void OnToggleValueChanged(bool val)
    {
        string ledStatus = val == true ? "1" : "0";
        writeData(ledStatus);
    }
}

// 自定義Inspector繪製器，用於繪製下拉式選單
[CustomEditor(typeof(arduino_serial_port_test))]
public class SerialPortDropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 繪製默認的屬性編輯器
        DrawDefaultInspector();

        // 獲取腳本引用
        arduino_serial_port_test dropdown = (arduino_serial_port_test)target;

        // 獲取可用的串口名稱
        string[] availablePorts = dropdown.availablePorts;

        // 獲取選擇的串口
        string selectedPort = dropdown.selectedPort;

        // 如果可用串口不為空，則繪製下拉式選單
        if (availablePorts.Length != 0)
        {
            Debug.Log(availablePorts.Length);
            // 如果選擇的串口不在可用串口列表中，將其設置為第一個可用串口
            if (!System.Array.Exists(availablePorts, element => element == selectedPort))
            {
                selectedPort = availablePorts.Length > 0 ? availablePorts[0] : "";
                dropdown.selectedPort = selectedPort;
            }

            // 繪製下拉式選單
            int selectedIndex = EditorGUILayout.Popup("Serial Port", System.Array.IndexOf(availablePorts, selectedPort), availablePorts);

            // 更新選擇的串口
            dropdown.selectedPort = availablePorts[selectedIndex];
        }
        else
        {
            int selectedIndex = EditorGUILayout.Popup("Serial Port", 0, availablePorts);
            dropdown.selectedPort = "";
        }
    }
}