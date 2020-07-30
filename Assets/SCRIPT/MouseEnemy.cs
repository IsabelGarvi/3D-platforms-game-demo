using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEnemy : MonoBehaviour {

    /* Need to know where are the points
     */
    public List<GameObject> points;
    /* Where is the enemy now? 
     */
    private int current_point;
    /* Enemy speed
     */
    private float speed;
    private Vector3 new_dir /* point where the enemy is going */;

	// Use this for initialization
	void Start () {
		speed = 10;
	}
	
	// Update is called once per frame
	void Update () {
        moveMouse();	
        rotateEnemy();
	}

    private void moveMouse() {

        /* Move the enemy from its position to the current point position
         */
        transform.position = Vector3.MoveTowards(transform.position, 
            points[current_point].transform.position, 
            speed * Time.deltaTime);

        /* It goes from one point to the next. Once it has gone through all of them,
         * go back to the first one (position 0 of the list)
         */
        if(transform.position == points[current_point].transform.position) {
            current_point++;
            if(current_point >= points.Count) {
                current_point = 0;
            }
        }
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
}
