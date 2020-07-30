using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    private float time;
    private int scene;

	// Use this for initialization
	void Start () {
        scene = SceneManager.GetActiveScene().buildIndex;
    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        if (time > 2) {
            switch (scene) {
                case 0:
                    SceneManager.LoadScene(1);
                    break;
                case 3:
                    SceneManager.LoadScene(1);
                    break;
            }
        }		
	}
}
