using System.Collections;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Animator animator;
    public LayerMask interactionLayer;
    public bool isMelee = true;
    [HideInInspector]public Vector2 meleeRange;
    public Vector2 punchRange;
    public float punchRate;
    private bool isFireAble = true;
    public float fireRate;
    public bool isDead = false;
    public PickupWeapon weapon;
    public GameObject currentWeapon;
    public Enemy Enemy;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    public AudioClip deathSFX;
    private AudioSource audioSource;
    public float sfxVolume = 1.0f;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = sfxVolume;
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (Input.GetMouseButton(0) && (weapon != null && weapon.currentBulletCount != 0 || isMelee))
        {
            animator.SetBool("isFire", true);
        }
        else
        {
            animator.SetBool("isFire", false);
        }
        if (currentWeapon != null)
        {
            animator.SetBool("isHold", true);
        }
        // 무기 줍기
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractNearestObject();
        }

        // 무기 사용
        if (isFireAble)
        {
            if (Input.GetMouseButton(0))
            {
                UseWeapon();                
            }
            if (Input.GetMouseButtonUp(1) && currentWeapon != null)
            {
                ThrowWeapon();
            }
        }
        
    }

    void InteractNearestObject()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, interactionLayer);
        Debug.Log("list: " + colliders.Length);

        Collider2D nearestObject = null;
        float minDistance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = collider;
            }
        }
        Debug.Log(nearestObject);
        if (nearestObject != null)
        {
            nearestObject.GetComponent<Interaction>().action();
        } else{
            Debug.Log("None");
        }
    }

    void UseWeapon()
    {
        if (currentWeapon != null)
        {
            StartCoroutine(FireDeley(fireRate));
            if (!isMelee)
            {
                FireBullet();
            }
            else
            {
                SwingMeleeWeapon();
            }
        }
        else
        {
            Punch();
            StartCoroutine(FireDeley(punchRate));
        }
    }

    IEnumerator FireDeley(float deleyTime)
    {
        isFireAble = false;
        yield return new WaitForSeconds(deleyTime);
        isFireAble = true;   
    }

    void FireBullet()
    {
        if(weapon.currentBulletCount > 0)
        {
            float recoil = Random.Range(-weapon.recoil, weapon.recoil);
            Quaternion rotation = Quaternion.Euler(0,0,firePoint.eulerAngles.z + recoil);
            Instantiate(bulletPrefab, firePoint.position, rotation);
            weapon.currentBulletCount--;

            PlaySFX(weapon.fireSFX);
        }
        else if(weapon.currentBulletCount <= 0) 
        {
            Debug.Log("Out of Bullet");
            return;           
        }
        
    }

    void SwingMeleeWeapon()
    {
        Vector2 point = (Vector2)(transform.position) + transform.right * meleeRange / 2;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, meleeRange, transform.rotation.z);
        Debug.Log(colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Dead();
                }
            }
        }
    }

    void Punch()
    {
        meleeRange = punchRange;
        Vector2 point = (Vector2)(transform.position) + transform.right * meleeRange / 2;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, meleeRange, transform.rotation.z);
        Debug.Log(colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && !enemy.isKnockBack)
                {
                    enemy.KnockBack();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            Dead();
        }
    }

    public void Dead()
    {
        animator.SetBool("isDead", true);
        isDead = true;
        if (col != null)
        {
            Debug.Log("AAA");
            col.enabled = false;
        }

        if (audioSource != null && deathSFX != null)
        {
            audioSource.PlayOneShot(deathSFX);
        }

        GameManager.Instance.GameOver();
    }

    void ThrowWeapon()
    {
        animator.SetBool("isHold", false);

        currentWeapon.transform.position = transform.position;
        currentWeapon.SetActive(true);
        Rigidbody2D weaponRb = currentWeapon.GetComponent<Rigidbody2D>();
        weaponRb.AddForce(transform.right * 25f, ForceMode2D.Impulse); // 무기를 던지기
        weaponRb.AddTorque(10f, ForceMode2D.Impulse); // 무기 회전

        currentWeapon.GetComponentInChildren<ThrownWeapon>().enabled = true;

        currentWeapon = null;
        isMelee = true;
        meleeRange = punchRange;
    }


    public void DropWeapon()
    {
        animator.SetBool("isHold", false);

        currentWeapon.transform.position = transform.position;
        currentWeapon.SetActive(true);
        currentWeapon.GetComponent<Rigidbody2D>().AddForce(transform.right * 5f, ForceMode2D.Impulse);// 무기를 던지기
        currentWeapon.GetComponent<Rigidbody2D>().AddTorque(10f, ForceMode2D.Impulse);// 무기 회전

        currentWeapon.GetComponentInChildren<ThrownWeapon>().enabled = false;
        currentWeapon = null;
        isMelee = true;
        meleeRange = punchRange;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.volume = sfxVolume; // 볼륨 적용
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        if (isMelee)
        {
            Gizmos.color = Color.red;

            // 공격 범위의 중심점 계산
            float angle = transform.rotation.z * Mathf.Deg2Rad;
            float x = meleeRange.x / 2 * Mathf.Cos(angle);
            float y = meleeRange.y / 2 * Mathf.Sin(angle);

            Vector2 point = new Vector2(x, y);
            // 기즈모 매트릭스 설정
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.localRotation, Vector3.one);
            Gizmos.matrix = matrix;

            // 기즈모로 상자 그리기
            Gizmos.DrawWireCube(point, meleeRange);
        }
    }
}
