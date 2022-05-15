using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LoginDatabase : MonoBehaviour
{
    [Header("Pages")]
    public GameObject registerPage;
    public GameObject loginPage;
    public TMP_Text infoText;

    [Header("Register")]
    public TMP_InputField registerName;
    public TMP_InputField registerPassword;

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
            Debug.Log(www.error);
        }
        else
        {
            if (www.downloadHandler.text != "0")
            {
                Debug.Log($"{www.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Completed Registration");
            }
        }
    }
}
