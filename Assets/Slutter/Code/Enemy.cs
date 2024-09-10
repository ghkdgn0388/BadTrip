using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    public float detectionRange = 10f;
    public float chaseSpeed = 3f;
    public float separationSpeed = 2f;
    public float stopDistance = 2f;
    public float attackDistance = 0.5f;
    public float minAttackDelay = 0.2f;
    public float maxAttackDelay = 3f;
    public float rotationSpeed = 5f;
    public float separationDistance = 1.5f;
    public float retreatDistance = 1f;

    public GameObject dropPrefab;  // 프리팹 변수 추가
    public float dropForce = 2f;   // 밀려나는 힘의 크기

    private Transform player;
    private Vector2 lastPlayerPostion;
    private bool findPlayer = false;

    public bool isDead = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    public bool isKnockBack = false;

    public bool isMelee = true;
    private bool isFireAble = true;
    private float fireRate;
    private bool isReload = false;
    private float recoil;
    private int maxBulletCount;
    [SerializeField] private int currentBulletCount;
    public PickupWeapon weapon;
    public GameObject bulletPrefab;
    public Transform firePoint;
    [HideInInspector] public Vector2 meleeRange;

    private static List<Enemy> allEnemies = new List<Enemy>();

    private PathFinding path;
    private Vector2 firstNode;

    private Animator anim;
    public LayerMask blockLayer;

    private Rigidbody2D rigid;
    private Collider2D col;

    public AudioClip deathSFX;
    private AudioSource audioSource;
    public float sfxVolume = 1.0f;

    private void Awake()
    {
        path = GetComponent<PathFinding>();
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        allEnemies.Add(this);
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = sfxVolume;
        SetWeapon();
    }

    private void OnDestroy()
    {
        allEnemies.Remove(this);
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (isDead || isKnockBack)
        {
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, Vector2.Distance(transform.position, player.position), blockLayer);
        if (hit.collider == null) {
            lastPlayerPostion = player.position;
            findPlayer = true;
        }
        if (findPlayer && Vector2.Distance(transform.position, lastPlayerPostion) >= 0.001f) {
            isChasing = true;
        }
        else{
            isChasing = false;
            findPlayer = false;
        }
        if (dropPrefab != null && !isMelee && isReload)
        {
            StartCoroutine(Reload(3f));
        }
        if (isChasing)
        {
            LookAtPlayer();
            CheckAttackAble();

            if (distanceToPlayer > stopDistance)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer <= retreatDistance)
            {
                //RetreatFromPlayer();
            }
            if (isAttacking && isFireAble)
            {
                UseWeapon();
            }
        }

        //MaintainDistanceFromOtherEnemies();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("Hit");
            Dead();
        }
    }

    public void Dead()
    {
        anim.SetBool("isDead", true);
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

        DropWeapon();

        GameManager.Instance.AddTime();
    }

    private void DropWeapon()
    {
        if (dropPrefab != null)
        {
            GameObject drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

            // dropPrefab이 Rigidbody2D를 가지고 있는지 확인
            Rigidbody2D dropRigid = drop.GetComponent<Rigidbody2D>();
            if (dropRigid != null)
            {
                // 플레이어 반대 방향으로 힘을 가함
                Vector2 forceDirection = (drop.transform.position - player.position).normalized;
                dropRigid.AddForce(forceDirection * dropForce, ForceMode2D.Impulse);
            }
            dropPrefab = null;
        }
    }

    void SetWeapon()
    {
        if(dropPrefab != null)
        {
            PickupWeapon weapon = dropPrefab.GetComponent<PickupWeapon>();
            isMelee = weapon.isMelee;
            fireRate = weapon.fireRate;
            if (isMelee)
            {
                meleeRange = weapon.meleeRange;
            }
            else
            {
                firePoint.localPosition = weapon.firePosition;
                recoil = weapon.recoil;
                maxBulletCount = weapon.maxBulletCount;
                currentBulletCount = weapon.currentBulletCount;
            }
        }
    }

    public void KnockBack()
    {
        anim.SetBool("isUnarmed", true);
        StartCoroutine(KnockBackTime());
        DropWeapon();
        rigid.AddForce(-transform.right * 1f, ForceMode2D.Impulse);
    }

    IEnumerator KnockBackTime()
    {
        isKnockBack = true;
        col.isTrigger = true;
        anim.SetBool("isStun", true);
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("isStun", false);
        col.isTrigger = false;
        isKnockBack = false;
    }

    private void LookAtPlayer()
    {
        Vector2 direction = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    private void ChasePlayer()
    {
        //Vector2 direction = (player.position - transform.position).normalized;
        //Move(direction, chaseSpeed);

        if (path.finalNodeList.Count == 0)
        {
            path.Finding(lastPlayerPostion);
            path.finalNodeList.RemoveAt(0);
            firstNode = path.GetFirstNode();
        }
        rigid.position = Vector3.MoveTowards(transform.position, firstNode, chaseSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, firstNode) <= 0.01f)
        {
            if(path.finalNodeList.Count > 1)
            {
                path.Finding(lastPlayerPostion);
                path.finalNodeList.RemoveAt(0);
                firstNode = path.GetFirstNode();
            }
        }
    }

    private void RetreatFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        Move(direction, chaseSpeed);
    }

    private void Move(Vector2 direction, float speed)
    {
        Vector2 newPosition = (Vector2)transform.position + direction * speed * Time.deltaTime;
        rigid.MovePosition(newPosition);
    }

    private void CheckAttackAble()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }   
    }

    void UseWeapon()
    {
        if (!isMelee)
        {
            FireBullet();
        }
        else
        {
            SwingMeleeWeapon();
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
        if (currentBulletCount > 0)
        {
            anim.SetTrigger("doAttack");
            float recoil = Random.Range(-this.recoil, this.recoil);
            Quaternion rotation = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + recoil);
            Instantiate(bulletPrefab, firePoint.position, rotation);
            currentBulletCount--;
            PlaySFX(weapon.fireSFX);
            if (currentBulletCount == 0)
                isReload = true;
            StartCoroutine(FireDeley(fireRate));
        }
        
    }

    IEnumerator Reload(float reloadTime)
    {
        isReload = false;
        yield return new WaitForSeconds(reloadTime);
        currentBulletCount = maxBulletCount;
    }

    void SwingMeleeWeapon()
    {
        anim.SetTrigger("doAttack");
        Vector2 point = (Vector2)(transform.position) + transform.right * meleeRange / 2;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(point, meleeRange, transform.rotation.z);
        Debug.Log(colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Player")
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.Dead();
                }
            }
        }
        StartCoroutine(FireDeley(fireRate));
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.volume = sfxVolume; // 볼륨 적용
            audioSource.PlayOneShot(clip);
        }
    }

    private void MaintainDistanceFromOtherEnemies()
    {
        Vector2 separationForce = Vector2.zero;
        int neighborCount = 0;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != this)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < separationDistance)
                {
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;
                    separationForce += direction / distance;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            separationForce /= neighborCount;
            Move(separationForce, separationSpeed);
        }
    }
}
