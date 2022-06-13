using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour
{
    public UserOnBoard[] users;

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
            //phrase.Remove(0, 1);
            string[] words = phrase.Split('@');
            Debug.Log(words[0]);
            SetupBoard(words);
        }
    }

    void SetupBoard(string[] info)
    {
        //int temp = 0;
        //foreach (var user in users)
        //{
        //    user.playerName.text = info[temp];
        //    user.playerScore.text = info[temp + 1];
        //    temp += 2;
        //}
    }

}

class LeaderboardOutcome
{

}
