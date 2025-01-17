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
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); //collider 크기만큼 위에 만들어놓기.
        transform.rotation = Camera.main.transform.rotation; // HPbar 가 카메라 로테이션에 맞게 보이게.

        float ratio = _stat.HP / (float)_stat.MaxHp; //정수와 정수형의 나눗셈은 무조건 정수만 나옴. float casting 하기.
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = ratio;
    }
}
