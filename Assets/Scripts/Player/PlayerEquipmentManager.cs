using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEquipmentManager : MonoBehaviour {
        InputHandler inputHandler;
        PlayerInventory playerInventory;
        
        [Header("Equipment Model Changer")]
        HelmetModelChanger helmetModelChanger;

        public BlockingCollider blockingCollider;

        private void Awake() {
            inputHandler = GetComponentInParent<InputHandler>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
        }

        private void Start() {
            helmetModelChanger.UnEquipAllHelmetModels();
            helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
        }

        // ���� Collider ����
        public void OpenBlockingCollider() {
            if (inputHandler.twoHandFlag) { // ����� ������ ������ �氨�� ����
                blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
            } else { // �ƴҰ�� �޼� ������ �氨�� ����
                blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider() {
            blockingCollider.DisableBlockingCollider();
        }
    }
}
