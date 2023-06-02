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

    public GameObject testCube;
    SerialPort serialPort;

    Thread readThread;

    bool isNewMessage;
    string readMessage;
    public Toggle ledToggle;

    private void OnEnable()
    {
        RefreshSerialPorts();
        ReconnectSerialPort();
    }

    void ReconnectSerialPort()
    {
        try
        {
            // Open the serial serialPort
            serialPort.PortName = selectedPort;
            serialPort.BaudRate = 9600;
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
            catch (System.Exception e)
            {
                if (e.Message != "The operation has timed out.")
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

// �۩w�qInspectorø�s���A�Ω�ø�s�U�Ԧ����
[CustomEditor(typeof(ArduinoSerial))]
public class SerialPortDropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ø�s�q�{���ݩʽs�边
        DrawDefaultInspector();

        // ����}���ޥ�
        ArduinoSerial dropdown = (ArduinoSerial)target;

        // ����i�Ϊ���f�W��
        string[] availablePorts = dropdown.availablePorts;

        // �����ܪ���f
        string selectedPort = dropdown.selectedPort;

        // �p�G�i�Φ�f�����šA�hø�s�U�Ԧ����
        if (availablePorts.Length != 0 || availablePorts == null)
        {
            Debug.Log(availablePorts.Length);
            // �p�G��ܪ���f���b�i�Φ�f�C���A�N��]�m���Ĥ@�ӥi�Φ�f
            if (!System.Array.Exists(availablePorts, element => element == selectedPort))
            {
                selectedPort = availablePorts.Length > 0 ? availablePorts[0] : "";
                dropdown.selectedPort = selectedPort;
            }

            // ø�s�U�Ԧ����
            int selectedIndex = EditorGUILayout.Popup("Serial Port", System.Array.IndexOf(availablePorts, selectedPort), availablePorts);

            // ��s��ܪ���f
            dropdown.selectedPort = availablePorts[selectedIndex];
        }
        else
        {
            int selectedIndex = EditorGUILayout.Popup("Serial Port", 0, availablePorts);
            dropdown.selectedPort = "";
        }
    }
}