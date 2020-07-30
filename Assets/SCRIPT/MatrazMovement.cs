using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrazMovement : MonoBehaviour {

    //private float time;
    //public PlayerController list;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /* Destroy the object after 2 seconds */
        Destroy(gameObject, 2);
    }

    private void OnTriggerEnter(Collider other) {

        switch (other.tag) {
            case "Enemy":
                /* If the object collides with an enemy, destroy the enemy */
                Destroy(other.gameObject);
                Destroy(gameObject);
                break;
            case "GOD":
                BossEnemy.life--;
                break;
        }
    }
}
