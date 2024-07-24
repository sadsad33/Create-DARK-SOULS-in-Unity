using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WeaponManager : MonoBehaviour {
        [Header("Buff FX")]
        [SerializeField] GameObject physicalBuffFX;
        [SerializeField] GameObject fireBuffFX;

        [Header("Trail FX")]
        [SerializeField] ParticleSystem defaultTrailFX;
        [SerializeField] ParticleSystem fireTrailFX;

        private bool weaponIsBuffed;
        private BuffType weaponBuffType;

        [HideInInspector] public MeleeWeaponDamageCollider damageCollider;
        public AudioSource audioSource;

        private void Awake() {
            damageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void BuffWeapon(BuffType buffType, float physicalBuffDamage, float fireBuffDamage, float poiseBuffDamage) {
            // 다른 활성화된 버프를 모두 리셋
            DebuffWeapon();
            
            weaponIsBuffed = true;
            weaponBuffType = buffType;
            audioSource.Play();

            switch (buffType) {
                case BuffType.Physical: physicalBuffFX.SetActive(true);
                    break;
                case BuffType.Fire: fireBuffFX.SetActive(true);
                    break;
                default:
                    break;
            }
            damageCollider.physicalBuffDamage = physicalBuffDamage;
            damageCollider.fireBuffDamage = fireBuffDamage;
            damageCollider.poiseBuffDamage = poiseBuffDamage;
        }

        public void DebuffWeapon() {
            weaponIsBuffed = false;
            audioSource.Stop();
            physicalBuffFX.SetActive(false);
            fireBuffFX.SetActive(false);
            damageCollider.physicalBuffDamage = 0;
            damageCollider.fireBuffDamage = 0;
            damageCollider.poiseBuffDamage = 0;
        }

        // 버프에 따른 무기의 트레일 재생
        public void PlayWeaponTrailFX() {
            if (weaponIsBuffed) {
                switch (weaponBuffType) {
                    case BuffType.Physical:
                        if (defaultTrailFX == null) return;
                        defaultTrailFX.Play();
                        break;
                    case BuffType.Fire:
                        if (fireTrailFX == null) return;
                        fireTrailFX.Play();
                        break;
                }
            }
        }
    }
}
