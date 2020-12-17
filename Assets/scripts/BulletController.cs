using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 5;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        LetBulletFly(in direction);
    }

    protected void LetBulletFly(in Vector3 direction)
    {
        this.gameObject.transform.Translate(direction * bulletSpeed * Time.deltaTime);
    }

    private void DisableBullet()
    {
        Destroy(this.gameObject);
    }

    void OnBecameInvisible()
    {
        // run out of the screen
        DisableBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bullet trigger");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("bullet Collider");
    }
}
