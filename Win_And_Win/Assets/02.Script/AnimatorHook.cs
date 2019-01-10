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

        public void Init(StateManager st)
        {
            states = st;
            animator = st.animator;
        }

        void OnAnimatorMove()
        {
            if (states.canMove)
                return;

            states.rigid.drag = 0;

            if (rootMotionMultiplier == 0)
                rootMotionMultiplier = 1;

            Vector3 delta = animator.deltaPosition;
            delta.y = 0;
            Vector3 v = (delta * rootMotionMultiplier) / states.delta;
            states.rigid.velocity = v;
        }

    }
}