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

        [Header("Idle Animations")]
        public string Right_Hand_Idle;
        public string Left_Hand_Idle;
        public string th_idle;

        [Header("Unarmed Animations")]
        public string UnarmedAttack1;
        public string UnarmedAttack2;

        [Header("One Handed Attack Animations")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Heavy_Attack_1;

        [Header("Two Handed Attack Animtaions")]
        public string TH_Light_Sword_Attack_1;
        public string TH_Light_Sword_Attack_2;

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
    }
}
