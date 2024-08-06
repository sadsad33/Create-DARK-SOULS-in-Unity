using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class SpellDamageCollider : DamageCollider {
        public CharacterManager characterSpelledThis;
        public GameObject impactParticles;
        public GameObject projectileParticles;
        public GameObject muzzleParticles;

        bool hasCollided = false;
        CharacterManager spellTarget; // 목표물의 Stat
        Vector3 impactNormal; // impactParticles 회전축

        protected override void Awake() {
            base.Awake();
        }

        private void Start() {
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if (muzzleParticles) {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 0.5f);
            }
        }

        //private void OnCollisionEnter(Collision collision) {

        //    if (!hasCollided) {
        //        spellTarget = collision.transform.root.GetComponent<CharacterManager>();
        //        if (spellTarget != null && spellTarget.characterStatsManager.teamIDNumber != teamIDNumber) {
        //            TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
        //            takeDamageEffect.physicalDamage = physicalDamage;
        //            takeDamageEffect.fireDamage = fireDamage;
        //            takeDamageEffect.poiseDamage = poiseDamage;
        //            takeDamageEffect.contactPoint = contactPoint;
        //            takeDamageEffect.angleHitFrom = angleHitFrom;
        //            spellTarget.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
        //        }

        //        hasCollided = true;
        //        GameObject checkLocation = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));
        //        impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)); // Vector3.up 을 impactNoraml 에 대해 회전

        //        Destroy(projectileParticles);
        //        Destroy(impactParticles, 2f);
        //        Destroy(gameObject, 5f);
        //    }
        //}

        private void OnTriggerEnter(Collider other) {
            if (!hasCollided) {
                spellTarget = other.transform.root.GetComponent<CharacterManager>();
                if (spellTarget != null) {
                    if (spellTarget.characterStatsManager.teamIDNumber == teamIDNumber) return;
                    else {
                        Debug.Log(other.transform.gameObject.name + "과 충돌 ! ");
                        TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
                        takeDamageEffect.physicalDamage = physicalDamage;
                        takeDamageEffect.fireDamage = fireDamage;
                        takeDamageEffect.poiseDamage = poiseDamage;
                        takeDamageEffect.contactPoint = contactPoint;
                        takeDamageEffect.angleHitFrom = angleHitFrom;
                        spellTarget.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
                    }
                }

                hasCollided = true;
                GameObject checkLocation = Instantiate(impactParticles, transform.position, Quaternion.identity);
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)); // Vector3.up 을 impactNoraml 에 대해 회전

                Destroy(projectileParticles);
                Destroy(impactParticles, 2f);
                Destroy(gameObject, 5f);
            }
        }
    }
}