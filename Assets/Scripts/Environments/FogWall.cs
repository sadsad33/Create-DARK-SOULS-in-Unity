using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWall : MonoBehaviour
{
    private void Awake() { // ȣ��� �� ó������ �Ȱ��� ��Ȱ��ȭ
        //gameObject.SetActive(false);
    }

    public void ActivateFogWall() { // �Ȱ��� Ȱ��ȭ
        gameObject.SetActive(true);
    }

    public void DeactivateFogWall() { // �Ȱ��� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
