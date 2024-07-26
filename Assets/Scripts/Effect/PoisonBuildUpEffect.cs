using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Character Effects/Poison Build Up")]
    public class PoisonBuildUpEffect : CharacterEffect {

        // 독 저항치의 적용 이전에 게임에서 한 틱당 축적되는 독의 양
        [SerializeField] float basePoisonBuildUpAmount = 7;
        // 독 축적치가 가득 찼을 때 중독 상태에 걸려있는 시간
        [SerializeField] float poisonAmount = 100;
        // 중독 상태에 걸렸을 때 한 틱당 받는 독 데미지
        [SerializeField] float poisonDamagePerTick = 5;
        public override void ProcessEffect(CharacterManager character) {
            // 캐릭터의 독 저항치가 적용된 독 축적치
            float finalPoisonBuildUp = 0;

            if (character.characterStatsManager.poisonResistance > 0) {
                if (character.characterStatsManager.poisonResistance >= 100) {
                    finalPoisonBuildUp = 0;
                } else {
                    float resistancePercentage = character.characterStatsManager.poisonResistance / 100;
                    finalPoisonBuildUp = basePoisonBuildUpAmount - (basePoisonBuildUpAmount * resistancePercentage);
                }
            }

            // 캐릭터에게 틱마다 독 축적
            character.characterStatsManager.poisonBuildUp += finalPoisonBuildUp;

            // 캐릭터가 이미 중독된 상태라면 빌드업 이펙트 제거
            if (character.characterStatsManager.isPoisoned) {
                character.characterEffectsManager.timedEffects.Remove(this);
            }

            // 캐릭터가 중독된 상태가 아니고 추가적으로 독이 축적되지 않는다면 축적치를 조금씩 감량해줌
            if (character.characterStatsManager.poisonBuildUp > 0 && character.characterStatsManager.poisonBuildUp < 100) {
                character.characterStatsManager.poisonBuildUp -= 1;
            }

            // 만약 축적치가 100 이상이라면 캐릭터를 중독시킴
            if (character.characterStatsManager.poisonBuildUp >= 100) {
                character.characterStatsManager.isPoisoned = true;
                character.characterStatsManager.poisonAmount = poisonAmount;
                character.characterStatsManager.poisonBuildUp = 0;

                // 원본을 사용하지말고 인스턴스화 해서 사용해야함
                // 만약 원본을 사용하게되면 원본을 사용하는 다른 모든 캐릭터가 현재 이 캐릭터의 축적치를 공유하게됨
                PoisonEffect poisonedEffect = Instantiate(WorldEffectsManager.instance.poisonedEffect);
                poisonedEffect.poisonDamage = poisonDamagePerTick;
                character.characterEffectsManager.timedEffects.Add(poisonedEffect);
                character.characterEffectsManager.timedEffects.Remove(this);
            }
        }
    }
}
