using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEffectsManager : MonoBehaviour {
        PlayerStats playerStats;
        WeaponSlotManager weaponSlotManager;
        public GameObject currentParticleFX; // 현재 플레이어의 상태에 따른 오라(?) 효과
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        private void Awake() {
            playerStats = GetComponentInParent<PlayerStats>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        }
        public void HealPlayerFromEffect() {
            playerStats.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStats.transform); // 회복 이펙트
            Destroy(healParticles, 2f);
        }

        public void UnLoadFlask() {
            Destroy(instantiatedFXModel.gameObject); // 에스트병 제거
        }
    }
}
