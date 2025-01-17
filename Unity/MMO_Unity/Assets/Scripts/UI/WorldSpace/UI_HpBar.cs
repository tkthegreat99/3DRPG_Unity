using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Base
{
    enum GameObjects
    {
        HpBar
    }

    Stats _stat;

    public override void Init()
    {

        Bind<GameObject>(typeof(GameObjects));   
        _stat = transform.parent.GetComponent<Stats>();
    }

    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); //collider ũ�⸸ŭ ���� ��������.
        transform.rotation = Camera.main.transform.rotation; // HPbar �� ī�޶� �����̼ǿ� �°� ���̰�.

        float ratio = _stat.HP / (float)_stat.MaxHp; //������ �������� �������� ������ ������ ����. float casting �ϱ�.
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
