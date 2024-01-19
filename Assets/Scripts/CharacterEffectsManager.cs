using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterEffectsManager : MonoBehaviour {
        [Header("Weapon FX")]
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        public virtual void PlayWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.PlayWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }

        public virtual void StopWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.StopWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation) {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }
    }
}