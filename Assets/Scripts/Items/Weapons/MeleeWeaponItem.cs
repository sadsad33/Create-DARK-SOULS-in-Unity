using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Melee Weapon Item")]
    public class MeleeWeaponItem : WeaponItem {
        public bool canBeBuffed = true;
    }
}
