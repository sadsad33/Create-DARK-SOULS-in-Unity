using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem {
        public float healAmount;

        public override void AttemptToCastSpell(CharacterManager character) {
            base.AttemptToCastSpell(character);
            Instantiate(spellWarmUpFX, character.characterAnimatorManager.transform);
            character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
            Debug.Log("영창 중..");
        }

        public override void SuccessfullyCastSpell(CharacterManager character) {
            base.SuccessfullyCastSpell(character);
            Instantiate(spellCastFX, character.characterAnimatorManager.transform);
            character.characterStatsManager.HealPlayer(healAmount);
            Debug.Log("시전!!");
        }
    }
}
