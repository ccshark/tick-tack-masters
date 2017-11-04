using UnityEngine;
using GameSparks.Api.Responses;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.RT;

public class GameSparksManager : MonoBehaviour {
    private static GameSparksManager instance = null;

    private GameSparksRTUnity gameSparksRTUnity;
    private RTSessionInfo sessionInfo;
    private ChatManager chatManager;


    public static GameSparksManager Instance() {
        if (instance != null) {
            return instance;
        } else {
            Debug.LogError("GSM| GameSparksManager Not Initialized...");
        }
        return null;
    }
    void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }



#region Login & Registration
    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);

    public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback) {
        new GameSparks.Api.Requests.RegistrationRequest()
          .SetDisplayName(_userName)
          .SetUserName(_userName)
          .SetPassword(_password)
          .Send((regResp) => {
              if (!regResp.HasErrors) {
                  Debug.Log("GSM| Registration Successful...");
                  _regcallback(regResp);
              } else {
                if (!(bool)regResp.NewPlayer) {
                      Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                      new GameSparks.Api.Requests.AuthenticationRequest()
                          .SetUserName(_userName)
                          .SetPassword(_password)
                          .Send((authResp) => {
                              if (!authResp.HasErrors) {
                                  Debug.Log("Authentication Successful...");
                                  _authcallback(authResp);
                              } else {
                                  Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                              }
                          });
                } else {
                    Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                }
           }
      });
    }
    #endregion


    #region Matchmaking Request
    public void FindPlayers() {
        Debug.Log("GSM| Attempting Matchmaking...");
        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode("rankedMatch") 
            .SetSkill(2000)
            .Send((response) => {
                if (response.HasErrors) {
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }
    #endregion


    public GameSparksRTUnity GetRTSession() {
        return gameSparksRTUnity;
    }

    public RTSessionInfo GetSessionInfo() {
        return sessionInfo;
    }

    public void StartNewRTSession(RTSessionInfo _info) {
        Debug.Log("GSM| Creating New RT Session Instance...");
        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>();

        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)_info.GetPortID())
                                            .AddString("host", _info.GetHostURL())
                                            .AddString("accessToken", _info.GetAccessToken());

        FindMatchResponse response = new FindMatchResponse(mockedResponse);

        gameSparksRTUnity.Configure(response,
            (peerId) => { OnPlayerConnectedToGame(peerId); },
            (peerId) => { OnPlayerDisconnected(peerId); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });
        gameSparksRTUnity.Connect(); 

    }

    private void OnPlayerConnectedToGame(int _peerId) {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId) {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady) {
        if (_isReady) {
            Debug.Log("GSM| RT Session Connected...");
            SceneManager.LoadScene("GameScene");
        }

    }


    private void OnPacketReceived(RTPacket _packet) {
        switch (_packet.OpCode) {
            case 1:
                if (chatManager == null) {
                    chatManager = GameObject.Find("Chat Manager").GetComponent<ChatManager>();
                }
                chatManager.OnMessageReceived(_packet); 
                break;
        }
    }
}


