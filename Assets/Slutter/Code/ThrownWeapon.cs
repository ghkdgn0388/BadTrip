using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rb != null && rb.velocity != Vector2.zero) 
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null && !enemy.isKnockBack)
                {
                    enemy.KnockBack();

                    // 무기의 속도를 줄여서 무기가 멈추도록 설정
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    // this.enabled = false;
                }
            }
        }  
    }
}
