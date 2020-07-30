using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    /* Variables:
     * follow_speed so we know how fast the camera follows the player 
     * look_z so the camera keeps looking to the Z axis even if the player rotates
     * height so we know how high is the camera going to be
     * rotation_speed so we know how fast it rotates and we can create the delay
     */
    private float follow_speed, look_z, height, rotation_speed;
    /* Variable so the camera knows who's following
     * temp_rot: child of the camera so it rotates to it*/
    public GameObject target, temp_rot; 

	// Use this for initialization
	void Start () {
        follow_speed = 3.5f;
        look_z = 6;
        height = 3;
        rotation_speed = 2;
	}
	
	// Update is called once per frame
	void Update () {
        /* If there is no target, back to the beginning
         */
        if (!target) {
            return;
        }

        positionCamera();
        rotationCamera();
	}

    private void positionCamera() {
        /* Initial position: camera's position
         * Final position: player's position but adding the height and substracting the Z
         * Speed: follow_speed variable*/
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(target.transform.position.x, target.transform.position.y + height, target.transform.position.z - look_z),
            follow_speed * Time.deltaTime);
    }

    private void rotationCamera() {
        /* Easy way to do it
         * Not useful because it follows the player all the way, and we can't see the player move (no delay)
         * transform.LookAt(target.transform);
         */

        /* Initial position: camera's rotation
         * Final position: child rotation
         * Speed: rotation_speed variable
         */
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            temp_rot.transform.rotation,
            rotation_speed * Time.deltaTime);

    }
}
