using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealthController>() != null)
            PlayerHealthController.instance.HealPlayer(healAmount);
        
        Destroy(gameObject);
    }
}
