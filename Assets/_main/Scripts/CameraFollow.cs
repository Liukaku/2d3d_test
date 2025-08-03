using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SpriteGame
{
    public class CameraFollow : MonoBehaviour
    {

        [SerializeField]
        private Transform target;

        [SerializeField]
        private float minCameraYposition;

        public float currentRotationAngle;
        public float wantedRotationAngle;
        public float magicNumber = 10.0f;


        // each object parent has a box collider which is used to detect if the camera is blocked by an object
        // we will then disable the children of the parent object
        public bool hideBlockingObjects = true;
        public GameObject blockingObjectParentCache;
        public string childObjectCache;

        private void LateUpdate()
        {
            if (!target)
            {
                return;
            }

            currentRotationAngle = transform.eulerAngles.y;
            wantedRotationAngle = target.eulerAngles.y;

            currentRotationAngle = Mathf.LerpAngle(
                currentRotationAngle,
                wantedRotationAngle,
                0.5f);

            //float minCameraYposition = target.position.y < 5.0f ? 5.0f : target.position.y;
            minCameraYposition = 5.0f + target.position.y;

            minCameraYposition += (target.position.y / 4);

            transform.position = new Vector3(
            target.position.x,
                minCameraYposition,
                target.position.z);

            // currentRotationAngle degreed rotation around Y axis
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // rotate vector forward currentRotationAngle angle degrees around Y axis
            Vector3 rotatedPosition = currentRotation * Vector3.forward;

            transform.position -= rotatedPosition * magicNumber;

            transform.LookAt(target);
        }

        private void FixedUpdate()
        {
            if (!target)
            {
                return;
            }

            if (!hideBlockingObjects)
            {
                return;
            }

            // Check if there is an object blocking the camera view
            GameObject blockingObject = CheckForObjectBlockingCamera(target);

            // check if it is a different object from the cache, if so enable the previously cached object
            if (blockingObject != null && blockingObjectParentCache != null && blockingObject?.name != blockingObjectParentCache?.name)
            {
                Debug.Log("bing" + blockingObject.name);
                Debug.Log("bing" + blockingObjectParentCache.name);
                EnableGameObject();
            }

            if (blockingObject != null && !(blockingObjectParentCache != null && childObjectCache == blockingObject.name))
            {
                Debug.Log($"Blocking object found: {blockingObject.name}");
                childObjectCache = blockingObject.name;
                //GameObject parentObject = blockingObject.transform.parent.gameObject;
                blockingObjectParentCache = blockingObject;
                // parentObject.SetActive(false);
                foreach (Transform child in blockingObject.transform)
                {
                    // disable all children of the parent object
                    child.gameObject.SetActive(false);
                }

                return;
            }

            // nothing is currently blocking the camera but there is something still cached
            if (blockingObjectParentCache != null)
            {
                EnableGameObject();
            }
        }

        public void RotateCamera(Vector3 rotateAround)
        {
            transform.Rotate(rotateAround, 90);
        }

        private GameObject CheckForObjectBlockingCamera(Transform target)
        {
            RaycastHit hit;
            Vector3 direction = target.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude))
            {
                if (hit.transform != target)
                {
                    return hit.transform.gameObject;
                }
            }
            return null;
        }

        private bool WouldObjectBlockCamera(GameObject objToTest)
        {
            // Store original layer
            int originalLayer = objToTest.layer;

            // Set to Ignore Raycast layer (or a custom layer you use for this purpose)
            objToTest.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Perform the raycast
            bool isBlocked = false;
            RaycastHit hit;
            Vector3 direction = target.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude))
            {
                if (hit.transform != target)
                {
                    isBlocked = true;
                }
            }

            // Restore original layer
            objToTest.layer = originalLayer;

            return isBlocked;
        }

        private void EnableGameObject()
        {
            Debug.Log($"Enabling cached blocking object: {blockingObjectParentCache.name}");
            //if (WouldObjectBlockCamera(blockingObjectParentCache))
            //{
            //    Debug.Log($"Blocking object {blockingObjectParentCache.name} would block camera, skipping.");
            //    return;
            //}
            foreach (Transform child in blockingObjectParentCache.transform)
            {
                // enable all children of the parent object
                if (WouldObjectBlockCamera(child.gameObject))
                {
                    Debug.Log($"Child {child.name} would block camera, skipping.");
                    return;
                }
                Debug.Log($"Enabling child: {child.name}");
                child.gameObject.SetActive(true);
            }
            blockingObjectParentCache = null;
            childObjectCache = null;
        }
    }
}

