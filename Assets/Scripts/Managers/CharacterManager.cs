using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterManager : MonoBehaviour {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Comabat Colliders")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;

        [Header("Combat Flags")]
        public bool canBeRiposted;

        // 데미지는 애니메이션 이벤트로 가해질 것
        public float pendingCriticalDamage;
    }
}
