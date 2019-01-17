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

        public Transform target;
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;
        StateManager states;

        float turnSmoothing = 0.1f;
        public float minAngle = -15;
        public float maxAngle = 35;


        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;
        public float lookAngle;
        public float tiltAngle;
        float h;
        float v;

        bool usedRightAxis;

        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;

            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {

            if (Input.GetButton("CameraLeft"))
            {
                h = -1;
            }
            else if (Input.GetButton("CameraRight"))
            {
                h = 1;
            }

            if (Input.GetButton("CameraUp"))
            {
                v = -1;
            }
            else if (Input.GetButton("CameraDown"))
            {
                v = 1;
            }

            float targetSpeed = mouseSpeed;
            if (lockOnTarget != null)
            {
                if (lockOnTransform == null)
                {
                    lockOnTransform = lockOnTarget.GetTarget();
                    states.lockOnTransform = lockOnTransform;
                }
                //if (Mathf.Abs(c_h) > 0.6f)
                //{
                //    if(!usedRightAxis)
                //    {
                //        lockOnTransform = lockOnTarget.GetTarget((c_h>0));
                //        states.lockOnTransform = lockOnTransform;
                //        usedRightAxis = true;
                //    }
                //}
            }

            //if (usedRightAxis)
            //{
            //    if (Mathf.Abs(c_h) < 0.6f)
            //    {
            //        usedRightAxis = false;
            //    }
            //}

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
            h = 0;
            v = 0;
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

            // 룩온X, 회전 값 계산 및 적용 부분
            
            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            // 룩온 했을때 회전 값 계산 및 적용 부분
            if (lockOn && lockOnTarget != null)
            {
                Vector3 targetDir = lockOnTransform.position - transform.position;
                targetDir.Normalize();

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
                lookAngle = transform.eulerAngles.y;
                return;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

        }

        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}