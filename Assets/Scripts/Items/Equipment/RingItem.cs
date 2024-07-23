using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Ring")]

    public class RingItem : Item {
        [SerializeField] StaticCharacterEffect effect;
        private StaticCharacterEffect effectClone;

        [Header("Item Effect Description")]
        [TextArea] public string itemEffectInformation;

        // 반지를 장착하면 캐릭터에게 효과가 적용돼야함
        public void EquipRing(CharacterManager character) {
            // Effect 의 클론을 만들어서 만약 이후에 원본인 Scriptable Object 의 변수들이 변경되어도 영향이 없도록
            effectClone = Instantiate(effect);

            character.characterEffectsManager.AddStaticEffect(effectClone);
        }
        
        // 반지를 해제하면 캐릭터에게서 효과를 제거
        public void UnEquipRing(CharacterManager character) {
            character.characterEffectsManager.RemoveStaticEffect(effect.effectID);
        }
    }
}
