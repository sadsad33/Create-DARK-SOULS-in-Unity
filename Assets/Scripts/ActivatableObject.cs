using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour
{
    public bool active = false;
    protected virtual void Update() {
        if (active) Activation();
    }

    public virtual void Activation() {
        //Debug.Log("������Ʈ Ȱ��ȭ");
    }
}
