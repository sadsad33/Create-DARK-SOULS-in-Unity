using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class LeverInteraction : Interactable {
        public Transform playerStandingPosition;
        public Animator animator;
        public GameObject activatableObject;
        protected override void Awake() {
            animator = GetComponent<Animator>();
        }

        public override void Interact(PlayerManager player) {
            base.Interact(player);
            Vector3 rotationDirection = -transform.forward;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, 300 * Time.deltaTime);
            player.transform.rotation = targetRotation;

            player.playerInteractionManager.InteractionAtPosition("Pull Lever", playerStandingPosition);
            animator.Play("Lever Pulled");
            StartCoroutine(Delay());
        }

        IEnumerator Delay() {
            yield return new WaitForSeconds(1.5f);
            activatableObject.GetComponent<ActivatableObject>().active = true;
            Destroy(this);
        }
    }
}