using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Character Effects/Take Damage")]
    public class TakeDamageEffect : CharacterEffect {

        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;// 데미지를 주는 캐릭터

        [Header("Damage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("SFX")]
        public bool willPlayDamageFX = true;
        public AudioClip elementalDamageSoundSFX;// 추가적인 사운드 이펙트(원소 피해 등과 같은 공격이 첨가됐을 경우)

        [Header("Directional Damage Taken From")]
        public float angleHitFrom;
        public Vector3 contactPoint; // 캐릭터에게 데미지가 가해진 지점 ( 피 이펙트를 위해 사용)


        public override void ProcessEffect(CharacterManager character) {
            if (character.characterStatsManager.isDead) return;
            if (character.isInvulnerable) return;

            // 캐릭터의 방어력을 적용한 최종 데미지 계산
            CalculateDamage(character);
            if (poiseIsBroken)
                // 어떤 방향으로부터 가해진 데미지인지에 따라 애니메이션 결정
                CheckWhichDirectionDamageCameFrom(character);
            // 애니메이션 재생
            PlayDamageAnimation(character);
            // 데미지 SFX 재생
            PlayDamageSoundFX(character);
            // 피격 FX 재생
            PlayBloodSplatter(character);
            // 만약 캐릭터가 AI라면, AI에게 현재 타겟을 공격한 캐릭터로 설정
            AssignNewAITarget(character);
        }

        private void CalculateDamage(CharacterManager character) {

           
            if (characterCausingDamage != null) {
                physicalDamage *= (characterCausingDamage.characterStatsManager.physicalDamagePercentageModifier / 100);
                //Debug.Log(characterCausingDamage.characterStatsManager.physicalDamagePercentageModifier);
                fireDamage *= (characterCausingDamage.characterStatsManager.fireDamagePercentageModifier / 100);
            }

            character.characterAnimatorManager.EraseHandIKForWeapon();

            float totalPhysicalDamageAbsorption = 1 -
                (1 - character.characterStatsManager.physicalDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionLegs / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionHands / 100);
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);

            physicalDamage -= physicalDamage * (character.characterStatsManager.physicalAbsorptionPercentageModifier / 100);

            float totalFireDamageAbsorption = 1 -
                (1 - character.characterStatsManager.fireDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionLegs / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionHands / 100);
            fireDamage -= (fireDamage * totalFireDamageAbsorption);

            fireDamage -= fireDamage * (character.characterStatsManager.fireAbsorptionPercentageModifier / 100);

            float finalDamage = physicalDamage + fireDamage;
            character.characterStatsManager.currentHealth -= finalDamage;

            Debug.Log("캐릭터의 강인도 : " + character.characterStatsManager.totalPoiseDefense);
            Debug.Log("강인도 데미지 : " + poiseDamage);
            character.characterStatsManager.totalPoiseDefense -= poiseDamage;
            character.characterStatsManager.poiseResetTimer = character.characterStatsManager.totalPoiseResetTime;
            if (character.characterStatsManager.totalPoiseDefense <= 0) {
                poiseIsBroken = true;
            }

            if (character.characterStatsManager.currentHealth <= 0) {
                character.characterStatsManager.currentHealth = 0;
                character.characterStatsManager.isDead = true;
            }
        }

        private void CheckWhichDirectionDamageCameFrom(CharacterManager character) {
            if (manuallySelectDamageAnimation) return;
            //Debug.Log(angleHitFrom);
            if (angleHitFrom >= -30 && angleHitFrom <= 30) {
                //Debug.Log("뒤!!!");
                damageAnimation = "Damage_Backward_1";
            } else if (angleHitFrom < -30 && angleHitFrom > -150) {
                //Debug.Log("오른쪽!!!");
                damageAnimation = "Damage_Right_1";
            } else if (angleHitFrom > 30 && angleHitFrom < 150) {
                //Debug.Log("왼쪽!!!");
                damageAnimation = "Damage_Left_1";
            } else {
                //Debug.Log("앞!!!");
                damageAnimation = "Damage_Forward_1";
            }
        }

        #region 데미지의 애니메이션이 여러개라면, 공격 방향과 강도에 따라 리스트에서 랜덤하게 선택해 해당 애니메이션을 재생
        private void ChooseDamageAnimationForward(CharacterManager character) {
            //if (poiseDamage <= 24) {
            //    //damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.damageAnimationsMediumForward);

            //} else if (poiseDamage <= 49) {
            //    //damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.damageAnimationsMediumForward);
            //} else if (poiseDamage <= 74) {
            //    //damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.damageAnimationsHeavyForward);
            //} else {
            //    //damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.damageAnimationsColossalForward);
            //}
        }

        private void ChooseDamageAnimationBack(CharacterManager character) {

        }

        private void ChooseDamageAnimationRight(CharacterManager character) {

        }

        private void ChooseDamageAnimationLeft(CharacterManager character) {

        }
        #endregion

        private void PlayDamageSoundFX(CharacterManager character) {
            character.characterSoundEffectsManager.PlayRandomDamageSoundFX();
            //if (fireDamage > 0) {
            //character.characterSoundEffectsManager.PlaySoundEffect(elementalDamageSoundSFX);
            //}
        }

        private void PlayDamageAnimation(CharacterManager character) {
            // 강한 공격과 약한 공격을 함께 맞는다면
            // 강한공격의 애니메이션을 재생해야함
            // 강한 공격을 맞은 바로 직후 약한 공격을 맞는다 해도, 강한 피격 애니메이션이 도중에 끊기고 약한 피격 애니메이션이 재생되는 것이 아닌 강한 공격 피격 애니메이션의 재생을 끝마쳐야함

            if (character.isInteracting && character.characterStatsManager.previousPoiseDamageTaken > poiseDamage) {
                // 현재 받은 강인도 데미지가 현재의 피격 애니메이션이 끝나기 전에 받은 강인 데미지보다 낮다면 반환
                return;
            }

            if (character.characterStatsManager.isDead) {
                character.characterWeaponSlotManager.CloseDamageCollider();
                if (characterCausingDamage.currentTarget == character) {
                    characterCausingDamage.currentTarget = null;
                    if (characterCausingDamage is PlayerManager) {
                        //Debug.Log("타겟이 죽으면 록온 해제");
                        characterCausingDamage.characterNetworkManager.isLockedOn.Value = false;
                        CameraHandler.instance.ClearLockOnTargets();
                    }
                }
                character.characterAnimatorManager.PlayTargetAnimation("Dead", true);
                //Debug.Log(character.characterStatsManager);
                if (character.characterStatsManager.isBoss) {
                    if (WorldEventManager.instance.currentEvent != null) {
                        WorldEventManager.instance.TerminateCurrentEvent();
                    }
                }
                return;
            }

            if (!poiseIsBroken) {
                return;
            } else {
                //Debug.Log("피격 애니메이션 재생");
                if (playDamageAnimation) {
                    character.characterAnimatorManager.PlayTargetAnimation(damageAnimation, true);
                }
            }
        }

        private void PlayBloodSplatter(CharacterManager character) {
            character.characterEffectsManager.PlayBloodSplatterFX(contactPoint);
        }

        private void AssignNewAITarget(CharacterManager character) {
            AICharacterManager enemy = character as AICharacterManager;

            if (enemy != null && characterCausingDamage != null) {
                enemy.currentTarget = characterCausingDamage;
            }
        }
    }
}