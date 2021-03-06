using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 5;
    private Vector3 direction;
    [HideInInspector]
    public string desc = "";

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.up;
        StartCoroutine(WaitToDestroy(3f));
    }

    IEnumerator WaitToDestroy(float sec)
    {
        yield return new WaitForSeconds(sec);
        DisableBullet();
    }
    // Update is called once per frame
    void Update()
    {
        LetBulletFly(direction);
    }

    protected void LetBulletFly(Vector3 direction)
    {
        this.gameObject.transform.Translate(direction * bulletSpeed * Time.deltaTime);
    }

    private void DisableBullet()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bullet trigger");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("bullet Collider");
        DisableBullet();
    }
}
