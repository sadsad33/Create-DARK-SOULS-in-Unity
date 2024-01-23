using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class DamagePlayer : MonoBehaviour {
        private float damage = 25;
        private void OnTriggerEnter(Collider other) {
            PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();
            if (playerStats != null) {
                playerStats.TakeDamage(damage, 0);
            }
        }
    }
}
