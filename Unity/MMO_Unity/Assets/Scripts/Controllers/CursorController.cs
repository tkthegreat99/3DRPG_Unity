using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

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
    }


    void Update()
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
                // 커서 모양이 칼 모양인데 왼쪽 위로부터 칼 끝이 1/5 정도 밀려나있어서 보정.
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handicon, new Vector2(_handicon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }

            }
        }
    }
}
