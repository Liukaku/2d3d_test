using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteGame
{
    public class ThrowAttack : MonoBehaviour
    {
        public bool UseAutoTarget = true;

        public Vector3 targetVelocity = new Vector3(-1.99f, 3.64f, 3.38f);
        [SerializeField]
        private Transform Target;

        [SerializeField]
        private float MaxThrowForce = 25f;
        [Range(0, 1)]
        [Tooltip("Using a values closer to 0 will make the agent throw with the lower force"
        + "down to the least possible force (highest angle) to reach the target.\n"
        + "Using a value of 1 the agent will always throw with the MaxThrowForce below.")]
        public float ForceRatio = 0;
        [SerializeField]
        public Rigidbody AttackProjectile;
        public float CastRadius = 10f;

        [SerializeField]
        public LayerMask SightLayers;
        [SerializeField]
        private float AttackDelay = 5f;

        [Range(0.01f, 5f)]
        public float HistoricalTime = 1f;
        [Range(1, 100)]
        public int HistoricalResolution = 10;
        public Queue<Vector3> HistoricalPositions;

        private float HistoricalPositionInterval;
        private float LastHistoryRecordedTime;

        private float SpherecastRadius = 0.5f;
        private float LastAttackTime;

        private int capacity;

        private void Awake()
        {
            AttackProjectile.useGravity = false;
            AttackProjectile.isKinematic = true;

            GetNearestEnemy(new Vector3(0,0,0));

            capacity = Mathf.CeilToInt(HistoricalResolution * HistoricalTime);
            HistoricalPositions = new Queue<Vector3>(capacity);
        }

        private void FixedUpdate()
        {
            if (LastHistoryRecordedTime + HistoricalPositionInterval < Time.time && Target != null)
            {
                LastHistoryRecordedTime = Time.time;
                if (HistoricalPositions.Count >= capacity)
                {
                    HistoricalPositions.Dequeue();
                }
                HistoricalPositions.Enqueue(Target.position);
            }
        }

        public IEnumerator Attack(Vector3 originPosition)
        {
            originPosition.y += 0.2f;
            originPosition.x += Random.Range(0.2f, 0.5f);
            AttackProjectile.gameObject.SetActive(true);

            if(Target == null)
            {
                GetNearestEnemy(originPosition);
            }

            ThrowData throwData = CalculateThrowData(
                Target.position,
                originPosition
                );

            if (UseAutoTarget)
            {
                throwData = GetPredictedPositionThrowData(throwData, Target.position, originPosition);
            }

            targetVelocity = throwData.ThrowVelocity;

            AttackProjectile.transform.position = originPosition;
            DoThrow(throwData, originPosition, 0f);
            AttackProjectile.excludeLayers = SightLayers;
            yield return new WaitForSeconds(0.1f);
            AttackProjectile.excludeLayers = 0;
            yield return null;
        }

        public void DoThrow(ThrowData ThrowData, Vector3 originPosition, float attackDelay)
        {
            AttackProjectile.useGravity = true;
            AttackProjectile.isKinematic = false;
            AttackProjectile.AddForce(ThrowData.ThrowVelocity, ForceMode.VelocityChange);

            //override the delay if it is set to 0
            if (attackDelay == 0f)
            {
                attackDelay = AttackDelay;
            }

            DisableAfterTimer(AttackDelay);

        }

        private void DisableAfterTimer(float timer)
        {
            StartCoroutine(DisableAfterTimerCoroutine(timer));
        }

        private IEnumerator DisableAfterTimerCoroutine(float timer)
        {
            yield return new WaitForSeconds(timer);
            AttackProjectile.gameObject.SetActive(false);
            AttackProjectile.isKinematic = true;
            AttackProjectile.useGravity = false;
        }

        public void SetTarget(Transform target)
        {
            Target = target;

            int capacity = Mathf.CeilToInt(HistoricalResolution * HistoricalTime);
            HistoricalPositions = new Queue<Vector3>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                HistoricalPositions.Enqueue(Target.position);
            }
            HistoricalPositionInterval = HistoricalTime / HistoricalResolution;

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

        public ThrowData CalculateThrowData(Vector3 TargetPosition, Vector3 StartPosition)
        {
            Vector3 displacement = new Vector3(
                TargetPosition.x,
                StartPosition.y,
                TargetPosition.z
                ) - StartPosition;

            float deltaY = TargetPosition.y - StartPosition.y;
            float deltaXZ = displacement.magnitude;

            float gravity = Mathf.Abs(Physics.gravity.y);
            float throwStrength = Mathf.Clamp(
                Mathf.Sqrt(
                    gravity *
                    (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) +
                    Mathf.Pow(deltaXZ, 2)))),
                0.01f,
                MaxThrowForce);

            throwStrength = Mathf.Lerp(throwStrength, MaxThrowForce, ForceRatio);

            float angle;
            if (ForceRatio == 0)
            {
                angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2 - (deltaY / deltaXZ)));
            }
            else
            {
                angle = Mathf.Atan(
                    (Mathf.Pow(throwStrength, 2) - Mathf.Sqrt(
                        Mathf.Pow(throwStrength, 4) - gravity
                        * (gravity * Mathf.Pow(deltaXZ, 2)
                        + 2 * deltaY * Mathf.Pow(throwStrength, 2)))
                    ) / (gravity * deltaXZ)
                );
            }
            if (float.IsNaN(angle))
            {
                // you will need to handle this case when there
                // is no feasible angle to throw the object and reach the target.
                return new ThrowData();
            }

            Vector3 initialVelocity =
                Mathf.Cos(angle) * throwStrength * displacement.normalized
                + Mathf.Sin(angle) * throwStrength * Vector3.up;

            return new ThrowData
            {
                ThrowVelocity = initialVelocity,
                Angle = angle,
                DeltaXZ = deltaXZ,
                DeltaY = deltaY
            };
        }

        public ThrowData GetPredictedPositionThrowData(ThrowData DirectThrowData, Vector3 TargetPosition, Vector3 originPosition)
        {
            Vector3 throwVelocity = DirectThrowData.ThrowVelocity;
            throwVelocity.y = 0;
            float time = DirectThrowData.DeltaXZ / throwVelocity.magnitude;
            Vector3 playerMovement;


            Vector3[] positions = HistoricalPositions.ToArray();
            Vector3 averageVelocity = Vector3.zero;
            for (int i = 1; i < positions.Length; i++)
            {
                averageVelocity += (positions[i] - positions[i - 1]) / HistoricalPositionInterval;
            }
            averageVelocity /= HistoricalTime * HistoricalResolution;
            playerMovement = averageVelocity;

            Vector3 newTargetPosition = new Vector3(
                TargetPosition.x + playerMovement.x,
                TargetPosition.y + playerMovement.y,
                TargetPosition.z + playerMovement.z
            );

            // Option Calculate again the trajectory based on target position
            ThrowData predictiveThrowData = CalculateThrowData(
                newTargetPosition,
                originPosition
            );

            predictiveThrowData.ThrowVelocity = Vector3.ClampMagnitude(
                predictiveThrowData.ThrowVelocity,
                MaxThrowForce
            );

            return predictiveThrowData;
        }

        public struct ThrowData
        {
            public Vector3 ThrowVelocity;
            public float Angle;
            public float DeltaXZ;
            public float DeltaY;
        }
    }
}