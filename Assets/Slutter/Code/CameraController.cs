using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform을 연결
    public float followSpeed = 2f;  // 플레이어를 따라가는 속도
    public float lookAheadSpeed = 2f;  // 마우스 포인터를 따라가는 속도

    private bool followPlayer = true;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            followPlayer = false;
        }
        else
        {
            followPlayer = true;
        }

        if (followPlayer == false)
        {
            LookAhead();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector3 playerPos = player.position;
        playerPos.z = -10; // Ensure the camera's z-position is correct

        // Smoothly move the camera towards the player position
        transform.position = Vector3.Lerp(transform.position, playerPos, followSpeed * Time.deltaTime);

        //Debug.Log("Following player to position: " + playerPos);
    }

    void LookAhead()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -10; // Ensure the camera's z-position is correct

        // Smoothly move the camera towards the mouse position
        Vector3 point = (mousePos + player.position) / 2f;

        point -= player.position;
        point = Vector3.ClampMagnitude(point, 3f);
        point += player.position;

        transform.position = point;

        //Debug.Log("Looking ahead to position: " + point);
    }
}
