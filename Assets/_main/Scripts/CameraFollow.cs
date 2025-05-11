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

        public void RotateCamera(Vector3 rotateAround)
        {
            transform.Rotate(rotateAround, 90);
        }
    }

}
