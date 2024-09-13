using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Character Effects/Weapon Buff Effect")]
    public class WeaponBuffEffect : CharacterEffect {
        [Header("Buff Info")]
        [SerializeField] BuffType buffType;
        [SerializeField] float lengthOfBuff;
        public float timeRemainingOnBuff;
        [HideInInspector] public bool isRightHandedBuff;

        [Header("Buff SFX")]
        [SerializeField] AudioClip buffAmbientSound;
        [SerializeField] float ambientSoundVolume = 0.3f;

        [Header("Damage Info")]
        [SerializeField] float buffBaseDamagePercentageMultiplier = 15;

        [Header("Poise Buff")]
        [SerializeField] bool buffPoiseDamage;
        [SerializeField] float buffBasePoiseDamagePercentageMultiplier = 15;

        [Header("General")]
        [SerializeField] bool buffHasStarted = false;
        private WeaponManager weaponManager;

        // 무기 버프 진행
        public override void ProcessEffect(CharacterManager character) {
            base.ProcessEffect(character);

            // 버프 시작
            if (!buffHasStarted) {
                timeRemainingOnBuff = lengthOfBuff;
                buffHasStarted = true;
                BuffStart(character);
            }

            // 버프 유지
            if (buffHasStarted) {
                timeRemainingOnBuff -= 1;
                Debug.Log(" 버프의 남은 시간 : " + timeRemainingOnBuff);
                if (timeRemainingOnBuff <= 0) {
                    weaponManager.DebuffWeapon();
                    if (isRightHandedBuff) {
                        character.characterEffectsManager.rightWeaponBuffEffect = null;
                    }
                }
            }
        }

        public void BuffStart(CharacterManager character) {
            weaponManager = character.characterWeaponSlotManager.rightHandDamageCollider.GetComponentInParent<WeaponManager>();
            weaponManager.audioSource.loop = true;
            weaponManager.audioSource.clip = buffAmbientSound;
            weaponManager.audioSource.volume = ambientSoundVolume;

            float baseWeaponDamage = weaponManager.damageCollider.physicalDamage + weaponManager.damageCollider.fireDamage;
            float physicalBuffDamage = 0;
            float fireBuffDamage = 0;
            float poiseBuffDamage = 0;

            if (buffPoiseDamage) {
                poiseBuffDamage = weaponManager.damageCollider.poiseDamage * (buffBasePoiseDamagePercentageMultiplier / 100);
            }

            switch (buffType) {
                case BuffType.Physical:
                    physicalBuffDamage = baseWeaponDamage * (buffBaseDamagePercentageMultiplier / 100);
                    break;
                case BuffType.Fire:
                    fireBuffDamage = baseWeaponDamage * (buffBaseDamagePercentageMultiplier / 100);
                    break;
                default:
                    break;
            }

            weaponManager.BuffWeapon(buffType, physicalBuffDamage, fireBuffDamage, poiseBuffDamage);
        }

        public BuffType GetCurrentBuffType() {
            return buffType;
        }
    }
}
