﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform targetTransform;      // 추적할 타깃 게임오브젝트의 Transform 변수 (쿼터뷰 시점)

    public float dist = -3.0f;       // 카메라와의 일정 거리
    public float height = 8.0f;     // 카메라의 높이 설정
    public float dampTrace = 120.0f;  // 부드러운 추적을 위한 변수
   
    // 카메라 자신의 Transform 변수
    private Transform tr;
    // 카메라의 위치를 조정해줄 변수
    private Vector3 cameraPosition;
  
    void Start()
    {
        // 카메라 자신의 Transform 컴포넌트를 tr에 할당
        tr = GetComponent<Transform>();

    }

    // Update 함수 호출 이후 한 번씩 호출되는 함수인 LateUpdate 사용
    // 추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate 사용

    private void LateUpdate()
    {
        // 각각의 좌표를 입력 받는다.
        cameraPosition.x = targetTransform.position.x;
        cameraPosition.y = targetTransform.position.y + height;
        cameraPosition.z = targetTransform.position.z + dist;

        // 적용
        tr.transform.position = cameraPosition;
        

    }
}
