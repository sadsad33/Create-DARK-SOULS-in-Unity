using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWall : MonoBehaviour
{
    private void Awake() { // 호출시 맨 처음에는 안개벽 비활성화
        //gameObject.SetActive(false);
    }

    public void ActivateFogWall() { // 안개벽 활성화
        gameObject.SetActive(true);
    }

    public void DeactivateFogWall() { // 안개벽 비활성화
        gameObject.SetActive(false);
    }
}
