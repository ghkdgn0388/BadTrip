using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool moving = false;
    float angle;
    public Vector2 inputVec;
    public float speed;
    public Texture2D cursorTexture;
    public Vector2 cursorHotspot = Vector2.zero;

    Rigidbody2D rigid;
    Collider2D col;    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    private void FixedUpdate()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(direction.y - transform.position.y, direction.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
}
