using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : PlayerController
{
    private void AutoMove()
    {
        //TO DO
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enemy trigger");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("enemy Collider");

        if (collision.gameObject.tag == "playerbullet")
        {
            Debug.LogWarning("restart");
            Destroy(this.gameObject);
            SceneManager.LoadScene("mainScene"); //for temporary
        }
    }
}
