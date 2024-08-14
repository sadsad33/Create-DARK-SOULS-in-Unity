using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerInteractionManager : MonoBehaviour {

        [SerializeField]
        InteractableUI interactableUI;
        //LayerMask interactableLayer;
        PlayerManager player;

        public NPCScript[] currentDialog;
        private float turnPageTimer;
        private readonly float turnPageTime = 10f;
        private int currentPageIndex;
        public Interactable currentInteractable;

        private void Awake() {
            player = transform.root.GetComponent<PlayerManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            //interactableLayer = 1 << 0 | 1 << 9;
        }

        private void OnTriggerStay(Collider other) {
            if (player.isInteracting || player.isInConversation || player.isClimbing || player.isAtBonfire) return;
            if (other.CompareTag("Interactable")) {
                currentInteractable = other.GetComponent<Interactable>();
                SetInteractionUI();
            } else if (other.CompareTag("Character")) {
                AICharacterManager character;
                character = other.GetComponent<CharacterManager>() as AICharacterManager;
                if (character.canTalk) {
                    Debug.Log(character.transform.root.gameObject);
                    currentInteractable = other.GetComponent<Interactable>();
                    //    if (currentInteractable != null) {
                    //        if (!UIManager.instance.itemInteractableGameObject.activeSelf) {
                    //            StartInteraction(currentInteractable, other, false);
                    //        } else if (player.inputHandler.a_Input) {
                    //            UIManager.instance.itemInteractableGameObject.SetActive(false);
                    //        }
                    //    }
                } else currentInteractable = null;                
                SetInteractionUI();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Interactable")) currentInteractable = null;
            if (UIManager.instance.interactableUIGameObject != null) {
                UIManager.instance.interactableUIGameObject.SetActive(false);
            }
        }

        private void SetInteractionUI() {
            if (currentInteractable != null) {
                string interactableText = currentInteractable.interactableText;
                interactableUI.interactableText.text = interactableText;
                UIManager.instance.interactableUIGameObject.SetActive(true);
            }
        }

        public void TryInteraction() {
            if (player.isInteracting || player.isInConversation || player.isClimbing || player.isAtBonfire) return;
            if (player.inputHandler.a_Input) {
                if (currentInteractable != null) {
                    currentInteractable.GetComponent<Interactable>().Interact(player);
                    UIManager.instance.interactableUIGameObject.SetActive(false);
                } else {
                    if (UIManager.instance.interactableUIGameObject != null)
                        UIManager.instance.interactableUIGameObject.SetActive(false);
                    if (UIManager.instance.itemInteractableGameObject != null)
                        UIManager.instance.itemInteractableGameObject.SetActive(false);
                }
            }
        }


        public void HandleConversation() {
            if (player.isInConversation) {
                float distance = Vector3.Distance(currentInteractable.transform.position, transform.position);
                if (currentPageIndex <= currentDialog.Length - 1) {
                    PrintDialog();
                    if (distance >= 3f) FinishConversation();
                } else if (turnPageTimer <= 0) {
                    NPCManager npc = currentInteractable.GetComponent<NPCManager>();
                    npc.interactCount++;
                    FinishConversation();
                }
            }
        }

        public void LeaveBonfire() {
            player.isAtBonfire = false;
            player.playerAnimatorManager.PlayTargetAnimation("Bonfire_End", true);
        }

        private void FinishConversation() {
            player.isInConversation = false;
            currentPageIndex = 0;
            UIManager.instance.dialogUI.SetActive(false);
        }

        private void PrintDialog() {
            if (!UIManager.instance.dialogUI.activeSelf) UIManager.instance.dialogUI.SetActive(true);
            if (turnPageTimer == 0)
                turnPageTimer = turnPageTime;
            interactableUI.dialogText.text = currentDialog[currentPageIndex].script;
            HandleTurnPageTimer();
            if (turnPageTimer == 0 && currentPageIndex < currentDialog.Length)
                currentPageIndex++;
        }

        private void HandleTurnPageTimer() {
            if (turnPageTimer <= 0) turnPageTimer = 0;
            else {
                turnPageTimer -= Time.deltaTime;
                if (player.inputHandler.a_Input) turnPageTimer -= turnPageTime;
            }
        }

        public void InteractionAtPosition(string animation, Transform playerStandingPosition) {
            player.characterWeaponSlotManager.rightHandSlot.UnloadWeapon();
            player.characterWeaponSlotManager.leftHandSlot.UnloadWeapon();
            //player.playerLocomotionManager.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //player.characterController.Move(Vector3.zero);
            player.transform.position = playerStandingPosition.position;
            player.playerAnimatorManager.PlayTargetAnimation(animation, true);
        }


        public void PassThroughFogWallInteraction(Transform fogWallEntrance) {
            player.playerLocomotionManager.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Vector3 rotationDirection = fogWallEntrance.transform.forward;
            Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = turnRotation;

            player.playerAnimatorManager.PlayTargetAnimation("PassThroughFog", true);
        }
    }
}
