﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string theGun;

    private bool collected;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealthController>() != null && !collected)
        {
            //give ammo
            PlayerController.instance.AddGun(theGun);
            Destroy(gameObject);
            collected = true;
        }
    }
}
