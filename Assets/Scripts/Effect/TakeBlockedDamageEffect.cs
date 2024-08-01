using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Character Effects/Take Blocked Damage")]
    public class TakeBlockedDamageEffect : CharacterEffect {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;// 데미지를 주는 캐릭터

        [Header("Damage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float staminaDamage = 0;
        public float poiseDamage = 0;

        [Header("Animation")]
        public string blockAnimation;

        public override void ProcessEffect(CharacterManager character) {
            if (character.characterStatsManager.isDead) return;
            if (character.isInvulnerable) return;

            CalculateDamage(character);
            CalculateStaminaDamage(character);
            DecideBlockAnimationBasedOnPoiseDamage(character);
            PlayBlockSoundEffect(character);
            AssignNewAITarget(character);

            if (!character.IsOwner) return; 

            if (character.characterStatsManager.isDead) {
                character.characterAnimatorManager.PlayTargetAnimation("Dead", true);
            } else {
                if (character.characterStatsManager.currentStamina <= 0) {
                    character.characterAnimatorManager.PlayTargetAnimation("BreakGuard", true);
                    character.canBeRiposted = true;
                    //Play Guard Break Sound
                    character.characterNetworkManager.isBlocking.Value = false;
                } else {
                    character.characterAnimatorManager.PlayTargetAnimation(blockAnimation, true);
                    //character.isAttacking = false;
                }
            }
        }

        private void CalculateDamage(CharacterManager character) {
            if (!character.IsOwner) return;
            if (character.characterStatsManager.isDead) return;

            if (characterCausingDamage != null) {
                physicalDamage *= (characterCausingDamage.characterStatsManager.physicalDamagePercentageModifier / 100);
                fireDamage *= (characterCausingDamage.characterStatsManager.fireDamagePercentageModifier / 100);
            }

            character.characterAnimatorManager.EraseHandIKForWeapon();

            float totalPhysicalDamageAbsorption = 1 -
                (1 - character.characterStatsManager.physicalDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionLegs / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionHands / 100);
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);


            float totalFireDamageAbsorption = 1 -
                (1 - character.characterStatsManager.fireDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionLegs / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionHands / 100);
            fireDamage -= (fireDamage * totalFireDamageAbsorption);

            physicalDamage -= physicalDamage * (character.characterStatsManager.physicalAbsorptionPercentageModifier / 100);
            fireDamage -= fireDamage * (character.characterStatsManager.fireAbsorptionPercentageModifier / 100);

            float finalDamage = physicalDamage + fireDamage;
            finalDamage = 0;
            character.characterStatsManager.currentHealth -= finalDamage;

            if (character.characterStatsManager.currentHealth <= 0) {
                character.characterStatsManager.currentHealth = 0;
                character.characterStatsManager.isDead = true;
            }
        }

        private void CalculateStaminaDamage(CharacterManager character) {
            float staminaDamageAbsorption = staminaDamage * (character.characterStatsManager.blockingStabilityRating / 100);
            float staminaDamageAfterAbsorption = staminaDamage - staminaDamageAbsorption;
            character.characterStatsManager.currentStamina -= staminaDamageAfterAbsorption;
        }

        private void DecideBlockAnimationBasedOnPoiseDamage(CharacterManager character) {
            //if (character.isTwoHandingWeapon) {
            //    if (poiseDamage <= 24) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsMediumForward);

            //    } else if (poiseDamage <= 49) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsMediumForward);
            //    } else if (poiseDamage <= 74) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsHeavyForward);
            //    } else {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsColossalForward);
            //    }
            //} else {
            //    if (poiseDamage <= 24) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsMediumForward);

            //    } else if (poiseDamage <= 49) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsMediumForward);
            //    } else if (poiseDamage <= 74) {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsHeavyForward);
            //    } else {
            //        blockAnimation = character.characterAnimatorManager.GetRandomBlockAnimationFromList(character.characterAnimatorManager.blockAnimationsColossalForward);
            //    }
            //}
            blockAnimation = "Block_Impact";
        }

        private void PlayBlockSoundEffect(CharacterManager character) {
            if (character.characterNetworkManager.isTwoHandingWeapon.Value) {
                character.characterSoundEffectsManager.PlayRandomSoundEffectFromArray(character.characterInventoryManager.rightWeapon.blockingNoises);
            } else {
                character.characterSoundEffectsManager.PlayRandomSoundEffectFromArray(character.characterInventoryManager.leftWeapon.blockingNoises);
            }
        }
        private void AssignNewAITarget(CharacterManager character) {
            EnemyManager enemy = character as EnemyManager;

            if (enemy != null && characterCausingDamage != null) {
                enemy.currentTarget = characterCausingDamage.characterStatsManager;
            }
        }
    }
}
