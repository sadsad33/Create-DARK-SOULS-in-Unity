using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class Interactable : MonoBehaviour {
        public float radius;
        public string interactableText;

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        protected virtual void Awake() {
        }

        protected virtual void Start() {
        }

        // 다른 클래스에서 사용할때 override해서 사용할 수 있도록 virtual 형으로 만든다.
        public virtual void Interact(PlayerManager playerManager) {
            Debug.Log("You interacted with an object");
        }
    }
}
