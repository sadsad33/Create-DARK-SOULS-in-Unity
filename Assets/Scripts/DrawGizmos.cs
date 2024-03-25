using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour {
    public float radius;
    public Color color;
    public int input;
    private void OnDrawGizmos() {
        Gizmos.color = color;
        switch (input) {
            case 0:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;
            case 1:
                Gizmos.DrawSphere(transform.position, radius);
                break;
            case 2:
                Gizmos.DrawCube(transform.position, new Vector3(radius, radius, radius));
                break;
        }
    }
}
