using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteContainer : MonoBehaviour
{
    public Sprite[] pLegs, pUnarmedWalk, pPunch, pMac10Walk, pMac10Attack, pSushiKnifeWalk, pSushiKnifeAttack, pG19Walk, pG19Attack, pWoodStickWalk, pWoodStickAttack, pAKWalk, pAKAttack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite[] getPlayerLegs()
    {
        return pLegs;
    }
    public Sprite[] getPlayerUnarmedWalk(string name)
    {
        return pUnarmedWalk;
    }
    public Sprite[] getPlayerPunch()
    {
        return pPunch;
    }

    /*public Sprite[] getWeapon(string weapon)
    {
        switch (weapon)
        {
            case "Mac10" :
                return pMac10Attack;
                break;
            case "AK":
                return pAKAttack;
                break;
            case "G19":
                return pG19Attack;
                break;
            case "SushiKnife":
                return pSushiKnifeAttack;
                break;
            case "WoodStick":
                return pWoodStickAttack;
                break;
            default:
                return getPlayerPunch();
                break;
        }
    }
    public Sprite[] getWeaponWalk(string weapon)
    {
        switch (weapon)
        {
            case "Mac10":
                return pMac10Walk;
                break;
            case "AK":
                return pAKWalk;
                break;
            case "G19":
                return pG19Walk;
                break;
            case "SushiKnife":
                return pSushiKnifeWalk;
                break;
            case "WoodStick":
                return pWoodStickWalk;
                break;
            default:
                return getPlayerUnarmedWalk(name);
                break;
        }
    }*/
}
