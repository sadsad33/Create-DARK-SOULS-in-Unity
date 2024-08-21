using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    
    public class FogWall : MonoBehaviour {
        public void CloseFogWall() {
            //Debug.Log("입구 닫기");
            //transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        public void DestroyFogWall() {
            Destroy(this.gameObject);
        }
    }
}
