using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public Animator anim;

	// Use this for initialization
	void Start () {
		anim.SetBool("open",false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        anim.SetBool("open",true);        
    }

    private void OnTriggerExit(Collider other) {
        anim.SetBool("open",false);        
    }
}
