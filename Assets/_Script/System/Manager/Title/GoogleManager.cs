using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class GoogleManager : MonoBehaviour
{
    public Text Userid;
    public bool LoginSuccess = false;

    [Header("TEST")]
    public InputField IFText;
    public Text userIdText;
    public Button LoginBtn;
    string path;
    Regex idPattern =new Regex(@"^[a-zA-Z]+[0-9]*$");

     string id;
     string pw;

    private void Awake()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            LoginBtn.interactable = false;
            IFText.interactable = false;
            LoginBtn.transform.GetChild(0).GetComponent<Text>().text = "인터넷 연결 실패";
        }
        else
        {
           PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();


            path = Application.persistentDataPath + "/data.txt";
            //LogIn();

            //  id = "g12422071366773018695@cell.com";
            pw = "cjfals12";
            User.FabId = "";

            //testLogin();
            LoadJson();
        }
       
    }

    public void SaveJson()
    {
        if (File.Exists(path)) File.Delete(path);
        File.WriteAllText(path, userIdText.text);
    }

    public void LoadJson()
    {
        if (!File.Exists(path)) return;

        IFText.interactable = false;
        IFText.text = File.ReadAllText(path);
    }

    public void testLogin()
    {       
        if (!idPattern.IsMatch(IFText.text))
        {
            LoginBtn.transform.GetChild(0).GetComponent<Text>().text = "재입력 하세요";
            return;
        }

        LoginBtn.interactable = false;
        IFText.interactable = false;

        var request = new LoginWithEmailAddressRequest { Email = IFText.text + "@test.com" , Password = pw };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => 
        {
            LoginSuccess = true;
            LoginBtn.transform.GetChild(0).GetComponent<Text>().text = "로그인 성공";
            User.FabId = result.PlayFabId;
            SaveJson();
            DotweenAni.instance.killme();
            SceneManager.LoadSceneAsync("Main");
        }, (error) => testPlayFabRegister());
    }

    public void testPlayFabRegister()
    {
        var request = new RegisterPlayFabUserRequest { Email = IFText.text + "@test.com", Password = pw, Username = IFText.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (resule) => {
            LoginBtn.transform.GetChild(0).GetComponent<Text>().text = "회원가입 성공";
            testLogin();
        }, (error) =>
        {
            Debug.Log(error);
            LoginBtn.transform.GetChild(0).GetComponent<Text>().text = "아이디 교체";
            LoginBtn.interactable = true;
            IFText.interactable = true;
        });
    }

    public void LogIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Userid.text = Social.localUser.userName;

                PlayFabLogin();
            }
            else
            {
                Userid.text = "Login Faild";
                LoginSuccess = false;
            }
        });
    }

    public void PlayFabLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = Social.localUser.id + "@cell.com", Password = Social.localUser.id };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => LoginSuccess = true, (error) => PlayFabRegister());

    }

    public void PlayFabRegister()
    {
        var request = new RegisterPlayFabUserRequest { Email = Social.localUser.id + "@cell.com", Password = Social.localUser.id, Username = RandomUserName() };
        PlayFabClientAPI.RegisterPlayFabUser(request, (resule) => { Debug.Log("success Register"); PlayFabLogin(); }, (error) => Debug.Log("Fail Register"));
    }

    string RandomUserName()
    {
        string strPool = "abcdefghijklamopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //문자 생성 풀
        char[] chRandom = new char[15];
        System.Random random = new System.Random();

        for (int i = 0; i < 15; i++)
        {
            chRandom[i] = strPool[random.Next(strPool.Length)];
        }
        string strRet = new String(chRandom);  // char to string

        return strRet;
    }

}
