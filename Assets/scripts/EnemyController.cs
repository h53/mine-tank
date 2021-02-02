using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : BasePlayer
{
    public Vector2 position;
    void Awake()
    {
        if (bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        rb = GetComponent<Rigidbody2D>();
        moveDirection = new Vector2(0, 0);
    }
    private void Start()
    {
        fireFlag = false;
        position = rb.position;
    }
    void Update()
    {
        //ProcessInputs();
        Move();
        Fire();
    }

    void FixedUpdate()
    {
        //Move();
        //Fire();
    }
    protected override void ProcessInputs(bool disable)
    {
        //base.ProcessInputs();
    }

    protected override void Move()
    {
        if (rb.position != position)
        {
            rb.MoveRotation(-(moveDirection.x + (moveDirection.y == 0 ? 0 : moveDirection.y - 1)) * NUM.DIRECTION_ANGLE); // rotate
            rb.MovePosition(Vector2.Lerp(rb.position,position,0.675f)); // move
        }
    }

    protected override void Fire()
    {
        if (fireFlag)
        {
            Instantiate(bullet, bulletPos.position, this.gameObject.transform.rotation).GetComponent<BulletController>().desc = this.desc;
            fireFlag = false;
        }
    }
}
