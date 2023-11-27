using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerEquipmentManager : MonoBehaviour {
        InputHandler inputHandler;
        PlayerInventory playerInventory;
        public BlockingCollider blockingCollider;

        private void Awake() {
            inputHandler = GetComponentInParent<InputHandler>();
            playerInventory = GetComponentInParent<PlayerInventory>();
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
