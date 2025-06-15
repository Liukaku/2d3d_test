using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class SpellStatsAndDamageHandler : MonoBehaviour
    {
        public int damage = 10;

        [SerializeField]
        private TargetType targetType;

        private void Awake()
        {
            if (targetType == TargetType.Default)
            {
                targetType = TargetType.Enemy; // Default to Enemy if not set
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag(targetType.ToString()))
            {
                Debug.Log("Hit " + collider.gameObject.name);
                Damageable damageable = collider.gameObject.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("No Damageable component found on " + collider.gameObject.name);
                }
            }
        }

        private enum TargetType
        {
            Default,
            Player,
            Enemy
        }
    }

}