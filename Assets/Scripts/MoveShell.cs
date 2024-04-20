using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShell : MonoBehaviour {

    public float speed = 1.0f;
    public Transform parentTurret;

    public void Fire()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(1,0,0));
    }
}
