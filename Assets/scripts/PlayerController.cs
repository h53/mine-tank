using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Direction
{
    up,
    right,
    down,
    left
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float moveSpeed = 2;
    public GameObject bullet;

    protected Direction nowDirection;

    private static int DIRECTION_ANGLE = 90;

    // Start is called before the first frame update
    void Start()
    {
        if(bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        instance = this;
        nowDirection = Direction.up;
    }

    // Update is called once per frame
    void Update()
    {
        MoveMent();
    }

    private void MoveMent()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))   //up
        {
            if (nowDirection != Direction.up) { TurnDirection(Direction.up); }
            MoveForward();
        }

        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftAlt))   //left
        {
            if (nowDirection != Direction.left) { TurnDirection(Direction.left); }
            MoveForward();
        }

        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) //down
        {
            if (nowDirection != Direction.down) { TurnDirection(Direction.down); }
            MoveForward();
        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))    //right
        {
            if (nowDirection != Direction.right) { TurnDirection(Direction.right); }
            MoveForward();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    protected void TurnDirection(Direction direction)
    {
        int angle = (direction - this.nowDirection) * DIRECTION_ANGLE;  // Rotate angle
        this.gameObject.transform.Rotate(Vector3.back * angle);
        this.nowDirection = direction;
    }

    protected void MoveForward()
    {
        this.gameObject.transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
    }

    protected void Fire()
    {
        Instantiate(bullet, this.gameObject.transform.position, this.gameObject.transform.rotation);
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