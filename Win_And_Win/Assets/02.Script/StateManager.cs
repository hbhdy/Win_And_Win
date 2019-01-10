using System.Collections;
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
        public bool rollInput;

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        

        [Header("Other")]
        public EnemyTarget lockOnTarget;

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

            a_hook.rootMotionMultiplier = 1;
            HandleRolls();
            
            animator.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;

            if (run)
            {
                targetSpeed = runSpeed;

                // LookOn한 상태에서 달리면 룩온 해제
                //lockOn = false;
            }

            if (onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            Vector3 targetDir = (lockOn == false) ? moveDir : lockOnTarget.transform.position - transform.position;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            animator.SetBool("lockOn", lockOn);

            if (lockOn == false)
                HandleMovementAnmations();
            else
                HandleLockOnMovementAnimation(moveDir);
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

        void HandleRolls()
        {
            if (!rollInput)
                return;

            float v = vertical;
            float h = horizontal;
            //v = (moveAmount > 0.3f) ? 1 : 0;
            //h = 0;

            //if (lockOn == false)
            //{
            //    v = (moveAmount > 0.3f) ? 1 : 0;
            //    h = 0;
            //}
            //else
            //{
            //    if (Mathf.Abs(v) < 0.3f)
            //        v = 0;
            //    if (Mathf.Abs(h) < 0.3f)
            //        h = 0;
            //}

            //if (v != 0)
            //{
            //    if (moveDir == Vector3.zero)
            //        moveDir = transform.forward;
            //    Quaternion targetRot = Quaternion.LookRotation(moveDir);
            //    transform.rotation = targetRot;
            //}

            //a_hook.rootMotionMultiplier = rollSpeed;

            //animator.SetFloat("Vertical", v);
            //animator.SetFloat("Horizontal", h);

            //canMove = false;
            //inAction = true;
            animator.CrossFade("Roll_Forword", 0.2f);

        }

        void HandleMovementAnmations()
        {
            animator.SetBool("run", run);
            animator.SetFloat("Vertical", moveAmount, 0.4f, delta);
        }

        void HandleLockOnMovementAnimation(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            animator.SetFloat("Vertical", v, 0.2f, delta);
            animator.SetFloat("Horizontal", h, 0.2f, delta);
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
