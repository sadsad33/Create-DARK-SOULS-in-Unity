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

        PoisonBuildUpBar poisonBuildUpBar;
        PoisonAmountBar poisonAmountBar;

        protected override void Awake() {
            base.Awake();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();

            poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
        }
        public void HealPlayerFromEffect() {
            playerStatsManager.HealPlayer(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, playerStatsManager.transform); // ȸ�� ����Ʈ
            Destroy(healParticles, 2f);
        }

        public void UnloadFlask() {
            Destroy(instantiatedFXModel.gameObject); // ����Ʈ�� ����
        }

        protected override void HandlePoisonBuildUp() {
            if (poisonBuildUp <= 0) {
                poisonBuildUpBar.gameObject.SetActive(false); // ����ġ�� �ϳ��� ���ٸ� ȭ�鿡 ������ �ʵ��� �Ѵ�.
            } else {
                poisonBuildUpBar.gameObject.SetActive(true); // ����ġ�� �����̶� �ִٸ� ȭ�鿡 ǥ��
            }
            base.HandlePoisonBuildUp();
            poisonBuildUpBar.SetPoisonBuildUpAmount(poisonBuildUp); // �� ����ġ�� ��Ÿ�� UI�ٿ� ���� ����ġ�� �ݿ��Ѵ�.
        }

        protected override void HandlePoisonedEffect() {
            if (!isPoisoned) {
                poisonAmountBar.gameObject.SetActive(false); // �� �����̻� �ɸ��� �ʾҴٸ� ����ġ ǥ�� X
            } else {
                poisonAmountBar.gameObject.SetActive(true); // �����̻� �ɸ� ���¶�� ����ġ ǥ��
            }
            base.HandlePoisonedEffect();
            poisonAmountBar.SetPoisonAmount(poisonAmount); // �� ����ġ�� ��Ÿ�� UI�ٿ� ���� ����ġ�� �ݿ�
        }

    }
}
