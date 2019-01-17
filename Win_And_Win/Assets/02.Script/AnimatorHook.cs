using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator animator;
        StateManager states;

        public float rootMotionMultiplier;
        public bool rolling;
        float roll_Tick;
        public void Init(StateManager st)
        {
            states = st;
            animator = st.animator;

        }

        public void InitForRoll()
        {
            rolling = true;
            roll_Tick = 0;
        }

        public void CloseRoll()
        {
            if (rolling == false)
                return;

            rootMotionMultiplier = 1;
            roll_Tick = 0;
            rolling = false;
        }

        void OnAnimatorMove()
        {
            if (states.canMove)
                return;

            states.rigid.drag = 0;

            if (rootMotionMultiplier == 0)
                rootMotionMultiplier = 1;

            if (rolling == false)
            {
                Vector3 delta = animator.deltaPosition;
                delta.y = 0;
                Vector3 v = (delta * rootMotionMultiplier) / states.delta;
                states.rigid.velocity = v;
            }
            else
            {
                roll_Tick += states.delta / 0.4f;
                if (roll_Tick > 1)
                {
                    roll_Tick = 1;
                }
                float zValue = states.roll_curve.Evaluate(roll_Tick);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rootMotionMultiplier);
                states.rigid.velocity = v2;
            }
        }

    }
}