using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [HideInInspector]
    public bool isOver;
    public GameObject GameOverCanvas;
    public GameObject HudCanvas;
    public Text onLineText;
    public Text tipText;
    public InputField input;
    public string inputStr;
    public static bool isTyping;
    private static IEnumerator coroutine;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        isOver = false;
        isTyping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver) {
            GameOver();
        }

        SendText();

        ShowOnlineNum();
    }

    IEnumerator WaitforInput(float sec)
    {
        input.ActivateInputField();
        yield return new WaitForSeconds(sec);
        while (input.isFocused)
        {
            yield return new WaitForSeconds(sec);
        }
        inputStr = input.text;
        input.gameObject.SetActive(false);
    }

    /// <summary>
    /// click Enter to active inputfield
    /// click twice to disable it
    /// </summary>
    private void SendText()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!input.IsActive())
            {
                input.gameObject.SetActive(true);
                coroutine = WaitforInput(2f);
                StartCoroutine(coroutine);
            }
            else
            {
                // send message
                StopCoroutine(coroutine);
                inputStr = input.text;
                if (!inputStr.Equals(""))
                {
                    NetManager.Send("Text|" +
                        PlayerController.instance.desc + "," +
                        inputStr + ","
                        );
                }
                input.text = "";
                input.gameObject.SetActive(false);
            }
        }

        if (input.IsActive()) { isTyping = input.isFocused; }
        else isTyping = false;
    }

    private void ShowOnlineNum()
    {
        onLineText.text = GlobalVars.onLineNum.ToString();
    }

    public void StartButton()
    {
        ResetVars();
        SceneManager.LoadScene("mainScene");
    }

    private void ResetVars()
    {
        GlobalVars.onLineNum = 1;
    }

    private void GameOver()
    {
        GameOverCanvas.SetActive(true);
        HudCanvas.SetActive(false);
        isOver = false;
    }

    public void ShowTip(string tip,float sec)
    {
        tipText.text = tip;
        tipText.gameObject.SetActive(true);
        StartCoroutine(Waitfor(sec, () =>  tipText.gameObject.SetActive(false)));
    }

    IEnumerator Waitfor(float sec, System.Action action)
    {
        yield return new WaitForSeconds(sec);
        action();
    }
}
