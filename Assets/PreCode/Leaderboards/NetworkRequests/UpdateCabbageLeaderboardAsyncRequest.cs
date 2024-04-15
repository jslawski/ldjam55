using UnityEngine;
using CabbageNetwork;

public class UpdateCabbageLeaderboardAsyncRequest : AsyncRequest
{
    public UpdateCabbageLeaderboardAsyncRequest(string username, string value, string tableName, NetworkRequestSuccess successCallback = null, NetworkRequestFailure failureCallback = null)
    {
        string url = ServerSecrets.ServerName + "ldjam55/leaderboard/updateCabbageLeaderboard.php";

        this.form = new WWWForm();
        this.form.AddField("username", username);
        this.form.AddField("score", value);
        this.form.AddField("tableName", tableName);

        this.SetupRequest(url, successCallback, failureCallback);
    }
}
