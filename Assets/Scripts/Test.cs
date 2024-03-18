using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform target;

    private void Start() {
        transform.Translate(Vector3.right, Space.World);    
    }

    private void Update() {

    }
}
