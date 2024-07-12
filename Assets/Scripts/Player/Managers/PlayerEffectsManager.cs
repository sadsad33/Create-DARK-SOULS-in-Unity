using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerEffectsManager : CharacterEffectsManager {
        PlayerStatsManager playerStatsManager;
        PlayerWeaponSlotManager playerWeaponSlotManager;
        public GameObject currentParticleFX; // ���� �÷��̾��� ���¿� ���� ����(?) ȿ��
        public GameObject instantiatedFXModel;
        public float amountToBeHealed;

        protected override void Awake() {
            base.Awake();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
             
            //poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            //poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
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
                UIManager.instance.poisonBuildUpBar.gameObject.SetActive(false); // ����ġ�� �ϳ��� ���ٸ� ȭ�鿡 ������ �ʵ��� �Ѵ�.
            } else {
                UIManager.instance.poisonBuildUpBar.gameObject.SetActive(true); // ����ġ�� �����̶� �ִٸ� ȭ�鿡 ǥ��
            }
            base.HandlePoisonBuildUp();
            UIManager.instance.poisonBuildUpBar.SetPoisonBuildUpAmount(poisonBuildUp); // �� ����ġ�� ��Ÿ�� UI�ٿ� ���� ����ġ�� �ݿ��Ѵ�.
        }

        protected override void HandlePoisonedEffect() {
            if (!isPoisoned) {
                UIManager.instance.poisonAmountBar.gameObject.SetActive(false); // �� �����̻� �ɸ��� �ʾҴٸ� ����ġ ǥ�� X
            } else {
                UIManager.instance.poisonAmountBar.gameObject.SetActive(true); // �����̻� �ɸ� ���¶�� ����ġ ǥ��
            }
            base.HandlePoisonedEffect();
            UIManager.instance.poisonAmountBar.SetPoisonAmount(poisonAmount); // �� ����ġ�� ��Ÿ�� UI�ٿ� ���� ����ġ�� �ݿ�
        }

    }
}
