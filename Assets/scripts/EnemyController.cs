using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyController : BasePlayer
{
    public static EnemyController instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        rb = GetComponent<Rigidbody2D>();
        moveDirection = new Vector2(0, 0);
        fireFlag = false;
    }
    protected override void ProcessInputs()
    {
        //base.ProcessInputs();
    }

    public override void Move()
    {
        rb.MoveRotation(-(moveDirection.x + (moveDirection.y == 0 ? 0 : moveDirection.y - 1)) * NUM.DIRECTION_ANGLE); // rotate
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); // move
    }
    private void AutoMove()
    {
        //TO DO
    }
}
