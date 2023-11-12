using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterManager : MonoBehaviour {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Comabat Colliders")]
        public BoxCollider backStabBoxCollider;
        public BackStabCollider backStabCollider;
    }
}
