using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private bool collected;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerHealthController>() != null && !collected)
        {
            //give ammo
            PlayerController.instance.activeGun.GetAmmo();
            Destroy(gameObject);
            collected = true;
        }
    }
}
