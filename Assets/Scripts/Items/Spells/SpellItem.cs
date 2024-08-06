using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class SpellItem : Item {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;
        public string spellAnimation;

        [Header("Spell Cost")]
        public float focusCost;

        [Header("Spell Type")]
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        public virtual void AttemptToCastSpell(CharacterManager character) {
            //Debug.Log("주문 영창 시도");
        }

        public virtual void SuccessfullyCastSpell(CharacterManager character) {
            //Debug.Log("주문 영창 성공");

            // 주문을 사용한게 플레이어라면 집중력 소모
            PlayerManager player = character.transform.GetComponent<PlayerManager>();
            if (player != null)
                player.playerStatsManager.DeductFocus(focusCost);
        }
    }
}
