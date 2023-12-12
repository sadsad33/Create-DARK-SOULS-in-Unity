using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class SpellDamageCollider : DamageCollider {
        public GameObject impactParticles;
        public GameObject projectileParticles;
        public GameObject muzzleParticles;

        bool hasCollided = false;
        Rigidbody rigidBody;
        SphereCollider collider;
        CharacterStats spellTarget; // ��ǥ���� Stat
        Vector3 impactNormal; // impactParticles ȸ����

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<SphereCollider>();
        }
        private void Start() {
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if (muzzleParticles) {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 0.5f);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (!hasCollided) {
                spellTarget = collision.transform.GetComponent<CharacterStats>();
                if (spellTarget != null) {
                    spellTarget.TakeDamage(currentWeaponDamage);
                }
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)); // Vector3.up �� impactNoraml �� ���� ȸ��
                Destroy(projectileParticles);
                Destroy(impactParticles, 2f);
                Destroy(gameObject, 5f);
            }
        }
    }
}