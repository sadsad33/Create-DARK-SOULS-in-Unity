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
        new SphereCollider collider;
        CharacterStatsManager spellTarget; // 목표물의 Stat
        Vector3 impactNormal; // impactParticles 회전축

        private new void Awake() {
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
                spellTarget = collision.transform.GetComponent<CharacterStatsManager>();
                if (spellTarget != null && spellTarget.teamIDNumber != teamIDNumber) {
                    spellTarget.TakeDamage(0, fireDamage, currentDamageAnimation);
                }
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)); // Vector3.up 을 impactNoraml 에 대해 회전
                
                Destroy(projectileParticles);
                Destroy(impactParticles, 2f);
                Destroy(gameObject, 5f);
            }
        }
    }
}