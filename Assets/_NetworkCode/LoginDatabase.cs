using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LoginDatabase : MonoBehaviour
{
    [Header("Pages")]
    public TMP_Text username;
    public GameObject registerPage;
    public GameObject loginPage;
    public TMP_Text infoText;

    [Header("Login")]
    public TMP_InputField loginName;
    public TMP_InputField loginPassword;
    public UserOnBoard yourScore;

    [Header("Register")]
    public TMP_InputField registerName;
    public TMP_InputField registerPassword;
    [Space]
    public UnityEvent loginDone = new UnityEvent();
    public UnityEvent registrationDone = new UnityEvent();


    #region registration
    public void Button_Register()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", registerName.text);
        form.AddField("password", registerPassword.text);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~bosko.ivkovic/KernMod_4/Register.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            infoText.text = www.error;
        }
        else
        {
            if (www.downloadHandler.text[0] == '1')
            {
                string phrase = www.downloadHandler.text;
                string sessionID = StringHandler.Between(phrase, "(", ")");
                phrase = phrase.Replace(sessionID, "");
                phrase = phrase.Replace("<br>", "");

                infoText.text = phrase;

                if (phrase == "11")
                {
                    registrationDone.Invoke();
                    infoText.text = "Completed Registration";
                }
            }
            else
            {
                infoText.text = www.downloadHandler.text;
            }
        }
    }
    #endregion


    #region Login

    public void Button_Login()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", loginName.text);
        form.AddField("password", loginPassword.text);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~bosko.ivkovic/KernMod_4/Login.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            infoText.text = www.error;
        }
        else if (www.downloadHandler.text[0] == '1')
        {
            Debug.Log(www.downloadHandler.text);
            string phrase = www.downloadHandler.text;
            string sessionID = StringHandler.Between(phrase, "(", ")");
            phrase = phrase.Replace(sessionID, "");
            phrase = phrase.Replace("<br>", "");

            infoText.text = phrase;

            if (phrase[1] == '1')
            {
                infoText.text = $"Logged in as {loginName.text}!";
                username.text = loginName.text;

                string playerscore = phrase.Remove(0, 2);

                yourScore.playerName.text = username.text;
                playerscore = playerscore.Replace((username.text + " Highscore:"), "");
                playerscore = playerscore.Replace("hello", "");
                yourScore.playerScore.text = playerscore;

                MainMenu.username = username.text;
                loginDone.Invoke();
            }
        }
        else
        {
            infoText.text = www.downloadHandler.text;
        }
    }

    #endregion
}
