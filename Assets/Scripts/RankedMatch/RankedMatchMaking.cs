using UnityEngine;
using System.Collections;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class RankedMatchMaking : MonoBehaviour {
    
    public void findMatch() {
        new FindMatchRequest()
        .SetAction("cancel")
        //.SetMatchGroup(matchGroup)
        .SetMatchShortCode("rankedMatch")
        .SetSkill(2000)
        .Send((response) => {
            Debug.Log(response.MatchData);
            Debug.Log(response.Errors.JSON.ToString());
            string accessToken = response.AccessToken;
            string host = response.Host;
           /* GSData matchData = response.MatchData;
            string matchId = response.MatchId;
            GSEnumerable<var> opponents = response.Opponents;
            int? peerId = response.PeerId;
            string playerId = response.PlayerId;
            int? port = response.Port;
            GSData scriptData = response.ScriptData; */
        });
    }

}
