using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterLocomotionManager : MonoBehaviour {
        CharacterManager character;

        public Vector3 moveDirection;
        public LayerMask groundLayer;

        // 다른 클라이언트에서 내 오브젝트의 이동 애니메이션을 제어하기 위한 변수들
        [Header("Animator Movement Values")]
        public float verticalMovement;
        public float horizontalMovement;
        public float movementAmount;

        [Header("Gravitiy Settings")]
        public float inAirTimer;
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity; // 땅에 착지해있는 상태 동안 받는 힘
        [SerializeField] protected float fallStartYVelocity; // 떨어지기 시작하면 받게되는 힘(시간에 따라 증가함)
        [SerializeField] protected float gravityForce; // 중력
        [SerializeField] protected float groundCheckSphereRadius; // 착지 체크
        protected bool fallingVelocitySet = false;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start() {

        }

        protected virtual void Update() {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            character.animator.SetBool("isGrounded", character.isGrounded);
            HandleGroundCheck();
        }

        public virtual void HandleGroundCheck() {
            if (character.isClimbing) return; 
            if (character.isGrounded) {
                //if (yVelocity.y < groundedYVelocity) { 
                inAirTimer = 0;
                fallingVelocitySet = false;
                yVelocity.y = groundedYVelocity;
                //}
            } else {
                if (!fallingVelocitySet) {
                    fallingVelocitySet = true;
                    yVelocity.y = fallStartYVelocity;
                }
                inAirTimer += Time.deltaTime;
                yVelocity.y += gravityForce * Time.deltaTime;
            }
            character.animator.SetFloat("inAirTimer", inAirTimer);
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
        }
    }
}
