using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class RespawnManager : MonoBehaviour
    {
        public void Respawn(Damageable damageable)
        {
            if (damageable != null && !damageable.alive)
            {
                StartCoroutine(damageable.Respawn());
            }
            else
            {
                Debug.LogWarning("Damageable is either null or already alive.");
            }
        }
    }
}
