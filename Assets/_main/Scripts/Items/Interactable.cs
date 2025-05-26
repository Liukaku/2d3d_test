using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteGame
{
    public class Interactable : MonoBehaviour
    {
        public string Name = "Generic Item";
        public string ExamineText = "You see nothing special.";
        public bool MagicRotating = true;
        public bool CanBePickedUp = true;
        public bool IsInteracting = false;
        private GameObject ParentObject;
        private Inventory PlayerInventory;
        private ItemInteractManager itemInteractManager;

        void Awake()
        {
            ParentObject = transform.gameObject;
            PlayerInventory = FindObjectOfType<Inventory>();
            itemInteractManager = ParentObject.GetComponent<ItemInteractManager>();
            if (ParentObject == null)
            {
                Debug.LogError("Interactable object must have a parent GameObject.");
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // slowly rotate the object
            if (MagicRotating)
            {
                transform.Rotate(Vector3.up, 20 * Time.deltaTime);
            }
        }

        public void Interact()
        {
            Debug.Log($"Interacting with {Name}");
            // Add interaction logic here
            itemInteractManager.InitiateInteract();
        }

        public void PickUp()
        {
            if (CanBePickedUp)
            {
                PlayerInventory.AddToInventory(this);
                ParentObject.SetActive(false);
            }
        }

    }
}
