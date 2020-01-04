using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputs : MonoBehaviour
{
    private Camera _camera;
    private GrowableComponent _object = null;

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

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit))
            {
                _object = hit.transform.GetComponent<GrowableComponent>();
            }
        }
    }

    void OnGUI()
    {
        if (_object != null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 30;

            GUI.Label(new Rect(10, 20, 200, 60), "Debug : " + _object.Growable.EnergyRegulator.DebugString(), style);
        }
    }
}
