using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCWeaponSlotManager : CharacterWeaponSlotManager {
        public override void GrantWeaponAttackingPoiseBonus() {
            characterStatsManager.totalPoiseDefense += characterStatsManager.totalPoiseDefense + characterStatsManager.offensivePoiseBonus;
        }
        public override void ResetWeaponAttackingPoiseBonus() {
            characterStatsManager.totalPoiseDefense = characterStatsManager.armorPoiseBonus;
        }
    }
}
