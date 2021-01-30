using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [HideInInspector]
    public bool isOver;
    public GameObject GameOver;
    // Start is called before the first frame update

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
    }

    public void StartButton()
    {
        SceneManager.LoadScene("mainScene");
    }
}
