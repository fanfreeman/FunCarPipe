using UnityEngine;
using System.Collections;

public class MineThrower : CanBeShootOut {
    private Vector3 shootDirection;
    private Vector3 selfRotation;
    private Transform body;
    private bool fired = false;
    private float initSpeed = 50f;


    // Use this for initialization
    void Awake()
    {
        body = transform.GetChild(0);
    }

	// Update is called once per frame
	public override void Fire (float speed)
    {
        fired = true;
        //发射方向
        shootDirection = body.transform.TransformDirection(Vector3.up);
        //发射后旋转模拟抛物线
        selfRotation = body.transform.TransformDirection(Vector3.back);
        body.GetComponent<Rigidbody>().velocity =
        shootDirection.normalized * (speed + initSpeed);

        body.GetComponent<Rigidbody>().angularVelocity =
        selfRotation * 1.8f;
    }

    void FixedUpdate ()
    {
        if(!fired) return;
            body.GetComponent<Rigidbody>().AddForce(400f*500f*Vector3.forward*Time.deltaTime);
            body.GetComponent<Rigidbody>().AddForce(1200f*500f*Vector3.down*Time.deltaTime);
    }
}
