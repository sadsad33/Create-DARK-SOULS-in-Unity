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

        // 방어용 Collider 적용
        public void OpenBlockingCollider() {
            if (inputHandler.twoHandFlag) { // 양잡시 오른손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
            } else { // 아닐경우 왼손 무기의 경감율 적용
                blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider() {
            blockingCollider.DisableBlockingCollider();
        }
    }
}
