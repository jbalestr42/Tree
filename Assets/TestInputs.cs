using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputs : MonoBehaviour
{
    public Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                
                objectHit.GetComponent<MeshRenderer>().material.color = Color.black;
                objectHit.GetComponent<GrowableComponent>().Growable.Kill();
            }
        }
    }
}
