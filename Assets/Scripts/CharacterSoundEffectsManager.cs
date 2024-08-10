using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterSoundEffectsManager : MonoBehaviour {
        CharacterManager character;
        AudioSource audioSource;
        // 공격 사운드
        [Header("Weapon Sounds")]
        private List<AudioClip> potentialWeaponAttackSounds;
        private AudioClip lastWeaponAttackSound = null;


        // 피격 사운드
        [Header("Taking Damage Sounds")]
        public AudioClip[] takingDamageSounds;
        private List<AudioClip> potentialDamageSounds;
        private AudioClip lastDamageSoundPlayed;

        // 발 소리


        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
            audioSource = GetComponent<AudioSource>();
        }

        public virtual void PlayRandomDamageSoundFX() {
            //potentialDamageSounds = new List<AudioClip>();

            //foreach (var damageSound in takingDamageSounds) {
            //    if (damageSound != lastDamageSoundPlayed) {
            //        potentialDamageSounds.Add(damageSound);
            //    }
            //}

            //int randomValue = Random.Range(0, potentialDamageSounds.Count);
            //lastDamageSoundPlayed = takingDamageSounds[randomValue];
            int randomValue = Random.Range(0, takingDamageSounds.Length);
            audioSource.PlayOneShot(takingDamageSounds[randomValue], 0.4f);
        }

        public virtual void PlayRandomWeaponAttackSounds() {
            potentialWeaponAttackSounds = new List<AudioClip>();

            if (character.characterNetworkManager.isUsingRightHand.Value) {
                foreach (AudioClip weaponAttackSound in character.characterInventoryManager.rightWeapon.weaponAttackSounds) {
                    if (weaponAttackSound != lastWeaponAttackSound) {
                        potentialWeaponAttackSounds.Add(weaponAttackSound);
                    }
                }

                int randomValue = Random.Range(0, potentialWeaponAttackSounds.Count);
                lastWeaponAttackSound = character.characterInventoryManager.rightWeapon.weaponAttackSounds[randomValue];
                audioSource.PlayOneShot(character.characterInventoryManager.rightWeapon.weaponAttackSounds[randomValue]);
            } else {
                foreach (AudioClip weaponAttackSound in character.characterInventoryManager.leftWeapon.weaponAttackSounds) {
                    if (weaponAttackSound != lastWeaponAttackSound) {
                        potentialWeaponAttackSounds.Add(weaponAttackSound);
                    }
                }

                int randomValue = Random.Range(0, potentialWeaponAttackSounds.Count);
                lastWeaponAttackSound = character.characterInventoryManager.leftWeapon.weaponAttackSounds[randomValue];
                audioSource.PlayOneShot(character.characterInventoryManager.leftWeapon.weaponAttackSounds[randomValue]);
            }
        }

        public virtual void PlaySoundEffect(AudioClip soundFX) {
            audioSource.PlayOneShot(soundFX);
        }

        public virtual void PlayRandomSoundEffectFromArray(AudioClip[] soundArray) {
            int index = Random.Range(0, soundArray.Length);

            PlaySoundEffect(soundArray[index]);
        }
    }
}
