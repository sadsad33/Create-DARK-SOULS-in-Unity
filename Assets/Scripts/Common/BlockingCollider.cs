using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BlockingCollider : MonoBehaviour {
        public BoxCollider blockingCollider; // 방어할때 사용할 collider
        public float blockingPhysicalDamageAbsorption;
        public float blockingFireDamageAbsorption;

        private void Awake() {
            blockingCollider = GetComponent<BoxCollider>();
        }

        // 무기를 매개변수로 받아 물리 피해 흡수치를 설정한다.
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