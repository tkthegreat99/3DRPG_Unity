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

    //bool _moveToDest = false;
    Vector3 _destpos;
    
    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseClicked; //ó���� ���ִ� ������ �ߺ� ������û �ɱ��.
        Managers.Input.MouseAction += OnMouseClicked;        
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


            //LayerMask�� Blocking�� ���� � �����Ű��, RayCast�� ���� �÷��̾��� ���� �տ� Blocking �� ���� ��� Idle�� �ٲٰ�.
            if(Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Blocking")))
            {
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
    //Ground�� Monster�� Layermask
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    void OnMouseClicked(Define.MouseEvent evt)
    {
        if(_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        //wall�� ��쿡�� ���콺 �Է��� �޾Ƽ� �÷��̾ ������ �� �ְ� ��.
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            _destpos = hit.point;
            _destpos.y = transform.position.y;
            _state = PlayerState.Moving;

            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                Debug.Log("Monster Clicked!");
            }
            else
            {
                Debug.Log("Ground Clicked!");
            }
        }
    }
}
