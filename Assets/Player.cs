using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Speed;
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
   
    // Start is called before the first frame update
    void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.forward * -Speed;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += transform.up * Speed;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += transform.up * -Speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.right * -Speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, 0.5f, 0);        
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, -0.5f, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(0.5f * Vector3.left);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(-0.5f * Vector3.left);
        }


    }

}
