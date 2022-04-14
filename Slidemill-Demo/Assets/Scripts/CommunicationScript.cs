using System;
using System.Globalization;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationScript : MonoBehaviour
{
    public QuestToESP32Connector connector; 
    public TMPro.TextMeshPro textDisplay; //TextMeshes are a fun way to display text in VR (since there's no console to read logs from).
    private float acc_threshold = 0.25f;
    private float timeSinceLastMessage;
    public TMPro.TextMeshPro timeDisplay;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastMessage = 0;
        if (textDisplay != null)
            textDisplay.text = "Connecting..."; //If you have a TextMesh, this will set its text.
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // Not currently sending any messages back to the ESP, keep all things here
    //     // connector.SendMessage("This is a test message"); //This is how you send messages to the ESP.
    // }

    public void onMessageRecieved(string message) //Things inside this function will happen when the quest recieves a message from the ESP.
    {
        timeSinceLastMessage = 0;
        if (textDisplay != null) {
            string[] str_split = message.Split(',');
            bool moving = false;

            List<float> vals = new List<float>(6);   // in order acceleration x, y, z, gyro x, y, z
            int i = 0;
            foreach (var sub in str_split) {
                float cur = float.Parse(sub, CultureInfo.InvariantCulture.NumberFormat);
                vals.Add(cur);
                if (i < 3 && cur > acc_threshold) {
                    moving = true;
                }
            }

            if (moving) {
                textDisplay.text = "ESP32 is currently moving";
            } else {
                textDisplay.text = "ESP32 is currently still";
            }
            // textDisplay.text = message;
        }
    }

    public void onConnected()  //This is called when the ESP connects to the Quest.
    {
        if (textDisplay != null)
            textDisplay.text = "Connected!";
    }

    public void Update() {
        timeSinceLastMessage += Time.deltaTime;
        if (timeDisplay != null) {
            timeDisplay.text = timeSinceLastMessage.ToString();
        }
    }
}