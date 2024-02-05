using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponFX : MonoBehaviour {
        [Header("Weapon FX")]
        public ParticleSystem normalWeaponTrail;

        private void OnEnable() {
            normalWeaponTrail.Stop();
        }

        // 彼利 犁积
        public void PlayWeaponFX() {
            //Debug.Log("彼利 犁积");
            normalWeaponTrail.Play();
        }

        // 彼利 犁积 吝瘤
        public void StopWeaponFX() {
            //Debug.Log("彼利 犁积 吝瘤");
            normalWeaponTrail.Stop();
        }
    }
}
