using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour
{
    public UserOnBoard[] users;
    [SerializeField] List<LeaderboardOutcome> outcomes = new List<LeaderboardOutcome>();

    void Start()
    {
        UpdateBoard();
    }

    public void UpdateBoard()
    {
        foreach (var user in users)
        {
            user.playerName.text = "Loading...";
            user.playerScore.text = "0";
        }
        StartCoroutine(UpdateScoreBoard());
    }

    IEnumerator UpdateScoreBoard()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", "");
        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~bosko.ivkovic/KernMod_4/Leaderboard.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string phrase = www.downloadHandler.text.ToString();
            phrase = phrase.Replace("<br>", "");

            for (int i = 0; i < 10; i++)
            {
                string value = Between(phrase, "{", "}");
                outcomes.Add(JsonUtility.FromJson<LeaderboardOutcome>(value));
                phrase = phrase.Replace(value, "");
            }

            SetupBoard();
        }
    }

    void SetupBoard()
    {
        for (int i = 0; i < 10; i++)
        {
            users[i].playerName.text = outcomes[i].Playername;
            users[i].playerScore.text = outcomes[i].Score.ToString();
        }
    }

    public string Between(string STR, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString);
        FinalString = STR.Substring(Pos1 - 1, Pos2 - Pos1 +2);
        return FinalString;
    }


}

[System.Serializable]
class LeaderboardOutcome
{
    public string Playername;
    public int Score;
    public DateTime LastPlayed;
}
