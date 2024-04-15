using UnityEngine;
using CabbageNetwork;

public class GetCabbageLeaderboardAsyncRequest : AsyncRequest
{
    public GetCabbageLeaderboardAsyncRequest(string tableName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "ldjam55/leaderboard/getCabbageLeaderboard.php";

        this.form = new WWWForm();

        this.form.AddField("tableName", tableName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
