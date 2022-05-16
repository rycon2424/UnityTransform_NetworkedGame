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
    public GameObject registerPage;
    public GameObject loginPage;
    public TMP_Text infoText;

    [Header("Login")]
    public TMP_InputField loginName;
    public TMP_InputField loginPassword;

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
            if (www.downloadHandler.text != "0")
            {
                infoText.text = www.downloadHandler.text;
            }
            else
            {
                registrationDone.Invoke();
                infoText.text = "Completed Registration";
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
        else if (www.downloadHandler.text[0] == '0')
        {
            infoText.text = $"Logged in as {loginName.text}!";
            //Debug.Log($"Login succes, high score = {www.downloadHandler.text[1]}");
            loginDone.Invoke();
        }
        else
        {
            infoText.text = www.downloadHandler.text;
        }
    }

    #endregion
}
