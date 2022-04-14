using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ArduinoBluetoothAPI;
using System;
using System.Text;

[System.Serializable]
public class UnityStringEvent : UnityEvent<string>
{
}

public class QuestToESP32Connector : MonoBehaviour
{
    [Tooltip("Make sure this matches the bluetooth name of your ESP32 in the firmware (arduino).")]
    public String deviceName = "ESP32";
    public UnityEvent onConnected;
    public UnityStringEvent onMessageRecieved;

    BluetoothHelper bluetoothHelper;
    private string received_message;

    private string tmp;
    void Start()
    {
        try
        {
            BluetoothHelper.BLE = false;
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.OnScanEnded += OnScanEnded;

            bluetoothHelper.setTerminatorBasedStream("\n");

            if (!bluetoothHelper.ScanNearbyDevices())
            {
                //scan didnt start (on windows desktop (not UWP))
                //try to connect
                bluetoothHelper.Connect();//this will work only for bluetooth classic.
                //scanning is mandatory before connecting for BLE.

            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);
        }
    }

    private void write(string msg)
    {
        tmp += ">" + msg + "\n";
    }

    void OnMessageReceived(BluetoothHelper helper)
    {
        received_message = helper.Read();
        Debug.Log(received_message);
        write("Received : " + received_message);
        onMessageRecieved.Invoke(received_message);
    }

    void OnConnected(BluetoothHelper helper)
    {
        try
        {
            helper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);

        }
        onConnected.Invoke();
    }

    void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {

        if (helper.isDevicePaired()) //we did found our device (with BLE) or we already paired the device (for Bluetooth Classic)
            helper.Connect();
        else
            helper.ScanNearbyDevices(); //we didn't
    }

    void OnConnectionFailed(BluetoothHelper helper)
    {
        write("Connection Failed");
        Debug.Log("Connection Failed");
    }

    float currentTime = 0;
    // Update is called once per frame
    /*void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 2)
        {
            SendMessage(currentTime.ToString());
            currentTime = 0;
        }
    }*/

    public void SendMessage(string message)
    {
        if (bluetoothHelper.isConnected())
        {
            bluetoothHelper.SendData(message);
        }
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}


