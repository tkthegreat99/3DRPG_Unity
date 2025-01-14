using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour
{

    GameObject prefab;

    GameObject tank;
    void Start()
    {
        Managers.Resource.Instantiate("Tank");
    }

}
