using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        // 달리기 키 
        bool LeftShiftInput;

        bool aKey, sKey;
        float aKey_axis;
        float sKey_axis;

        StateManager states;
        CameraManager camManager;

        float delta;

        // Use this for initialization
        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();

            camManager = CameraManager.singleton;
            camManager.Init(this.transform);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();

            states.FixedTick(delta);
            camManager.Tick(delta);
        }

        void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);

        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");

            // 왼쪽 쉬프트 키 누를 시 달리기 애니메이션으로 교체(속도 업)
            LeftShiftInput = Input.GetButton("RunInput");
 
            // 기본적으로 A키를 입력 받는다.
            aKey = Input.GetButtonDown("AKey");
            // 키 클릭의 딜레이를 보완함
            aKey_axis = Input.GetAxis("AKey");
            if (aKey_axis != 0)
                aKey = true;

            // 기본적으로 S 키를 입력 받는다.
            sKey = Input.GetButtonDown("SKey");
            // 키 클릭의 딜레이를 보완함
            sKey_axis = Input.GetAxis("SKey");
            if (sKey_axis != 0)
                sKey = true;

        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = states.vertical * camManager.transform.forward;
            Vector3 h = states.horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (LeftShiftInput)
            {
                states.run = (states.moveAmount > 0);                
            }
            else
            {
                states.run = false;
            }

            states.aKey = aKey;
            states.sKey = sKey;
        }
    }


}