using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class UI_HpBar : UI_Base
{
    enum GameObjects
    {
        HPBar
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObject));   
    }

    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y); //collider 크기만큼 위에 만들어놓기.
    }
}
