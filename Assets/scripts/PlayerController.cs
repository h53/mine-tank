using UnityEngine;

public class PlayerController : BasePlayer
{
    public static PlayerController instance;
    void Awake()
    {
        instance = this;
        if (bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        moveDirection = new Vector2(0, 0);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
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
    protected override void ProcessInputs()
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
    protected override void Move()
    {
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            rb.MoveRotation(-(moveDirection.x + (moveDirection.y == 0 ? 0 : moveDirection.y - 1)) * NUM.DIRECTION_ANGLE); // rotate
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); // move
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