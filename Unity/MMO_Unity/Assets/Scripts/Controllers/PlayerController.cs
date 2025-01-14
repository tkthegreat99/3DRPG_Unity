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
    Texture2D _attackicon;
    Texture2D _handicon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackicon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handicon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        _stat = gameObject.GetComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseEvent; //ó���� ���ִ� ������ �ߺ� ������û �ɱ��.
        Managers.Input.MouseAction += OnMouseEvent;        
    }


    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
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

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            
            nma.Move(dir.normalized * moveDist);


            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.red);

            int _mask = (1 << (int)Define.Layer.Blocking);
            //LayerMask�� Blocking�� ���� � �����Ű��, RayCast�� ���� �÷��̾��� ���� �տ� Blocking �� ���� ��� Idle�� �ٲٰ�.
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _mask))
            {
                if(Input.GetMouseButton(0) == false)
                    _state = PlayerState.Idle;
                return;
            }

            //transform.position += dir.normalized * moveDist;

            //Quaternion.Slerp �� �ε巯�� ȸ���� �������ش�.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        //�ִϸ��̼�

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _stat.MoveSpeed);
    }

    void OnRunEvent()
    {
        Debug.Log("WalkWalk");
    }

    void UpdateIdle()
    {
        //�ִϸ��̼�

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        UpdateMouseCursor();

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

    void UpdateMouseCursor()
    {
        if (Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackicon, new Vector2(_attackicon.width / 5, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
                // Ŀ�� ����� Į ����ε� ���� ���κ��� Į ���� 1/5 ���� �з����־ ����.
            }
            else
            {
                if(_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handicon, new Vector2(_handicon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
                
            }
        }
    }


    GameObject _locktarget;

    //Ground�� Monster�� Layermask
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    void OnMouseEvent(Define.MouseEvent evt)
    {
        if(_state == PlayerState.Die)
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
                        _state = PlayerState.Moving;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _locktarget = hit.collider.gameObject;
                        else
                            _locktarget = null;       
                    }
                }
                break;
            case Define.MouseEvent.PointerUp:
                _locktarget = null;
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
