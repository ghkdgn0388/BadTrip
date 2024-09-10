using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    Sprite[] walking, attacking, legsSpr;
    int counter = 0, legCount = 0;
    Player pm;
    float timer = 0.05f, legTimer = 0.05f;
    public SpriteRenderer torso, legs;
    SpriteContainer sc;

    bool attackingB = false;
    void Start()
    {
        pm = this.GetComponent<Player>();
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SpriteContainer>();
        //walking = sc.getPlayerUnarmedWalk(name);
        //attacking = sc.getPlayerPunch();
        //legsSpr = sc.getPlayerLegs();
    }

    void Update()
    {
        animateLegs();
        if (attackingB == false)
        {
            animateTorso();
        }
        else
        {
            animateAttack();
        }
    }

    void animateTorso()
    {
        if (pm&&pm.moving == true) {
            torso.sprite = walking[counter];
            timer -= Time.deltaTime;
            if (timer <= 0)
            { 
                if (counter < walking.Length - 1)
                {
                    counter++;
                } else
                {
                    counter = 0;
                }
                timer = 0.1f;
            }
        }
    }

    void animateAttack()
    {
        torso.sprite = attacking[counter];

        timer -= Time.deltaTime ;
        if(timer <= 0)
        {
            if(counter < attacking.Length -1)
            {
                counter++;
            }
            else
            {
                if(attackingB == true)
                {
                    attackingB = false;
                }
                counter = 0;
            }
            timer = 0.05f;
        }
    }

    void animateLegs()
    {
        if (pm&&pm.moving == true)
        {
            legs.sprite = legsSpr[legCount];
            legTimer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (legCount < legsSpr.Length - 1)
                {
                    legCount++;
                    Debug.Log("++");
                }
                else
                {
                    legCount = 0;
                }
                timer = 0.05f;
            }
        }
    }

    public void attack()
    {
        attackingB = true;
    }
    public void resetCounter()
    {
        counter = 0;
    }
    public bool getAttack()
    {
        return attackingB;
    }
    public void setNewTorso(Sprite[] walk, Sprite[] attack)
    {
        counter = 0;
        attacking = attack;
        walking = walk;
    }
}
