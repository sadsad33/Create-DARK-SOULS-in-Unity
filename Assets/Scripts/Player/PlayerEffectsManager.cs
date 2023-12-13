using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEffectsManager : MonoBehaviour {
        PlayerStats playerStats;
        WeaponSlotManager weaponSlotManager;
        public GameObject currentParticleFX; // ���� �÷��̾��� ���¿� ���� ����(?) ȿ��
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        private void Awake() {
            playerStats = GetComponentInParent<PlayerStats>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        }
        public void HealPlayerFromEffect() {
            playerStats.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStats.transform); // ȸ�� ����Ʈ
            Destroy(healParticles, 2f);
        }

        public void UnLoadFlask() {
            Destroy(instantiatedFXModel.gameObject); // ����Ʈ�� ����
        }
    }
}
