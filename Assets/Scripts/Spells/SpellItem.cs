using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class SpellItem : MonoBehaviour {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;

        public string spellAnimation;
        [Header("Spell Type")]
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        public virtual void AttemptToCastSpell() {
            Debug.Log("주문 영창 시도");
        }

        public virtual void SuccessfullyCastSpell() {
            Debug.Log("주문 영창 성공");
        }
    }
}
