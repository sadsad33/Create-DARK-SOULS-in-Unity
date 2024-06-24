using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BombDamageCollider : DamageCollider {
        [Header("Explosive Damage & Radius")]
        public int explosiveRadius = 1;
        public float contactDamage; // 직격 데미지
        public float fireExplosionDamage; // 폭발시 광역 데미지

        public Rigidbody bombRigidbody;
        private bool hasCollided = false; // 화염병이 물체와 충돌했는지 여부
        public GameObject impactParticles;
        protected override void Awake() {
            damageCollider = GetComponent<Collider>();
            bombRigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision) {
            if (!hasCollided) {
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.identity);

                CharacterStatsManager character = collision.transform.root.GetComponent<CharacterStatsManager>();
                if (character != null && character.teamIDNumber != teamIDNumber) {
                    float directionHitFrom = (Vector3.SignedAngle(transform.forward, character.transform.forward, Vector3.up));
                    ChooseWhichDirectionDamageCameFrom(directionHitFrom);
                    character.TakeDamage(contactDamage, 0, currentDamageAnimation);
                    Debug.Log(currentDamageAnimation);
                }

                Explode();
                Destroy(impactParticles, 5f);
                Destroy(transform.parent.parent.gameObject);
            }
        }

        private void Explode() {
            Collider[] characters = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (Collider character in characters) {
                CharacterStatsManager characterStats = character.GetComponent<CharacterStatsManager>();
                if (characterStats != null && characterStats.teamIDNumber != teamIDNumber) {
                    float directionHitFrom = (Vector3.SignedAngle(transform.forward, character.transform.forward, Vector3.up));
                    ChooseWhichDirectionDamageCameFrom(directionHitFrom);
                    characterStats.TakeDamage(0, fireExplosionDamage, currentDamageAnimation);
                }
            }
        }

        protected override void ChooseWhichDirectionDamageCameFrom(float direction) {
            base.ChooseWhichDirectionDamageCameFrom(direction);
        }
    }
}
