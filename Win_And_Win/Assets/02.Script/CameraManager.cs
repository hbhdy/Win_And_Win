using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{


    public class CameraManager : MonoBehaviour
    {
        public bool lockOn;
        public float followSpeed = 9;
        public float mouseSpeed = 2;
        //public float controllerSpeed = 7;

        public Transform target;

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;

        float turnSmoothing = 0.1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;
        public float lookAngle;
        public float tiltAngle;
        float h;

        public void Init(Transform t)
        {
            target = t;

            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {
                 
            if (Input.GetButton("CameraLeft")){
                 h = -1;
            }
            else if(Input.GetButton("CameraRight"))
            {
                h = 1;          
            }
            float v = Input.GetAxis("Mouse Y");

            float targetSpeed = mouseSpeed;
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
            h = 0;
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d,float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else // 입력하지 않았을 때
            {
                smoothX = h;
                smoothY = v;
            }

            if (lockOn)
            {

            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }

        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}