using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public class Helper : MonoBehaviour
    {
        [Range(-1, 1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;

        public string[] attacks;
        public bool playAnim = true;
        public bool enableRootMotion;
        public bool drinking;
        public bool interacting;
        public bool lockOn;
        Animator animator;


        // Use this for initialization
        void Start()
        {
            //animator = this.transform.GetChild(0).GetComponent<Animator>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            enableRootMotion = !animator.GetBool("canMove");
            animator.applyRootMotion = enableRootMotion;

            interacting = animator.GetBool("interacting");

            if (lockOn==false)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }

            animator.SetBool("lockOn", lockOn);

            if (enableRootMotion)
                return;

            if (drinking)
            {             
                animator.Play("Drinking");
                drinking = false;
            }

            if(interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
            }

            if (playAnim)
            {
                string targetAnim;
                int r = Random.Range(0, attacks.Length);
                targetAnim = attacks[r];

                if (vertical > 0.5f)
                    targetAnim = "attack_3";

                vertical = 0;

                animator.CrossFade(targetAnim, 0.2f);
                //animator.SetBool("canMove", false);
                //enableRootMotion = true;
                playAnim = false;
            }
            animator.SetFloat("Vertical",vertical);
            animator.SetFloat("Horizontal", horizontal);
        }
    }
}
