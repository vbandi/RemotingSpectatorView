using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public Rigidbody TitleText;
    public Rigidbody[] BlownObjects;
    public float BlowStrength = 10f;
    public Vector3 TitleTextInitialForce;
    
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TitleText.drag = 0;
            TitleText.AddForce(TitleTextInitialForce);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            foreach (var body in BlownObjects)
            {
                BlowAway(body);
            }
        }
    }

    private void BlowAway(Rigidbody body)
    {
        if (!Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hitResult))
            return;

        if (hitResult.rigidbody == body)
        {
            body.AddForceAtPosition(_camera.transform.forward.normalized * BlowStrength, hitResult.point);
        }
    }
}
