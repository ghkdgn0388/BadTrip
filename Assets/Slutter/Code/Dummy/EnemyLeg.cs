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
        // �ִϸ��̼� ����
        if ((Vector2)transform.position != previousPosition)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // ���� ��ġ�� ���� ��ġ�� ������Ʈ
        previousPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
