using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System.Linq;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using System.Text;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviour
{

    public Text userId, connectionStatus;
    public InputField userNameInput, passwordInput;
    public GameObject loginPanel, playerListPanel;

    public Button loginBttn, matchmakingBttn, startGameBttn;

    public Text matchDetails, playerList;
    public GameObject matchDetailsPanel;

    public RTSessionInfo tempRTSessionInfo;


    void Start()
    {
        userId.text = "No User Logged In...";
        connectionStatus.text = "No Connection...";

        GS.GameSparksAvailable += (isAvailable) => {
            if (isAvailable)
            {
                connectionStatus.text = "GameSparks Connected...";
            }
            else
            {
                connectionStatus.text = "GameSparks Disconnected...";
            }
        };

        playerListPanel.SetActive(false);
        matchmakingBttn.gameObject.SetActive(false);
        startGameBttn.gameObject.SetActive(false);

        loginBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().AuthenticateUser(userNameInput.text, passwordInput.text, OnRegistration, OnAuthentication);
        });

        matchmakingBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers();
            playerList.text = "Searching For Players...";
        });

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) => {
            playerList.text = "No Match Found...";
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        startGameBttn.onClick.AddListener(() => {
            GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        });
    }

    private void OnRegistration(RegistrationResponse _resp)
    {
        userId.text = "User ID: " + _resp.UserId;
        connectionStatus.text = "New User Registered...";
        loginPanel.SetActive(false);
        loginBttn.gameObject.SetActive(false);
        matchmakingBttn.gameObject.SetActive(true);
        playerListPanel.SetActive(true);
    }
  
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        userId.text = "User ID: " + _resp.UserId;
        connectionStatus.text = "User Authenticated...";
        loginPanel.SetActive(false);
        loginBttn.gameObject.SetActive(false);
        matchmakingBttn.gameObject.SetActive(true);
        playerListPanel.SetActive(true);
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        Debug.Log("Match Found!...");
        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Host URL:" + _message.Host);
        sBuilder.AppendLine("Port:" + _message.Port);
        sBuilder.AppendLine("Access Token:" + _message.AccessToken);
        sBuilder.AppendLine("MatchId:" + _message.MatchId);
        sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }
        playerList.text = sBuilder.ToString(); // set the string to be the player-list field

        tempRTSessionInfo = new RTSessionInfo(_message); // we'll store the match data until we need to create an RT session instance
        matchmakingBttn.gameObject.SetActive(false);
        startGameBttn.gameObject.SetActive(true);


    }
}




