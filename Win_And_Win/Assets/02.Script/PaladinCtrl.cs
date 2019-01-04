using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    // 추후 애니메이션에 따라 추가예정
    idle, die, attack
};

public class PaladinCtrl : MonoBehaviour {

    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    public Transform tr;
    public Animator animator;
    //private Rigidbody playerRigidbody;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 5.0f;
    // 캐릭터 회전 속도 변수
    public float rotationSpeed = 100.0f;

    // 캐릭터의 위치값을 위한 변수
    [HideInInspector] public float h = 0.0f;
    [HideInInspector] public float v = 0.0f;

    private bool banjun = false;

    // Use this for initialization
    void Awake () {
        //playerRigidbody = this.GetComponent<Rigidbody>();

        // 스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        //h = Input.GetAxis("Horizontal");
        //v = Input.GetAxis("Vertical");

        //// 전후좌우 이동 방향 벡터 계산
        //Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //// Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
        //tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        //// Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
        ////tr.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * Input.GetAxis("Mouse X"));
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    banjun = true;
        //    tr.Rotate(1.0f, 180.0f, 1.0f, 0);
        //    //banjun = false;
        //}
        if (Input.GetKey(KeyCode.W))
        {
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            animator.SetBool("FrontKeyDown", true);
            animator.SetBool("BackKeyDown", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            tr.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            animator.SetBool("BackKeyDown", true);
            animator.SetBool("FrontKeyDown", false);
        }
        else
        {
            animator.SetBool("FrontKeyDown", false);
            animator.SetBool("BackKeyDown", false);
        }

        if (Input.GetKey(KeyCode.A))
            tr.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            tr.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        if(Input.GetKey(KeyCode.Mouse0))
        {
            animator.SetInteger("AttackNumber", 1);
        }
        else
        {
            animator.SetInteger("AttackNumber", 0);
        }


        // 블랜드 트리에서 v값과 h 값을 계산해서 애니메이션 실행된다.
        //animator.SetFloat("Vertical", v);
        //animator.SetFloat("Horizontal", h);
    }
}
