using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] titles;
    public void Lose()
    {
        titles[0].SetActive(true);
    }

    public void Win()
    {
        titles[1].SetActive(true);
    }

}
