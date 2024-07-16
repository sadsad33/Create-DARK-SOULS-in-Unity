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

        // �ٸ� Ŭ�������� ����Ҷ� override�ؼ� ����� �� �ֵ��� virtual ������ �����.
        public virtual void Interact(PlayerManager playerManager) {
            Debug.Log("You interacted with an object");
        }
    }
}
