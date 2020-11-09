using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camerabehavior : MonoBehaviour
{
    public Transform follow;

	public float smoothSpeed = 0.125f;
	public Vector3 p;

	void LateUpdate()
    {
        p.x =  follow.position.x;
        p.y =  follow.position.y;
        p.z = -10f;
    //transform.position = p;
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, p, smoothSpeed);
		transform.position = smoothedPosition;
    }

}
