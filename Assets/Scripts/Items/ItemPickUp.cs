using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ItemPickUp : Interactable {
        [Header("Item Information")]
        [SerializeField] int itemPickUpID; // 월드에 스폰될 아이템들의 식별 번호
        [SerializeField] bool hasBeenLooted; // 플레이어가 회수했는지 안했는지 여부
        public Item item;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            // 만약 현재 캐릭터 세이브 데이터의
            // 월드에서 회수한 아이템 목록에 이 아이템이 없다면
            // 이 아이템을 회수한 적이 없는 것
            // 따라서 회수 되지 않았다고 기록해야 함
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, false);
            }

            hasBeenLooted = WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[itemPickUpID];

            if (hasBeenLooted) {
                gameObject.SetActive(false);
            }
        }
        
        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // 부모의 Interact함수를 호출한다.

            // 캐릭터 데이터에 이 아이템이 플레이어에게 회수되었는지를 알려 만약 이미 회수된 아이템이라면 다시 월드에 스폰되지 않도록 함

            // 만약 현재 캐릭터 세이브 데이터의 게임 월드에서 회수한 아이템 목록에 이 스폰 아이템의 식별 번호가 존재한다면
            if (WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                // 해당 식별번호를 key 값으로 하는 항목을 제거
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Remove(itemPickUpID);
            }

            // 해당 식별번호를 key 값으로 하는 항목의 value에 true 를 입력하여 다시 추가
            WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, true);

            hasBeenLooted = true;

            // 플레이어의 인벤토리에 아이템을 전달한다
            PickUpItem(playerManager);
        }
        
        // 아이템 줍기
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventoryManager playerInventory;
            PlayerLocomotionManager playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventoryManager>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // 플레이어가 아이템을 줍는동안 정지
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            //playerInventory.weaponsInventory.Add(weapon);
            if (item is WeaponItem) playerInventory.weaponsInventory.Add(item as WeaponItem);
            else if (item is ConsumableItem) playerInventory.consumablesInventory.Add(item as ConsumableItem);

            // 무기 아이템을 루팅 할때는 무기 아이템의 이름이 표시되도록한다.
            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture; 
            playerManager.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
