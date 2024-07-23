using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {

    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Modify Damage Type")]

    // 데미지에 배율을 추가하는 정적 효과
    public class ModifyDamageTypeStaticEffect : StaticCharacterEffect {
        [Header("Damage Type Effected")]
        [SerializeField] DamageType damageType;
        [SerializeField] int modifiedValue = 0;

        
        // 인수로 전달받은 캐릭터에게 효과를 추가해줌
        public override void AddStaticEffect(CharacterManager character) {
            base.AddStaticEffect(character);

            switch (damageType) {
                case DamageType.Physical: 
                    character.characterStatsManager.physicalDamagePercentageModifier += modifiedValue;
                    break;
                case DamageType.Fire:
                    character.characterStatsManager.fireDamagePercentageModifier += modifiedValue;
                    break;
                default:
                    break;
            }
        }

        // 효과가 제거되면 값을 빼줘야함
        public override void RemoveStaticEffect(CharacterManager character) {
            base.RemoveStaticEffect(character);

            switch (damageType) {
                case DamageType.Physical:
                    character.characterStatsManager.physicalDamagePercentageModifier -= modifiedValue;
                    break;
                case DamageType.Fire:
                    character.characterStatsManager.fireDamagePercentageModifier -= modifiedValue;
                    break;
                default:
                    break;
            }
        }

    }
}
