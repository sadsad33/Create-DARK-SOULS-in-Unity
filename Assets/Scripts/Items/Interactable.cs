using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class Interactable : MonoBehaviour {
        public float radius = 0.6f;
        public string interactableText;

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }


        // �ٸ� Ŭ�������� ����Ҷ� override�ؼ� ����� �� �ֵ��� virtual ������ �����.
        public virtual void Interact(PlayerManager playerManager) {
            Debug.Log("You interacted with an object");
        }
    }
}