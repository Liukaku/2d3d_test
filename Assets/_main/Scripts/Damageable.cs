using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health = 100;

    private void OnTriggerEnter(Collider other)
    {
        WeaponStats attackerWeapon = other.gameObject.GetComponent<WeaponStats>();
        Debug.Log(other.gameObject.GetComponent<WeaponStats>().damgage);

        health -= attackerWeapon.damgage;
        Debug.Log("health: " + health);
    }
}
