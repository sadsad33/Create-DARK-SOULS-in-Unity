using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ItemPickUp : Interactable {
        [Header("Item Information")]
        [SerializeField] int itemPickUpID;
        [SerializeField] bool hasBeenLooted;
        public Item item;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();
            
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, false);
            }

            hasBeenLooted = WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[itemPickUpID];

            if (hasBeenLooted) {
                gameObject.SetActive(false);
            }
        }
        
        public override void Interact(PlayerManager player) {
            base.Interact(player);

            if (WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Remove(itemPickUpID);
            }

            WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, true);

            hasBeenLooted = true;

            PickUpItem(player);
        }
        
        private void PickUpItem(PlayerManager player) {
            Debug.Log("아이템 줍기");
            PlayerInventoryManager playerInventory;
            PlayerAnimatorManager animatorHandler;

            playerInventory = player.GetComponent<PlayerInventoryManager>();
            animatorHandler = player.GetComponentInChildren<PlayerAnimatorManager>();

            //playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero; // �÷��̾ �������� �ݴµ��� ����
            player.characterController.Move(Vector3.zero);
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            if (item is WeaponItem) {
                playerInventory.weaponsInventory.Add(item as WeaponItem);
                if (UIManager.instance.weaponInventorySlots.Length <= UIManager.instance.player.playerInventoryManager.weaponsInventory.Count) {
                    int currentIndex = UIManager.instance.weaponInventorySlots.Length;
                    Instantiate(UIManager.instance.itemInventorySlotPrefab, UIManager.instance.weaponInventorySlotsParent); // 슬롯 추가
                    UIManager.instance.weaponInventorySlots = UIManager.instance.weaponInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
                    UIManager.instance.weaponInventorySlots[currentIndex].AddItem(item);
                }
            } else if (item is ConsumableItem) {
                playerInventory.consumablesInventory.Add(item as ConsumableItem);
                if (UIManager.instance.consumableInventorySlots.Length <= UIManager.instance.player.playerInventoryManager.consumablesInventory.Count) {
                    int currentIndex = UIManager.instance.consumableInventorySlots.Length;
                    Instantiate(UIManager.instance.itemInventorySlotPrefab, UIManager.instance.consumableInventorySlotsParent); // 슬롯 추가
                    UIManager.instance.consumableInventorySlots = UIManager.instance.consumableInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
                    UIManager.instance.consumableInventorySlots[currentIndex].AddItem(item);
                }
            }
            
            UIManager.instance.ItemPopUpGameObject.GetComponentInChildren<Text>().text = item.itemName;
            UIManager.instance.ItemPopUpGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
            UIManager.instance.ItemPopUpGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
