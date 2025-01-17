using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    Coroutine co;
   protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        gameObject.GetOrAddComponent<CursorController>();
    }

    IEnumerator CoStopExplode(float seconds)
    {
        Debug.Log("Stop Enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Stop Execute");
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    IEnumerator ExplodeAfterSeconds(float seconds)
    {
        Debug.Log("Expload Enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Expload Execute");
        co = null;
    }

    public override void Clear()
    {
        
    }

    

    
}
