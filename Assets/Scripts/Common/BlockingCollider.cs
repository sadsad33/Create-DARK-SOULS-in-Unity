using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BlockingCollider : MonoBehaviour {
        public BoxCollider blockingCollider; // ����Ҷ� ����� collider
        public float blockingPhysicalDamageAbsorption;
        public float blockingFireDamageAbsorption;

        private void Awake() {
            blockingCollider = GetComponent<BoxCollider>();
        }

        // ���⸦ �Ű������� �޾� ���� ���� ���ġ�� �����Ѵ�.
        public void SetColliderDamageAbsorption(WeaponItem weapon) {
            if (weapon != null) {
                blockingPhysicalDamageAbsorption = weapon.physicalDamageAbsorption;
            }
        }

        public void EnableBlockingCollider() {
            blockingCollider.enabled = true;
        }

        public void DisableBlockingCollider() {
            blockingCollider.enabled = false;
        }
    }
}