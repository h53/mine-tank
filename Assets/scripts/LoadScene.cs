using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GlobalVars
{
    public static string serverip;
    public static int port;
    public static int onLineNum = 1;
}
public class LoadScene : MonoBehaviour
{
    public InputField serverIp;
    public InputField port;
    public void BeginBtn()
    {
        GlobalVars.serverip = serverIp.text.Length == 0 ? "127.0.0.1" : serverIp.text;
        GlobalVars.port = port.text.Length == 0 ? 1234 : int.Parse(port.text);
        Debug.LogWarning("serverip " + GlobalVars.serverip);
        Debug.LogWarning("port " + GlobalVars.port);
        SceneManager.LoadScene("mainScene");
    }
}
