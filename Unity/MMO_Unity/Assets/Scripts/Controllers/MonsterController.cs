using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BaseController
{
    Stats _stat;
    public override void Init()
    {
        _stat = gameObject.GetComponent<Stats>();

        if(gameObject.GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }

    protected override void UpdateIdle()
    {
        Debug.Log("Monster UpdateIdle");
    }

    protected override void UpdateMoving()
    {
        Debug.Log("Monster UpdateMoving");
    }

    protected override void UpdateSkill()
    {
        Debug.Log("Monster UpdateSkill");
    }

    void OnHitEvent()
    {
        Debug.Log("Monster Hit Event");
    }
}
