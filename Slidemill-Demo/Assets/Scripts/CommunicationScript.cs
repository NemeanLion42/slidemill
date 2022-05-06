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
    public GameObject foot;
    public Vector3 startPosition;
    public bool move;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastMessage = 0;
        if (textDisplay != null)
            textDisplay.text = "Connecting..."; //If you have a TextMesh, this will set its text.
        foot.GetComponent<Rigidbody>().detectCollisions = false;
        foot.transform.position = startPosition;
        move = false;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // Not currently sending any messages back to the ESP, keep all things here
    //     // connector.SendMessage("This is a test message"); //This is how you send messages to the ESP.
    // }

    public void ChangeMove() {
        move = !move;
    }

    public void onMessageRecieved(string message) //Things inside this function will happen when the quest recieves a message from the ESP.
    {
        timeSinceLastMessage = 0;
        if (message.Trim().Equals("reset")) {
            foot.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            foot.GetComponent<Rigidbody>().velocity = Vector3.zero;
            foot.transform.rotation = Quaternion.identity;
            foot.transform.position = startPosition;
            return;
        }
        if (textDisplay != null) {
            string[] str_split = message.Split(',');

            List<float> vals = new List<float>(10);   // in order foot, acceleration x, y, z, gyro x, y, z, mag x, y, z
            int i = 0;
            foreach (var sub in str_split) {
                float cur = float.Parse(sub, CultureInfo.InvariantCulture.NumberFormat);
                vals.Add(cur);
                i++;
            }
            string toDisplay = "foot: " + vals[0] + "\n" +
                               "xAcc: " + vals[1] + "\n" +
                               "yAcc: " + vals[2] + "\n" +
                               "zAcc: " + vals[3] + "\n" +
                               "xRot: " + vals[4] + "\n" +
                               "yRot: " + vals[5] + "\n" +
                               "zRot: " + vals[6];

            textDisplay.text = toDisplay;
            if (move) {
                foot.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-vals[2], -vals[3], vals[1])*foot.GetComponent<Rigidbody>().mass);
                foot.GetComponent<Rigidbody>().AddForce(new Vector3(0, 9.8f, 0)*foot.GetComponent<Rigidbody>().mass);
            }
            foot.GetComponent<Rigidbody>().angularVelocity = foot.transform.TransformDirection(new Vector3(-vals[5], -vals[6], vals[4]));
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
        if (move) {
            foot.GetComponent<Rigidbody>().AddForce(-foot.GetComponent<Rigidbody>().velocity*0.01f*Time.deltaTime*foot.GetComponent<Rigidbody>().mass);
        }
    }
}