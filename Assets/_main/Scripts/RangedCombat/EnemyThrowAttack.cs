using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class EnemyThrowAttack : ThrowAttack
    {
        public IEnumerator AttackPlayer(Vector3 originPosition, Vector3 player, float attackDelay)
        {
            originPosition.y += 0.2f;
            originPosition.x += Random.Range(0.2f, 0.5f);
            AttackProjectile.gameObject.SetActive(true);

            RandomThrowForce();

            ThrowData throwData = CalculateThrowData(
                player,
                originPosition
                );

            if (UseAutoTarget)
            {
                throwData = GetPredictedPositionThrowData(throwData, player, originPosition);
            }

            targetVelocity = throwData.ThrowVelocity;

            AttackProjectile.transform.position = originPosition;
            DoThrow(throwData, originPosition, attackDelay);
            AttackProjectile.excludeLayers = SightLayers;
            yield return new WaitForSeconds(0.1f);
            AttackProjectile.excludeLayers = 0;
            yield return null;
        }

        private void RandomThrowForce()
        {
            // Randomize the throw force within a range
            ForceRatio = Random.Range(0, 0.2f);
        }
    }
}
