using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    //bool _moveToDest = false;
    Vector3 _destpos;
    
    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard; // 혹시 다른 데서 구독할 수도 있으니까.
        Managers.Input.KeyAction += OnKeyboard; //구독 신청
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;        
    }


    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
    }

    PlayerState _state= PlayerState.Idle;


    void UpdateDie()
    {
        
    }

    void UpdateMoving()
    {
        Vector3 dir = _destpos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            _state = PlayerState.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            
            nma.Move(dir.normalized * moveDist);


            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.red);


            //LayerMask로 Blocking을 빌딩 등에 적용시키고, RayCast를 통해 플레이어의 방향 앞에 Blocking 이 있을 경우 Idle로 바꾸게.
            if(Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Blocking")))
            {
                _state = PlayerState.Idle;
                return;
            }

            //transform.position += dir.normalized * moveDist;

            //Quaternion.Slerp 는 부드러운 회전을 제공해준다.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        //애니메이션

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);
    }

    void OnRunEvent()
    {
        Debug.Log("WalkWalk");
    }

    void UpdateIdle()
    {
        //애니메이션

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0);
    }

    void Update()
    {

        switch(_state)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
        }        
    }

    void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if(_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        //wall인 경우에만 마우스 입력을 받아서 플레이어가 움직일 수 있게 함.
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destpos = hit.point;
            _destpos.y = transform.position.y;
            _state = PlayerState.Moving;
        }
    }
}
