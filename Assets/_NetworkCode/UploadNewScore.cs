using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;

public class UploadNewScore : MonoBehaviour
{
    [Button]
    void UploadScore(string username, int score)
    {
        StartCoroutine(UploadScoreToDatabase(username, score));
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
        else
        {
            if (www.downloadHandler.text != "0")
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }
}
