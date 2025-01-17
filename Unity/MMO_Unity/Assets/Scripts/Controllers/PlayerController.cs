using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destpos;
    

    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseEvent; //처음에 빼주는 이유은 중복 구독신청 될까봐.
        Managers.Input.MouseAction += OnMouseEvent;        
    }


    public enum PlayerState
    {   
        Die,
        Moving,
        Idle,
        Skill,
    }

    [SerializeField]
    PlayerState _state= PlayerState.Idle;

    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;
            Animator anim = GetComponent<Animator>();
            switch(_state)
            {
                case PlayerState.Idle:
                    anim.SetFloat("speed", 0);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Die:
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Moving:
                    anim.SetFloat("speed", _stat.MoveSpeed);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Skill:
                    anim.SetBool("attack", true);
                    break;
            }
        }
    }

    void UpdateDie()
    {
        
    }

    void UpdateMoving()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if(_locktarget != null)
        {
            float distance = (_destpos - transform.position).magnitude;
            if(distance <= 1)
            {
                State = PlayerState.Skill;
                return;
            }
        }

        //이동
        Vector3 dir = _destpos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = PlayerState.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            
            nma.Move(dir.normalized * moveDist);


            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.red);

            int _mask = (1 << (int)Define.Layer.Blocking);
            //LayerMask로 Blocking을 빌딩 등에 적용시키고, RayCast를 통해 플레이어의 방향 앞에 Blocking 이 있을 경우 Idle로 바꾸게.
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _mask))
            {
                if(Input.GetMouseButton(0) == false)
                    State = PlayerState.Idle;
                return;
            }

            //transform.position += dir.normalized * moveDist;

            //Quaternion.Slerp 는 부드러운 회전을 제공해준다.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        //애니메이션

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _stat.MoveSpeed);
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

    void UpdateSkill()
    {
        Animator anim = GetComponent<Animator>();

        anim.SetBool("attack", true);
    }

    //animation event에서 내가 추가한 event.
    void OnHitEvent()
    {
        Animator anim = GetComponent<Animator>();

        State = PlayerState.Moving;
    }

    void Update()
    {

        switch(State)
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
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }        
    }

    
    GameObject _locktarget;

    //Ground와 Monster를 Layermask
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    void OnMouseEvent(Define.MouseEvent evt)
    {
        if(State == PlayerState.Die)
            return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        switch(evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if(raycastHit)
                    {
                        _destpos = hit.point;
                        _destpos.y = transform.position.y;
                        State = PlayerState.Moving;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _locktarget = hit.collider.gameObject;
                        else
                            _locktarget = null;       
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_locktarget != null)
                    {
                        _destpos = _locktarget.transform.position;
                    }
                    else if (raycastHit)
                        _destpos = hit.point;
                }
                break;
        }     
    }
}
