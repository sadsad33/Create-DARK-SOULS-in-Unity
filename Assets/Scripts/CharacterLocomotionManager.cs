using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterLocomotionManager : MonoBehaviour {
        CharacterManager character;
        public Vector3 moveDirection;
        public LayerMask groundLayer;

        [Header("Gravitiy Settings")]
        public float inAirTimer;
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity = -20; // 땅에 착지해있는 상태 동안 받는 힘
        [SerializeField] protected float fallStartYVelocity = -7; // 떨어지기 시작하면 받게되는 힘(시간에 따라 증가함)
        [SerializeField] protected float gravityForce = -25;
        [SerializeField] protected float groundCheckSphereRadius;
        protected bool fallingVelocitySet = false;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start() {
        }

        protected virtual void Update() {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            HandleGroundCheck();
        }

        public virtual void HandleGroundCheck() {
            if (character.isGrounded) {
                if (yVelocity.y < 0) {
                    inAirTimer = 0;
                    fallingVelocitySet = false;
                    yVelocity.y = groundedYVelocity;
                }
            } else {
                if (!fallingVelocitySet) {
                    fallingVelocitySet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                yVelocity.y += gravityForce * Time.deltaTime;
            }
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }
}
