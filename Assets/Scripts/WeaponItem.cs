using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // Item�̶�� ScriptableObject�� �����׸����� WeaponItem�̶�� ScriptableObject�� �����.
    // WeaponItem�� Item�� ���� �Ӽ��� ���� GameObject�� bool �Ӽ��� �߰��� ���´�.
    [CreateAssetMenu(menuName = "Items/WeaponItem")]
    public class WeaponItem : Item {
        public GameObject modelPrefab;
        public bool isUnarmed;
    }
}
