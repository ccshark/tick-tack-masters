using UnityEngine;
using System.Collections;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.RT;

public class GameSparksManager : MonoBehaviour
{
    private static GameSparksManager instance = null;

    private GameSparksRTUnity gameSparksRTUnity;
    private RTSessionInfo sessionInfo;


    public static GameSparksManager Instance()
    {
        if (instance != null)
        {
            return instance; // return the singleton if the instance has been setup
        }
        else
        { // otherwise return an error
            Debug.LogError("GSM| GameSparksManager Not Initialized...");
        }
        return null;
    }
    void Awake()
    {
        instance = this; // if not, give it a reference to this class...
        DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
    }





#region Login & Registration
    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);

    public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
    {
        new GameSparks.Api.Requests.RegistrationRequest()
                  .SetDisplayName(_userName)
                  .SetUserName(_userName)
                  .SetPassword(_password)
                  .Send((regResp) => {
                      if (!regResp.HasErrors)
                      { // if we get the response back with no errors then the registration was successful
                    Debug.Log("GSM| Registration Successful...");
                          _regcallback(regResp);
                      }
                      else
                      {
                    
                    if (!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
                    {
                              Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                              new GameSparks.Api.Requests.AuthenticationRequest()
                                  .SetUserName(_userName)
                                  .SetPassword(_password)
                                  .Send((authResp) => {
                                      if (!authResp.HasErrors)
                                      {
                                          Debug.Log("Authentication Successful...");
                                          _authcallback(authResp);
                                      }
                                      else
                                      {
                                          Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                                      }
                                  });
                          }
                          else
                          {
                        
                        Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                          }
                      }
                  });
    }
    #endregion



    #region Matchmaking Request
    public void FindPlayers()
    {
        Debug.Log("GSM| Attempting Matchmaking...");
        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode("rankedMatch") // set the shortCode to be the same as the one we created in the first tutorial
            .SetSkill(2000) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
            .Send((response) => {
                if (response.HasErrors)
                { // check for errors
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }
    #endregion


    public GameSparksRTUnity GetRTSession()
    {
        return gameSparksRTUnity;
    }

    public RTSessionInfo GetSessionInfo()
    {
        return sessionInfo;
    }

    public void StartNewRTSession(RTSessionInfo _info)
    {
        Debug.Log("GSM| Creating New RT Session Instance...");
        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
        // In order to create a new RT game we need a 'FindMatchResponse' //
        // This would usually come from the server directly after a successful MatchmakingRequest //
        // However, in our case, we want the game to be created only when the first player decides using a button //
        // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
        // is passed in. //
        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)_info.GetPortID())
                                            .AddString("host", _info.GetHostURL())
                                            .AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details

        FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
        // So in the game-config method we pass in the response which gives the instance its connection settings //
        // In this example, I use a lambda expression to pass in actions for 
        // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
        // These methods are self-explanatory, but the important one is the OnPacket Method //
        // this gets called when a packet is received //

        gameSparksRTUnity.Configure(response,
            (peerId) => { OnPlayerConnectedToGame(peerId); },
            (peerId) => { OnPlayerDisconnected(peerId); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });
        gameSparksRTUnity.Connect(); // when the config is set, connect the game

    }

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");
            SceneManager.LoadScene("GameScene");
        }

    }

    private void OnPacketReceived(RTPacket _packet)
    {
    }


}


