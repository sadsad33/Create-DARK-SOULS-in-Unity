using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // Item이라는 ScriptableObject의 하위항목으로 WeaponItem이라는 ScriptableObject를 만든다.
    // WeaponItem은 Item이 가진 속성에 더해 GameObject와 bool 속성을 추가로 갖는다.
    [CreateAssetMenu(menuName = "Items/WeaponItem")]
    public class WeaponItem : Item {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Heavy_Attack_1;
    }
}
