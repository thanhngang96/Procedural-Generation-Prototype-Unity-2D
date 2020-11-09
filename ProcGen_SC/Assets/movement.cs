using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    Rigidbody2D rbody;
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.gravityScale = 0;
        moveSpeed = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        // Get user's current movement input     
        Vector2 movementVector = new Vector2(Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
        //rbody.isKinematic = false;
        rbody.MovePosition(rbody.position + movementVector * moveSpeed * Time.deltaTime);
        //rbody.velocity = Vector3.zero;   
        rbody.angularVelocity = 0f;  
        transform.localRotation = Quaternion.identity;
    }
}
