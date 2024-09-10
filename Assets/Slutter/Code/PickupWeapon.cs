using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interaction
{
    public AnimatorOverrideController overrideController;
    public AnimationClip weaponHold;
    public AnimationClip weaponFire;
    public float fireRate;

    [Header("Gun")]
    public Vector2 firePosition;
    public float recoil;
    public int maxBulletCount;
    public int currentBulletCount;

    [Header("Melee")]
    public bool isMelee;
    public Vector2 meleeRange;

    public AudioClip fireSFX;      // ���⸦ �߻��� �� ����

    private AudioSource audioSource;

    private void Start()
    {
        // AudioSource ������Ʈ�� �����ɴϴ�. (���ٸ� �߰��մϴ�.)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void action()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController.currentWeapon == null)
        {
            PickUp(player);
        }
        else
        {
            playerController.DropWeapon();
            PickUp(player);
        }
    }

    private void PickUp(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        // ���� ���� ������ ������Ʈ
        playerController.weapon = this;
        playerController.fireRate = fireRate;
        playerController.isMelee = isMelee;
        playerController.currentWeapon = gameObject;

        if (isMelee)
        {
            playerController.isMelee = true;
            playerController.meleeRange = meleeRange;
        }

        player.transform.Find("firePosition").localPosition = firePosition;

        Animator ani = playerController.animator;

        // AnimatorOverrideController�� ����Ͽ� ���� �ִϸ��̼��� ��ü
        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(ani.runtimeAnimatorController);
        }

        // �ִϸ��̼� Ŭ���� �������� ��ü
        overrideController["Hold"] = weaponHold;
        overrideController["Fire"] = weaponFire;

        // ��ü�� ��Ʈ�ѷ��� �ִϸ����Ϳ� ����
        ani.runtimeAnimatorController = overrideController;

        // ���⸦ ��Ȱ��ȭ (ȭ�鿡�� ������� �ϱ� ����)
        gameObject.SetActive(false);
    }
}
