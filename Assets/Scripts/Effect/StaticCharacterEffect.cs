using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {

    public class StaticCharacterEffect : ScriptableObject {

        public int effectID;

        // Static Effect는 아이템을 장착하거나 해제하는 등의 행동으로 추가되거나 제거되는 효과
        public virtual void AddStaticEffect(CharacterManager character) {

        }
        public virtual void RemoveStaticEffect(CharacterManager character) {

        }
    }
}
