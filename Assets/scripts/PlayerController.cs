using UnityEngine;

public class PlayerController : BasePlayer
{
    public static PlayerController instance;
    public Camera hudCamera;
    private Vector3 hudCameraPos;
    void Awake()
    {
        instance = this;
        if (bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        if (hudCamera == null) { Debug.LogError("hudCamera is null!"); }
        moveDirection = new Vector2(0, 0);
        rb = GetComponent<Rigidbody2D>();
        hudCameraPos = hudCamera.transform.position;
    }

    private void Start()
    {
        fireFlag = false;
    }

    void Update()
    {
        ProcessInputs(GameController.isTyping);
        HudCameraFollow();
    }

    void FixedUpdate()
    {
        Move();
        Fire();
    }

    private void HudCameraFollow()
    {
        hudCameraPos.x = this.gameObject.transform.position.x;
        hudCameraPos.y = this.gameObject.transform.position.y;
        hudCamera.transform.position = hudCameraPos;
    }
    protected override void ProcessInputs(bool disable)
    {
        if (disable) return;
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

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (hudCamera.orthographicSize <= 20)
                hudCamera.orthographicSize += 0.5F;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (hudCamera.orthographicSize > 0.3)
                hudCamera.orthographicSize -= 0.5F;
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