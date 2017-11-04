using System.Collections.Generic;
using GameSparks.Api.Messages;
using UnityEngine;


public class RTSessionInfo : MonoBehaviour {
    private string hostURL;
    public string GetHostURL() { return this.hostURL; }
    private string acccessToken;
    public string GetAccessToken() { return this.acccessToken; }
    private int portID;
    public int GetPortID() { return this.portID; }
    private string matchID;
    public string GetMatchID() { return this.matchID; }

    private List<RTPlayer> playerList = new List<RTPlayer>();


    public List<RTPlayer> GetPlayerList() {
        return playerList;
    }

    public RTSessionInfo(MatchFoundMessage _message) {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        acccessToken = _message.AccessToken;
        matchID = _message.MatchId;
        foreach (MatchFoundMessage._Participant p in _message.Participants) {
            playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
        }
    }

    public class RTPlayer {
        public RTPlayer(string _displayName, string _id, int _peerId) {
            this.displayName = _displayName;
            this.id = _id;
            this.peerId = _peerId;
        }

        public string displayName;
        public string id;
        public int peerId;
        public bool isOnline;
    }
}


