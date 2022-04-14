using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationExampleScript : MonoBehaviour
{
    public QuestToESP32Connector connector; 
    public TextMesh textDisplay; //TextMeshes are a fun way to display text in VR (since there's no console to read logs from).

    // Start is called before the first frame update
    void Start()
    {
        if (textDisplay != null)
            textDisplay.text = "Connecting..."; //If you have a TextMesh, this will set its text.
    }

    // Update is called once per frame
    void Update()
    {
        connector.SendMessage("This is a test message"); //This is how you send messages to the ESP.
    }

    public void onMessageRecieved(string message) //Things inside this function will happen when the quest recieves a message from the ESP.
    {
        if (textDisplay != null)
            textDisplay.text = message;
    }

    public void onConnected()  //This is called when the ESP connects to the Quest.
    {
        if (textDisplay != null)
            textDisplay.text = "Connected!";
    }
}
