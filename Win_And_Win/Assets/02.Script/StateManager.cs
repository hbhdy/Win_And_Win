﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool aKey, sKey;

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        //public bool isTwoHanded;-------------------------------------------

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        float _actionDelay;

        public void Init()
        {
            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);
            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            animator.SetBool("onGround", true);
        }

        void SetUpAnimator()
        {
            if (activeModel == null)
            {
                animator = GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.Log("애니메이션을 찾을 수 없다.");
                }
                else
                {
                    activeModel = animator.gameObject;
                }
            }
            if (animator == null)
                animator = activeModel.GetComponent<Animator>();

            animator.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            DetectAction();

            if (inAction)
            {
                animator.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
                return;
            }

            canMove = animator.GetBool("canMove");

            if (!canMove)
                return;

            animator.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;

            if (run)
            {
                targetSpeed = runSpeed;
                lockOn = false;
            }

            if (onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (!lockOn)
            {
                Vector3 targetDir = moveDir;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                {
                    targetDir = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
                transform.rotation = targetRotation;

            }
            HandleMovementAnmations();
        }

        public void DetectAction()
        {
            if (canMove == false)
                return;

            if (aKey == false && sKey == false )
                return;
            string targetAnim = null;   
            if (aKey)
                targetAnim = "attack_2";
            if (sKey)
                targetAnim = "attack_3";        

            if (string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;
            animator.CrossFade(targetAnim, 0.2f);
        }


        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            animator.SetBool("onGround", onGround);
        }

        void HandleMovementAnmations()
        {
            animator.SetBool("run", run);
            animator.SetFloat("Vertical", moveAmount, 0.4f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;

            // 거리
            float dis = toGround + 0.3f;

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;

            }
            return r;
        }
    }
}
