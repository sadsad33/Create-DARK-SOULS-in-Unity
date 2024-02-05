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

        // ���� ���
        public void PlayWeaponFX() {
            //Debug.Log("���� ���");
            normalWeaponTrail.Play();
        }

        // ���� ��� ����
        public void StopWeaponFX() {
            //Debug.Log("���� ��� ����");
            normalWeaponTrail.Stop();
        }
    }
}
