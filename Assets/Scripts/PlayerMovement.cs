using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    float hor;
    float ver;

    private Rigidbody rb;

    void Start()
    {
        rb  = GetComponent<Rigidbody>();
    }
    
	void Update ()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Vector3 playerMovement = new Vector3(hor, 0, ver).normalized * Speed * Time.deltaTime;
        //transform.Translate(playerMovement, Space.Self);
        rb.MovePosition(transform.position + (playerMovement * Speed * Time.deltaTime));        
    }
}