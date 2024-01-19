using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEffectsManager : CharacterEffectsManager {
        PlayerStatsManager playerStatsManager;
        PlayerWeaponSlotManager playerWeaponSlotManager;
        public GameObject currentParticleFX; // ���� �÷��̾��� ���¿� ���� ����(?) ȿ��
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        private void Awake() {
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
        }
        public void HealPlayerFromEffect() {
            playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStatsManager.transform); // ȸ�� ����Ʈ
            Destroy(healParticles, 2f);
        }

        public void UnLoadFlask() {
            Destroy(instantiatedFXModel.gameObject); // ����Ʈ�� ����
        }
    }
}
