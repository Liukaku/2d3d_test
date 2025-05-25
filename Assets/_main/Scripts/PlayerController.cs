using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public GameObject TargetIndicator;
        public bool canRotateCamera = true;

        private float sprintSpeed = 4.0f;
        private float dashSpeed = 10.0f;
        private float dashCooldown = 3.0f; // seconds
        private float dashTimer = 20.0f; // seconds
        private float dashCooldownTimer = 0.0f; // seconds

        private float defaultSpeed;
        private Vector3 movement;
        public float forwardSpeed;

        private Camera followCamera;
        private CharacterController m_ChController;
        private ThrowAttack m_ThrowAttack;
        private AreaAttack m_SpikeAttack;
        private readonly int m_hashWalking = Animator.StringToHash("Walking");
        private readonly int m_hashAttack = Animator.StringToHash("AttackOne");
        private Animator m_Animator;
        private SpriteRenderer m_SpriteRenderer;
        private float verticalSpeed;
        private bool input_q;
        private bool input_e;
        private bool input_click;
        private bool input_one;
        private bool cameraRotating = false;
        private DialogManager chattingWith = null;


        void Awake()
        {
            followCamera = Camera.main;
            m_ChController = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_ThrowAttack = GetComponent<ThrowAttack>();
            m_SpikeAttack = GetComponent<AreaAttack>();
            defaultSpeed = speed;
        }

        private void Update()
        {
            movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Quaternion camRotation = followCamera.transform.rotation;
            //Debug.Log(Input.GetAxis("Horizontal"));
            HandleVerticalMovement();
            HandleSpriteRotation();

            if (Input.GetKeyDown(KeyCode.E) && canRotateCamera)
            {
                input_e = true;
            }

            if (Input.GetKeyDown(KeyCode.Q) && canRotateCamera)
            {
                input_q = true;
            }

            if (Input.GetMouseButtonDown(0) && !isInConversation())
            {
                //StartCoroutine(TriggerAttackTimer());
                input_click = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                input_one = true;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ChangeTarget();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartInteract();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseAllInterfaces();
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
            DashCooldownHandler();

            if (input_click && !cameraRotating)
            {
                StartCoroutine(m_ThrowAttack.Attack(transform.position));
                input_click = false;
            }

            if (input_one && !cameraRotating)
            {
                StartCoroutine(m_SpikeAttack.Attack(transform.position));
                input_one = false;
            }

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
                desiredSpeed = desiredSpeed * 0.7f;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                DashToSprint();

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

        private void DashCooldownHandler()
        {
            // Check if the dash is on cooldown
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
        }

        public void DashToSprint()
        {
            if(dashCooldownTimer <= 0)
            {
                StartCoroutine(DashCoroutine());

                // Reset dash timer and cooldown
                dashCooldownTimer = dashCooldown;
            }

        }

        private IEnumerator DashCoroutine()
        {
            speed = dashSpeed;

            yield return new WaitForSeconds(0.1f);
            speed = sprintSpeed;

        }

        private bool isInConversation()
        {
            if(chattingWith != null && chattingWith.inConversation)
            {
                return true;
            } else
            {
                return false;
            }
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

        private void CloseAllInterfaces()
        {
            if(chattingWith != null)
            {
                chattingWith.CloseDialog();
                chattingWith = null;
            }
        }

        private void StartInteract()
        {
            Collider[] chatters = GetCollidersByTag(2f, "NPC", "Interactable");
            Debug.Log(chatters);
            Collider chat = chatters[0];
            if (chat.CompareTag("NPC"))
            {
                Debug.Log("Chatting with: " + chat.gameObject.name);
                if (chat.TryGetComponent<DialogManager>(out chattingWith))
                {

                    chattingWith.StartDialog(2.0f);
                    return;
                }
                else
                {
                    Debug.Log("No one to chat with");
                    return;
                }
            }
            if (chat.CompareTag("Interactable"))
            {
                if (chat.TryGetComponent<Interactable>(out var interactable))
                {
                    interactable.Interact();
                    return;
                }
                else
                {
                    Debug.Log("No interactable object found");
                    return;
                }
            }

        }


        private Collider[] GetCollidersByTag(float distance, string tag, string tagTwo = null)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, distance);
            return colliderArray.Where(c => c.CompareTag(tag) || c.CompareTag(tagTwo)).ToArray();
        }

        public Collider[] targets;
        private int targetIndex = 0;
        private void ChangeTarget()
        {
            //Collider[] colliderArray = Physics.OverlapSphere(transform.position, 10f);
            //Collider[] filteredColliders = colliderArray.Where(c => c.CompareTag("Enemy")).ToArray();
            Collider[] filteredColliders = GetCollidersByTag(10f, "Enemy");


            // if this is the first time we are setting the target, set it to the first enemy OR if if we need to reset the target
            if (targets.Length == 0 || targets[targetIndex] == null)
            {
                targetIndex = 0;
            } else
            {
                targetIndex++;
                if (targetIndex >= filteredColliders.Length)
                {
                    targetIndex = 0;
                }
            }
            

            targets = filteredColliders;
            Debug.Log(filteredColliders[targetIndex].gameObject.name);
            Debug.Log(filteredColliders[targetIndex].gameObject.tag);
            TargetIndicator.transform.SetParent(filteredColliders[targetIndex].transform, false);
            m_ThrowAttack.SetTarget(filteredColliders[targetIndex].transform);
            m_SpikeAttack.SetTarget(filteredColliders[targetIndex].transform);

        }

        private void OnDrawGizmos()
        {
            Color c = new Color(0.0f, 0.0f, 1.0f, 0.5f);
            UnityEditor.Handles.color = c;

            // draw a circle at current position with a radius of 2
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, 2f);
        }
    }
}
