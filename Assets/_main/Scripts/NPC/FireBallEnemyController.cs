using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class FireBallEnemyController : MonoBehaviour
    {
        [SerializeField]
        private float attackRange = 10f; // Range within which the enemy can attack
        private EnemyThrowAttack throwAttack;
        private GameObject player;
        private float cooldownTimer = 0f; // Timer to track cooldown between attacks
        private float lastAttackTime = 0f;
        private float attackCooldown = 2f; // Cooldown time between attacks
        private float topAttackCooldown = 5f;

        private Damageable damageable;

        private void Awake()
        {
            throwAttack = GetComponent<EnemyThrowAttack>();
            player = GameObject.FindGameObjectWithTag("Player");
            damageable = GetComponent<Damageable>();
        }

        private void FixedUpdate()
        {
            if (damageable != null && damageable.alive)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= attackRange && cooldownTimer <= 0f)
                {
                    // If the player is within attack range, initiate the throw attack
                    cooldownTimer = GetAttackCooldown(); // Reset cooldown timer
                    StartCoroutine(throwAttack.AttackPlayer(transform.position, player.transform.position, cooldownTimer - 0.2f));
                }
                else if (cooldownTimer > 0f)
                {
                    cooldownTimer -= Time.fixedDeltaTime; // Decrease cooldown timer
                }
            }
        }

        private float GetAttackCooldown()
        {
            // random number between attackCooldown and topAttackCooldown
            return Random.Range(attackCooldown, topAttackCooldown);
        }
    }
}
