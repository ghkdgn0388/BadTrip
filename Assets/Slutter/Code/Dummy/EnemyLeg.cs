using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeg : MonoBehaviour
{
    Animator anim;
    private Vector2 previousPosition;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        previousPosition = transform.position.normalized;
    }
    private void LateUpdate()
    {
        // 애니메이션 제어
        if ((Vector2)transform.position != previousPosition)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // 현재 위치를 이전 위치로 업데이트
        previousPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
