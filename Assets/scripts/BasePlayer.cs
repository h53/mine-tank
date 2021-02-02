using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NUM
{
    public static int DIRECTION_ANGLE = 90;
}

[RequireComponent(typeof(Rigidbody2D))]
public class BasePlayer : MonoBehaviour
{
    public float moveSpeed = 2;
    public GameObject bullet;
    public Transform bulletPos;
    [HideInInspector]
    public Vector2 moveDirection;
    [HideInInspector]
    public string desc = ""; // net desc
    [HideInInspector]
    public string hitdesc = "";
    [HideInInspector]
    public Rigidbody2D rb;
    public bool fireFlag;

    protected virtual void ProcessInputs(bool disable)
    {
        
    }
    protected virtual void Move()
    {

    }

    protected virtual void Fire()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("player trigger");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("player collider");

        //only detact self being hit
        this.hitdesc = collision.gameObject.GetComponent<BulletController>().desc;
    }

}