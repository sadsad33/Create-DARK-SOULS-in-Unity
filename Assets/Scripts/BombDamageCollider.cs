using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class BombDamageCollider : DamageCollider {
        [Header("Explosive Damage & Radius")]
        public int explosiveRadius = 1;
        public float contactDamage; // ���� ������
        public float fireExplosionDamage; // ���߽� ���� ������

        public Rigidbody bombRigidbody;
        private bool hasCollided = false; // ȭ������ ��ü�� �浹�ߴ��� ����
        public GameObject impactParticles;
        protected override void Awake() {
            damageCollider = GetComponent<Collider>();
            bombRigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision) {
            if (!hasCollided) {
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.identity);
                Explode();

                CharacterStatsManager character = collision.transform.GetComponent<CharacterStatsManager>();

                if (character != null && character.teamIDNumber != teamIDNumber) {
                    character.TakeDamage(0, contactDamage);
                }
                Destroy(impactParticles, 5f);
                Destroy(transform.parent.parent.gameObject);
            }
        }

        private void Explode() {
            Collider[] characters = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (Collider character in characters) {
                CharacterStatsManager characterStats = character.GetComponent<CharacterStatsManager>();
                if (characterStats != null && characterStats.teamIDNumber != teamIDNumber) {
                    characterStats.TakeDamage(0, fireExplosionDamage);
                }
            }
        }
    }
}
