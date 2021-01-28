using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed = 2;
    public GameObject bullet;
    public Transform bulletPos;

    private static int DIRECTION_ANGLE = 90;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool fireFlag;
    void Start()
    {
        if(bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveDirection = new Vector2(0, 0);
        fireFlag = false;
    }

    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        Move();
        Fire();
    }

    private void ProcessInputs()
    {
        if(moveDirection.y == 0)
        {
            moveDirection.x = Input.GetAxisRaw("Horizontal");
        }
        if(moveDirection.x == 0)
        {
            moveDirection.y = Input.GetAxisRaw("Vertical");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fireFlag = true;
        }
    }

    private void Move()
    {
        if(moveDirection.x != 0 || moveDirection.y != 0) // rotate
        {
            rb.MoveRotation(-(moveDirection.x + (moveDirection.y == 0? 0 : moveDirection.y - 1)) * DIRECTION_ANGLE);
        }
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); // move
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