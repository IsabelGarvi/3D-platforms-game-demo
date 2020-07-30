using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : MonoBehaviour {

    private CharacterController control;
    private Vector3 direction = Vector3.zero, new_dir /* point where the enemy is going */;
    private float gravity /* falls from sky */, 
        speed /* speed of the enemy */, 
        jump_speed /* how high the enemy jumps */, 
        jump_time /* how long from one jump to the next one */,
        dist_point /* area around a point */;

    public List<GameObject> points /* points where the enemy is patrolling */;
    private int current_point /* point where the enemy is at the current state */;

	// Use this for initialization
	void Start () {
        gravity = 20;
        speed = 7;
        jump_speed = 3;

        control = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        /* Different behaviour if it is grounded that if it is in the air */
        if (control.isGrounded) {
            jumpFrog();
        } else {
            directionFrogAir();
        }

        moveFrog();
	}

    private void jumpFrog() {
        /* Create point area to make it more realistic */
        dist_point = Vector3.Distance(transform.position, points[current_point].transform.position);

        /* When the enemy is in that area, moves to the next one */
        if (dist_point < 2) {
            current_point++;
            if (current_point >= points.Count) {
                current_point = 0;
            }
        }

        jump_time += Time.deltaTime;
        /* jumping rate */
        if (jump_time >= 0.7f) {
            direction.y = jump_speed;
            jump_time = 0;

            /* The enemy has to face the direction it is going */
            rotateEnemy();            
        }        
    }

    private void directionFrogAir() {
        /* Move towards the position of the next point */
        transform.position = Vector3.MoveTowards(transform.position,
                points[current_point].transform.position,
                speed * Time.deltaTime);
    }

    private void rotateEnemy() {
        /* where he is going */
        new_dir = points[current_point].transform.position - transform.position;
        new_dir.y = 0;

        /* if it changes its direction */
        if (new_dir != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(new_dir);
        }
    }

    private void moveFrog() {
        /* Need to check the gravity to move the player */
        direction = checkGravity(direction);
        /* Move the player */
        control.Move(direction * Time.deltaTime);
    }

    private Vector3 checkGravity(Vector3 dir) {
        /* Decrease the "y" component of the character controller, so it goes to the ground */
        dir.y -= gravity * Time.deltaTime;
        return dir;
    }
}
