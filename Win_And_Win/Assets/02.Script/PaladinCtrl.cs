using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    idle, die, attackDownUp, attackUpDown, attackHorizontal, escape
};

public class PaladinCtrl : MonoBehaviour
{
    // 캐릭터의 현재 상태 정보를 저장할 Enum 변수
    public PlayerState playerState = PlayerState.idle;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Animator animator;
    private Rigidbody playerRigidbody;

    // 캐릭터 이동 속도 변수
    public float moveSpeed = 3.0f;
    // 캐릭터 회전 속도 변수
    public float rotationSpeed = 100.0f;
    // 캐릭터 이동 거리를 위한 변수
    public Vector3 movement;

    // 캐릭터의 위치값을 위한 변수
    [HideInInspector] public float h = 0.0f;
    [HideInInspector] public float v = 0.0f;

    // 캐릭터 이동을 제한하기 위한 변수
    private bool CharacterControl = true;

    void Awake () {
        // Rigidbody 컴포넌트 할당
        playerRigidbody = this.GetComponent<Rigidbody>();

        // Animator 컴포넌트 할당
        animator = this.transform.GetChild(0).GetComponent<Animator>();
       
    }

    void Start()
    {
        // 키 입력 코루딘 시작
        StartCoroutine("KeyDownCheckCoroutine");
    }

    // 이동 및 회전 값을 받아 처리하는 FixedUpdate
    void FixedUpdate()
    {
        PlayerMove(h, v);
        PlayerRotation();
    }

    // 키 입력을 판별하는 코루틴
    IEnumerator KeyDownCheckCoroutine()
    {
        while (true)
        {
            if (CharacterControl == true)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
            }

            // 마우스 왼쪽 클릭시 첫번째 공격 애니메이션, 함수, 코루틴 실행
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerState = PlayerState.attackUpDown;
                CharacterControlLimit();
                animator.SetInteger("AttackNumber", 1);
                StartCoroutine("AnimExitCheckCoroutine");
            }
            // 마우스 오른쪽 클릭시 첫번째 공격 애니메이션, 함수, 코루틴 실행
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                playerState = PlayerState.attackDownUp;
                CharacterControlLimit();
                animator.SetInteger("AttackNumber", 2);
                StartCoroutine("AnimExitCheckCoroutine");
            }
            // F키 누를 때 세번째 공격 애니메이션, 함수, 코루틴 실행
            else if (Input.GetKeyDown(KeyCode.F))
            {
                playerState = PlayerState.attackHorizontal;
                CharacterControlLimit();
                animator.SetInteger("AttackNumber", 3);
                StartCoroutine("AnimExitCheckCoroutine");
            }

            // Space키 누를 때 구르기 애니메이션 및 코루틴 실행
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerState = PlayerState.escape;
                animator.SetBool("SpaceCheck", true);
                StartCoroutine("AnimExitCheckCoroutine");
            }

            //블랜드 트리에서 v값과 h 값을 계산해서 애니메이션 실행된다.
            animator.SetFloat("Vertical", v);
            animator.SetFloat("Horizontal", h);
            yield return null;
        }
    }

    // 애니메이션 종료 체크 코루틴
    IEnumerator AnimExitCheckCoroutine()
    {
        if (playerState == PlayerState.attackUpDown)
        {
            yield return new WaitForSeconds(1.1f);
            animator.SetInteger("AttackNumber", 0);
            CharacterControl = true;
        }
        else if(playerState == PlayerState.attackDownUp)
        {
            yield return new WaitForSeconds(1.3f);
            animator.SetInteger("AttackNumber", 0);
            CharacterControl = true;
        }
        else if(playerState == PlayerState.attackHorizontal)
        {
            yield return new WaitForSeconds(1.5f);
            animator.SetInteger("AttackNumber", 0);
            CharacterControl = true;
        }
        else if(playerState == PlayerState.escape)
        {
            yield return new WaitForSeconds(0.3f);
            animator.SetBool("SpaceCheck", false);
        }
        //StopCoroutine(AttackAnimExitCheckCoroutine());
        playerState = PlayerState.idle;
    }

    // 캐릭터 이동 함수
    void PlayerMove(float h, float v)
    {
        // 위치를 설정해주고 계산한다.
        movement.Set(h, 0, v);
        movement = movement.normalized * Time.deltaTime * moveSpeed;

        // rigidbody를 이용해 이동시킴
        playerRigidbody.MovePosition(transform.position + movement);
    }

    // 캐릭터 회전 함수
    void PlayerRotation()
    {
        // 회전후 움직임이 없을 때 정면을 바라보는 행동을 제한하기 위함
        if (h == 0 && v == 0)
        {
            return;
        }
            Quaternion newRotation = Quaternion.LookRotation(movement);    
            playerRigidbody.rotation = Quaternion.Slerp(playerRigidbody.rotation, newRotation, rotationSpeed * Time.deltaTime);     
    }

    // 캐릭터가 공격할 때 이동의 제한을 위한 함수
    void CharacterControlLimit()
    {
        if (CharacterControl == true)
        {
            CharacterControl = false;
            h = 0;
            v = 0;
        }
    }
}
