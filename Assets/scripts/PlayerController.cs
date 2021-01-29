using UnityEngine;

public class PlayerController : BasePlayer
{
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveDirection = new Vector2(0, 0);
    }
    void Start()
    {
        if (bullet == null) { Debug.LogError("bullet gameobject is null!"); }
        fireFlag = false;
    }
    private void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        Move();
        Fire();
    }
}