using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteGame
{
    public class PlayerController : MonoBehaviour
    {
        public float maxForwardSpeed = 0.4f;
        public float speed;
        public float speedModifier = 2;
        public float gravity = 9.8f;
        public bool canMove = true;

        private float sprintSpeed = 4.0f;
        private float defaultSpeed;
        private Vector3 movement;
        public float forwardSpeed;
        private Camera followCamera;
        private CharacterController m_ChController;
        private ThrowAttack m_ThrowAttack;
        private readonly int m_hashWalking = Animator.StringToHash("Walking");
        private readonly int m_hashAttack = Animator.StringToHash("AttackOne");
        private Animator m_Animator;
        private SpriteRenderer m_SpriteRenderer;
        private float verticalSpeed;
        private bool input_q;
        private bool input_e;
        private bool cameraRotating = false;

        void Awake()
        {
            followCamera = Camera.main;
            m_ChController = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_ThrowAttack = GetComponent<ThrowAttack>();
            defaultSpeed = speed;
        }

        private void Update()
        {
            movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Quaternion camRotation = followCamera.transform.rotation;
            //Debug.Log(Input.GetAxis("Horizontal"));
            HandleVerticalMovement();
            HandleSpriteRotation();

            if (Input.GetKeyDown(KeyCode.E))
            {
                input_e = true;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                input_q = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TriggerAttackTimer());
                StartCoroutine(m_ThrowAttack.Attack(transform.position));
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Quaternion camRotation = followCamera.transform.rotation;
            //Debug.Log(Input.GetAxis("Horizontal"));
            HandleVerticalMovement();
            HandleSpriteRotation();

            if (input_e && !cameraRotating)
            {
                StartCoroutine(HandleScreenRotationPause(90));
            }

            if (input_q && !cameraRotating)
            {
                StartCoroutine(HandleScreenRotationPause(-90));
            }


            if (!canMove) { 
                return;
            }

            if (movement.y != 0f && movement.x != 0f)
            {
                movement = Vector3.ClampMagnitude(movement, 1);
            }

            //Vector3 rotatedMovement = Quaternion.Euler(0, camRotation.eulerAngles.y, 0) * movement;
            Vector3 targetDirection = camRotation * movement;
            targetDirection.y = 0;

            float desiredSpeed = movement.normalized.magnitude * maxForwardSpeed;

            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
            {
                desiredSpeed = desiredSpeed * 0.5f;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
                //forwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.6f, Time.fixedDeltaTime);

            }
            else
            {
                speed = defaultSpeed;
                //maxForwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.4f, Time.fixedDeltaTime);

            }

            forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, Time.fixedDeltaTime);
            //Debug.Log(forwardSpeed);
            if (movement.normalized.magnitude > 0)
            {
                m_Animator.SetBool(m_hashWalking, true);
            }
            else
            {
                m_Animator.SetBool(m_hashWalking, false);
            }



            m_ChController.Move(forwardSpeed * speed * Time.fixedDeltaTime * targetDirection);
        }

        private void HandleSpriteRotation()
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                m_SpriteRenderer.flipX = true;
            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                m_SpriteRenderer.flipX = false;
            }
        }

        private void HandleVerticalMovement()
        {
            verticalSpeed = -gravity;
            m_ChController.Move(verticalSpeed * Vector3.up * Time.fixedDeltaTime);
        }

        private IEnumerator TriggerAttackTimer()
        {
            canMove = false;
            m_Animator.SetBool(m_hashAttack, true);
            yield return new WaitForSeconds(0.5f);
            m_Animator.SetBool(m_hashAttack, false);
            canMove = true;
        }

        public IEnumerator HandleScreenRotationPause(int rotateAmount)
        {
            cameraRotating = true;
            canMove = false;
            transform.Rotate(0, rotateAmount, 0);
            yield return new WaitForSeconds(0.2f);
            cameraRotating = false;
            canMove = true;
            if (rotateAmount > 0)
            {
                input_e = false;
            } else
            {
                input_q = false;
            }
        }
    }

}
