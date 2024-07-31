using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ItemPickUp : Interactable {
        [Header("Item Information")]
        [SerializeField] int itemPickUpID; // ���忡 ������ �����۵��� �ĺ� ��ȣ
        [SerializeField] bool hasBeenLooted; // �÷��̾ ȸ���ߴ��� ���ߴ��� ����
        public Item item;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            // ���� ���� ĳ���� ���̺� ��������
            // ���忡�� ȸ���� ������ ��Ͽ� �� �������� ���ٸ�
            // �� �������� ȸ���� ���� ���� ��
            // ���� ȸ�� ���� �ʾҴٰ� ����ؾ� ��
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, false);
            }

            hasBeenLooted = WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[itemPickUpID];

            if (hasBeenLooted) {
                gameObject.SetActive(false);
            }
        }
        
        public override void Interact(PlayerManager playerManager) {
            base.Interact(playerManager); // �θ��� Interact�Լ��� ȣ���Ѵ�.

            // ĳ���� �����Ϳ� �� �������� �÷��̾�� ȸ���Ǿ������� �˷� ���� �̹� ȸ���� �������̶�� �ٽ� ���忡 �������� �ʵ��� ��

            // ���� ���� ĳ���� ���̺� �������� ���� ���忡�� ȸ���� ������ ��Ͽ� �� ���� �������� �ĺ� ��ȣ�� �����Ѵٸ�
            if (WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpID)) {
                // �ش� �ĺ���ȣ�� key ������ �ϴ� �׸��� ����
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Remove(itemPickUpID);
            }

            // �ش� �ĺ���ȣ�� key ������ �ϴ� �׸��� value�� true �� �Է��Ͽ� �ٽ� �߰�
            WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpID, true);

            hasBeenLooted = true;

            // �÷��̾��� �κ��丮�� �������� �����Ѵ�
            PickUpItem(playerManager);
        }
        
        // ������ �ݱ�
        private void PickUpItem(PlayerManager playerManager) {
            PlayerInventoryManager playerInventory;
            PlayerLocomotionManager playerLocomotion;
            PlayerAnimatorManager animatorHandler;

            playerInventory = playerManager.GetComponent<PlayerInventoryManager>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero; // �÷��̾ �������� �ݴµ��� ����
            animatorHandler.PlayTargetAnimation("PickingUp", true);
            //playerInventory.weaponsInventory.Add(weapon);
            if (item is WeaponItem) playerInventory.weaponsInventory.Add(item as WeaponItem);
            else if (item is ConsumableItem) playerInventory.consumablesInventory.Add(item as ConsumableItem);

            // ���� �������� ���� �Ҷ��� ���� �������� �̸��� ǥ�õǵ����Ѵ�.
            UIManager.instance.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
            UIManager.instance.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
            UIManager.instance.itemInteractableGameObject.SetActive(true);
            
            Destroy(gameObject);
        }
    }
}
