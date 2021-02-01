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
    public GameObject GameOver;
    public Text onLineText;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        isOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver) { 
            GameOver.SetActive(true);
            isOver = false;
        }

        onLineText.text = GlobalVars.onLineNum.ToString();
    }

    public void StartButton()
    {
        SceneManager.LoadScene("mainScene");
    }
}
