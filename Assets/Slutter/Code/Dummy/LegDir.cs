using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegDir : MonoBehaviour
{
    Animator anim;

    Vector3 rot;
    public Vector2 inputVec;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        rot = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)) 
        {
            rot = new Vector3(0, 0, 90);
            transform.eulerAngles = rot;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rot = new Vector3(0, 0, 270);
            transform.eulerAngles = rot;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rot = new Vector3(0, 0, 180);
            transform.eulerAngles = rot;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot = new Vector3(0, 0, 0);
            transform.eulerAngles = rot;
        }
    }

    private void FixedUpdate()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }
    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;
        anim.SetFloat("Speed", inputVec.magnitude);
    }
}
