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

    public AudioClip fireSFX;      // 무기를 발사할 때 사운드

    private AudioSource audioSource;

    private void Start()
    {
        // AudioSource 컴포넌트를 가져옵니다. (없다면 추가합니다.)
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

        // 무기 관련 데이터 업데이트
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

        // AnimatorOverrideController를 사용하여 기존 애니메이션을 교체
        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(ani.runtimeAnimatorController);
        }

        // 애니메이션 클립을 동적으로 교체
        overrideController["Hold"] = weaponHold;
        overrideController["Fire"] = weaponFire;

        // 교체된 컨트롤러를 애니메이터에 적용
        ani.runtimeAnimatorController = overrideController;

        // 무기를 비활성화 (화면에서 사라지게 하기 위해)
        gameObject.SetActive(false);
    }
}
