using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collison");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        //Debug.Log(Input.mousePosition); // Screen

        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // Viewport


        if(Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            //int mask = (1 << 8 ) | (1 << 9);

            LayerMask mask = LayerMask.GetMask("Monster");

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                Debug.Log($"Raycast Camera @ {hit.collider.gameObject.name}");
            }
        }
        

    }
}
