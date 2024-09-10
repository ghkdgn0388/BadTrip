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

                    // ������ �ӵ��� �ٿ��� ���Ⱑ ���ߵ��� ����
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    // this.enabled = false;
                }
            }
        }  
    }
}
