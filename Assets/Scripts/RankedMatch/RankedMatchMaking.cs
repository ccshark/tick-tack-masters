using UnityEngine;
using System.Collections;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class RankedMatchMaking : MonoBehaviour {
    

    //GSRequestData parsedJson = new GSRequestData(json)
    public void findMatch() {
        new MatchmakingRequest()
       // .SetAction("cancel")
       // .SetCustomQuery(customQuery)
       // .SetMatchData(matchData)
        //.SetMatchGroup(matchGroup)
        .SetMatchShortCode("rankedMatch")
            //.SetParticipantData({"data": {"name" : "frans"}})
        .SetSkill(2000)
        .Send((response) => {
            Debug.Log("done");

        });

    }
}
