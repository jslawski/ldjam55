using UnityEngine;
using CabbageNetwork;

public class GetCabbageLeaderboardEntryAsyncRequest : AsyncRequest
{
    public GetCabbageLeaderboardEntryAsyncRequest(string chatterName, string tableName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "ldjam55/leaderboard/getCabbageLeaderboardEntry.php";

        this.form = new WWWForm();
        this.form.AddField("username", chatterName);
        this.form.AddField("tableName", tableName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
