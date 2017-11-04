using System;
using System.Collections.Generic;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public GameObject chatWindow;
    public Button chatToogleBttn;
    private bool isChatWindowOpen;

    public InputField messageInput;
    public Button sendMessageBttn;

    public Text chatLogOutput;
    public int elementsInChatLog = 7;
    private Queue<string> chatLog = new Queue<string>();

    void Start() {
        chatLogOutput.text = string.Empty;
        chatWindow.SetActive(false);
        chatToogleBttn.onClick.AddListener(ToogleChatWindow);

        sendMessageBttn.onClick.AddListener(SendMessage);
    }

    private void SendMessage() {
        UpdateChatLog("Me", messageInput.text, DateTime.Now.ToString());
        if (messageInput.text != string.Empty) { 
            using (RTData data = RTData.Get()) {
                data.SetString(1, messageInput.text); 
                data.SetString(2, DateTime.Now.ToString()); 
                messageInput.text = string.Empty;

                GameSparksManager.Instance().GetRTSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data);
            }
        } else {
            Debug.Log("No Chat Message To Send...");
        }
    }

    private void ToogleChatWindow() {
        isChatWindowOpen = !isChatWindowOpen;
        if (isChatWindowOpen) {
            chatWindow.SetActive(true);
            chatToogleBttn.transform.GetComponentInChildren<Text>().text = "End Chat";
        } else {
            chatWindow.SetActive(false);
            chatToogleBttn.transform.GetComponentInChildren<Text>().text = "Start Chat";
        }
    }

    public void OnMessageReceived(RTPacket _packet) {
        Debug.Log("Message Received...\n" + _packet.Data.GetString(1)); 
                                                                     
        foreach (RTSessionInfo.RTPlayer player in GameSparksManager.Instance().GetSessionInfo().GetPlayerList()) {
            if (player.peerId == _packet.Sender) {
                UpdateChatLog(player.displayName, _packet.Data.GetString(1), _packet.Data.GetString(2));
            }
        }
    }

    private void UpdateChatLog(string _sender, string _message, string _date) {
        chatLog.Enqueue("<b>" + _sender + "</b>\n<color=black>" + _message + "</color>" + "\n<i>" + _date + "</i>");
        if (chatLog.Count > elementsInChatLog) {
            chatLog.Dequeue();
        }
        chatLogOutput.text = string.Empty;
        foreach (string logEntry in chatLog.ToArray()) {
            chatLogOutput.text += logEntry + "\n";
        }
    }

}


