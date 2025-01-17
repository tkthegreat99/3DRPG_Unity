using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{

    //Ground�� Monster�� Layermask
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    PlayerStat _stat;

    bool _stopSkill = false;
    public override void Init()
    {
        _stat = gameObject.GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseEvent; //ó���� ���ִ� ������ �ߺ� ������û �ɱ��.
        Managers.Input.MouseAction += OnMouseEvent;
        if (gameObject.GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }

    protected override void UpdateMoving()
    {
        // ���Ͱ� �� �����Ÿ����� ������ ����
        if(_locktarget != null)
        {
            float distance = (_destpos - transform.position).magnitude;
            if(distance <= 1)
            {
                State = Define.State.Skill;
                return;
            }
        }

        //�̵�
        Vector3 dir = _destpos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.red);

            int _mask = (1 << (int)Define.Layer.Blocking);
            //LayerMask�� Blocking�� ���� � �����Ű��, RayCast�� ���� �÷��̾��� ���� �տ� Blocking �� ���� ��� Idle�� �ٲٰ�.
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _mask))
            {
                if(Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            //Quaternion.Slerp �� �ε巯�� ȸ���� �������ش�.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    void OnRunEvent()
    {
        
    }

    protected override void UpdateSkill()
    {
      if(_locktarget != null)
        {
            Vector3 dir = _locktarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //animation event���� ���� �߰��� event.
    void OnHitEvent()
    {
        if(_locktarget != null)
        {
            Stats targetStat = _locktarget.GetComponent<Stats>();
            PlayerStat myStat = gameObject.GetComponent<PlayerStat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.HP -= damage;
        }
        if(_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }

    }

    
    void OnMouseEvent(Define.MouseEvent evt)
    {
        switch(State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    if(evt == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
        }
        
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destpos = hit.point;
                        _destpos.y = transform.position.y;
                        State = Define.State.Moving;
                        _stopSkill = false;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _locktarget = hit.collider.gameObject;
                        else
                            _locktarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_locktarget == null && raycastHit)
                        _destpos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }
}
