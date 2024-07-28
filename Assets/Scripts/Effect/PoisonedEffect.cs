using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Character Effects/Poison Effect")]
    public class PoisonedEffect : CharacterEffect {
        public float poisonDamage = 1;

        public override void ProcessEffect(CharacterManager character) {
            PlayerManager player = character as PlayerManager;
            if (character.characterStatsManager.isPoisoned) { // 캐릭터가 중독된 상태라면
                if (character.characterStatsManager.poisonAmount > 0) { // 캐릭터의 독 축적치가 아직 남아있다면
                    character.characterStatsManager.poisonAmount -= 1;
                    if (player != null) {
                        //Debug.Log("Poison Damage");
                        player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(character.characterStatsManager.poisonAmount);
                    }

                } else { // 캐릭터의 독 축적치가 모두 경감됐다면
                    character.characterStatsManager.isPoisoned = false; // 중독상태를 해제
                    character.characterStatsManager.poisonAmount = 0; // 축적치를 0으로 만들어줌
                    player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(0);
                }
            } else { // 캐릭터가 중독상태가 아니라면
                character.characterEffectsManager.timedEffects.Remove(this); // 캐릭터의 시간에 따른 이펙트 리스트에서 독 이펙트를 제거해줌
                character.characterEffectsManager.RemoveTimedEffectParticle(EffectParticleType.Poison);
            }
        }
    }
}
