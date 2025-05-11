using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteGame
{
    public class AreaAttack : MonoBehaviour
    {
        [SerializeField]
        private Transform Target;

        [SerializeField]
        private GameObject AttackObject;
        public float CastRadius = 10f;

        [SerializeField]
        private LayerMask SightLayers;
        [SerializeField]
        private float AttackDelay = 1f;

        [SerializeField]
        private float AttackSpeed = 0.3f;

        [SerializeField]
        private bool MovesToTarget = true;

        private float LastHistoryRecordedTime;

        private float SpherecastRadius = 0.5f;
        private float LastAttackTime;


        private void Start()
        {

            GetNearestEnemy(new Vector3(0,0,0));

        }

        public IEnumerator Attack(Vector3 originPosition)
        {
            AttackObject.SetActive(true);

            if(Target == null)
            {
                GetNearestEnemy(originPosition);
            }
            

            DoAttack();
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }

        private void DoAttack()
        {
            if(MovesToTarget == false)
            {
                AttackObject.transform.position = new Vector3(Target.position.x, 0.2f, Target.position.z);
            }
            else
            {
                AttackObject.transform.position = new Vector3(Target.position.x, -1.0f, Target.position.z);
                StartCoroutine(AnimateMovingUp(Target.position, AttackSpeed));
            }
            DisableAfterTimer(AttackDelay);

        }

        private IEnumerator AnimateMovingUp(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = AttackObject.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                AttackObject.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            AttackObject.transform.position = targetPosition;
        }

        private void DisableAfterTimer(float timer)
        {
            StartCoroutine(DisableAfterTimerCoroutine(timer));
        }

        private IEnumerator DisableAfterTimerCoroutine(float timer)
        {
            yield return new WaitForSeconds(timer);
            AttackObject.SetActive(false);
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }

        public void GetNearestEnemy(Vector3 originPosition)
        {
            Collider[] colliderArray = Physics.OverlapSphere(originPosition, CastRadius);
            foreach (Collider collider in colliderArray) {
                if (collider.CompareTag("Enemy"))
                {
                    Target = collider.transform;
                    return;
                }
            }
        }

        private struct ThrowData
        {
            public Vector3 ThrowVelocity;
            public float Angle;
            public float DeltaXZ;
            public float DeltaY;
        }
    }
}