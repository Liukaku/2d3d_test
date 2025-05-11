using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteGame
{
    public class ThrowAttack : MonoBehaviour
    {
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
        private Rigidbody AttackProjectile;
        public float CastRadius = 10f;

        [SerializeField]
        private LayerMask SightLayers;
        [SerializeField]
        private float AttackDelay = 5f;

        [Range(0.01f, 5f)]
        public float HistoricalTime = 1f;
        [Range(1, 100)]
        public int HistoricalResolution = 10;
        private Queue<Vector3> HistoricalPositions;

        private float HistoricalPositionInterval;
        private float LastHistoryRecordedTime;

        private float SpherecastRadius = 0.5f;
        private float LastAttackTime;


        private void Start()
        {
            AttackProjectile.useGravity = false;
            AttackProjectile.isKinematic = true;

            GetNearestEnemy(new Vector3(0,0,0));
            //SpherecastRadius = AttackProjectile.GetComponent<SphereCollider>().radius;
            //LastAttackTime = Random.Range(0, 5);

            //int capacity = Mathf.CeilToInt(HistoricalResolution * HistoricalTime);
            //HistoricalPositions = new Queue<Vector3>(capacity);
            //for (int i = 0; i < capacity; i++)
            //{
            //    HistoricalPositions.Enqueue(Target.position);
            //}
            //HistoricalPositionInterval = HistoricalTime / HistoricalResolution;
        }

        private void FixedUpdate()
        {

        }


        //private void Update()
        //{
        //    if (Time.time > LastAttackTime + AttackDelay
        //        && Physics.SphereCast(
        //            transform.position,
        //            SpherecastRadius,
        //            (Target.transform.position + Vector3.up - transform.position).normalized,
        //            out RaycastHit hit,
        //            float.MaxValue,
        //            SightLayers)
        //        && hit.transform == Target)
        //    {
        //        LastAttackTime = Time.time;
        //        AttackProjectile.transform.SetParent(transform, true);
        //        AttackProjectile.transform.localPosition = new Vector3(0, 0, 1f);
        //        AttackProjectile.useGravity = false;
        //        AttackProjectile.velocity = Vector3.zero;
        //        StartCoroutine(Attack());
        //    }

        //    if (LastHistoryRecordedTime + HistoricalPositionInterval < Time.time)
        //    {
        //        LastHistoryRecordedTime = Time.time;
        //        HistoricalPositions.Dequeue();
        //        HistoricalPositions.Enqueue(Target.position);
        //    }
        //}

        public IEnumerator Attack(Vector3 originPosition)
        {
            originPosition.y += 0.2f;
            originPosition.x += Random.Range(0.2f, 0.5f);
            AttackProjectile.gameObject.SetActive(true);
            //AttackProjectile.transform.SetParent(null, true);

            if(Target == null)
            {
                GetNearestEnemy(originPosition);
            }

            ThrowData throwData = CalculateThrowData(
                Target.position,
                originPosition
                );
            targetVelocity = throwData.ThrowVelocity;

            AttackProjectile.transform.position = originPosition;
            DoThrow(throwData, originPosition);
            AttackProjectile.excludeLayers = SightLayers;
            yield return new WaitForSeconds(0.1f);
            AttackProjectile.excludeLayers = 0;
            yield return null;
        }

        private void DoThrow(ThrowData ThrowData, Vector3 originPosition)
        {
            AttackProjectile.useGravity = true;
            AttackProjectile.isKinematic = false;
            AttackProjectile.AddForce(ThrowData.ThrowVelocity, ForceMode.VelocityChange);
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
            //AttackProjectile.velocity = Vector3.zero;
            AttackProjectile.isKinematic = true;
            AttackProjectile.useGravity = false;
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

        private ThrowData CalculateThrowData(Vector3 TargetPosition, Vector3 StartPosition)
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

        private struct ThrowData
        {
            public Vector3 ThrowVelocity;
            public float Angle;
            public float DeltaXZ;
            public float DeltaY;
        }
    }
}