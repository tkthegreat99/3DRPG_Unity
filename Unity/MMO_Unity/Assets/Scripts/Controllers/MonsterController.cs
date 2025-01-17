using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stats _stat;

    bool _isPlayerAlive = true;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stats>();

        if(gameObject.GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }

    protected override void UpdateIdle()
    {

        GameObject player = Managers.Game.GetPlayer();
        //GameObject player = GameObject.FindGameObjectWithTag("Player"); // tag�� �÷��̾� ã��
        if (player == null)
            return;

        if (_isPlayerAlive != true)
        {
            State = Define.State.Idle;
            return;
        }
            
        float distance  = (player.transform.position - transform.position).magnitude;
        if(distance <= _scanRange)
        {
            _locktarget = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateMoving()
    {
        if (_locktarget != null)
        {
            _destpos = _locktarget.transform.position;
            float distance = (_destpos - transform.position).magnitude;
            if (distance <= _attackRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position); // ������ ���� ������ �ְ�. ��� �����̴� �� ����.
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
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destpos);
            nma.speed = _stat.MoveSpeed;

            //Quaternion.Slerp �� �ε巯�� ȸ���� �������ش�.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        if (_locktarget != null)
        {
            Vector3 dir = _locktarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
        Debug.Log("Monster Hit Event");

        if(_locktarget != null)
        {
            Stats targetStat = _locktarget.GetComponent<Stats>();
            targetStat.OnAttacked(_stat);

            if(targetStat.HP > 0)
            {
                float distance = (_locktarget.transform.position - transform.position).magnitude;
                if(distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                _isPlayerAlive = false;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
