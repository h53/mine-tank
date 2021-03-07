using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GlobalVars
{
    public static string serverip = "127.0.0.1";
    public static int port = 1234;
    public static int onLineNum = 1;
}
public class LoadScene : MonoBehaviour
{
    public InputField serverIp;
    public InputField port;

    private void Start()
    {
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        if(commandLineArgs.Length > 2)
        {
            GlobalVars.serverip = commandLineArgs[1];
            GlobalVars.port = int.Parse(commandLineArgs[2]);
            Debug.LogWarning("bench mode on");
            SceneManager.LoadScene("mainScene");
        }
        
    }
    public void BeginBtn()
    {
        GlobalVars.serverip = serverIp.text.Length == 0 ? "127.0.0.1" : serverIp.text;
        GlobalVars.port = port.text.Length == 0 ? 1234 : int.Parse(port.text);
        Debug.LogWarning("serverip " + GlobalVars.serverip);
        Debug.LogWarning("port " + GlobalVars.port);
        SceneManager.LoadScene("mainScene");
    }
}
