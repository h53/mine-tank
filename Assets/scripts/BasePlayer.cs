using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Vector2 moveDirection;
    [HideInInspector]
    public string desc = ""; // net desc

    public Rigidbody2D rb;
    protected bool fireFlag;

    protected virtual void ProcessInputs()
    {
        if (moveDirection.y == 0)
        {
            moveDirection.x = Input.GetAxisRaw("Horizontal");
        }
        if (moveDirection.x == 0)
        {
            moveDirection.y = Input.GetAxisRaw("Vertical");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fireFlag = true;
        }
    }
    public virtual void Move()
    {
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            rb.MoveRotation(-(moveDirection.x + (moveDirection.y == 0 ? 0 : moveDirection.y - 1)) * NUM.DIRECTION_ANGLE); // rotate
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); // move
        }
    }

    protected void Fire()
    {
        if (fireFlag)
        {
            Instantiate(bullet, bulletPos.position, this.gameObject.transform.rotation);
            fireFlag = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("player trigger");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("player collider");
        if (collision.gameObject.tag == "enemybullet")
        {
            Debug.LogWarning("restart");
            Destroy(this.gameObject);
            SceneManager.LoadScene("mainScene"); //for temporary
        }
    }

}