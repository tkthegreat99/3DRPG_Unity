using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    

    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }

    string _name;


    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;
        UI_Base.BindEvent(
            Get<GameObject>((int)GameObjects.ItemIcon),
            ((PointerEventData) => { Debug.Log($"Item Clicked !{_name}"); }),
            Define.UIEvent.Click
        );
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
