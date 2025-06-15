using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class Damageable : MonoBehaviour
    {
        public int health = 100;
        public bool alive = true;

        [SerializeField]
        private string uuid;

        [SerializeField]
        private int SpawnTime = 10;
        private float respawnTimer = 0f;

        private QuestLog questLog;
        private Vector3 SpawnPoint;
        private RespawnManager respawnManager;

        private void Awake()
        {
            questLog = FindObjectOfType<QuestLog>();
            respawnManager = FindObjectOfType<RespawnManager>();
            SpawnPoint = transform.position;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");

            if (health <= 0)
            {
                Die();
                Debug.Log(gameObject.name + " has died.");
            }
        }

        private void Die()
        {
            questLog.DefeatNpc(uuid);
            alive = false;
            respawnManager.Respawn(this);
            transform.gameObject.SetActive(false);
        }

        public IEnumerator Respawn()
        {
            yield return new WaitForSeconds(SpawnTime);
            transform.position = SpawnPoint;
            respawnTimer = 0f;
            alive = true;
            transform.gameObject.SetActive(true);
            Debug.Log($"{gameObject.name} has respawned.");
        }
    }
}
