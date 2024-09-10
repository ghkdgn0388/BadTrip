using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float deleteTime = 5f;
    public LayerMask layerType;
    [HideInInspector] public Vector2 bulletSize;

    private void Start()
    {
        StartCoroutine(DeleteBullet(deleteTime));
    }
    private void Update()
    {
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
        BulletHit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((layerType.value & (1 << collision.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DeleteBullet(float deleteTime) {
        yield return new WaitForSeconds(deleteTime);
        Destroy(gameObject);
    }


    void BulletHit()
    {
        Vector2 point = (Vector2)(transform.position);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, bulletSize, transform.rotation.z);
        /*foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Debug.Log("Hit");
            }
        }*/
    }
}
