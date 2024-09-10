using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    public enum InfoType { HighScore, Level, Floor, Time, Health }
    public InfoType type;

    Text myText;

    // Start is called before the first frame update
    void Awake()
    {
        myText = GetComponent<Text>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.HighScore:
                myText.text = string.Format("High Score: {0:F0}F", GameManager.Instance.GetHighScore());
                break;
            case InfoType.Level:
                
                break;
            case InfoType.Floor:
                myText.text = string.Format("{0:F0}F", GameManager.Instance.Floor);
                break;
            case InfoType.Time:
                float remainTime = GameManager.Instance.maxGameTime - GameManager.Instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                
                break;
        }
    }
}
