using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using TMPro;

public class UploadNewScore : MonoBehaviour
{
    [SerializeField] int currentPoints;
    [SerializeField] TMP_Text pointsText;

    public static UploadNewScore instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        pointsText.text = "Score: " + currentPoints.ToString();
    }

    public int GetPoints()
    {
        return currentPoints;
    }

    [Button]
    public void UploadScore(int highscore)
    {
        StartCoroutine(UploadScoreToDatabase(MainMenu.username, highscore));
    }

    IEnumerator UploadScoreToDatabase(string username, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", score);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~bosko.ivkovic/KernMod_4/UploadScore.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        currentPoints = 0;
    }
}
