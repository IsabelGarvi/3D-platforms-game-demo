using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEnemy : MonoBehaviour {

    private bool active_attack;
    private List<Collider> players = new List<Collider>();
    private float delay_attack,
        radius_attack;
    private PlayerController player_script;
    private GameObject player;
    //private const string EmissiveValue = "_EmissionScaleUI";
    public static int life;

    // Use this for initialization
    void Start() {
        radius_attack = 8;
        /* Access PlayerController script */
        player = GameObject.Find("Body");
        player_script = player.GetComponent<PlayerController>();
        life = 5;
    }

    // Update is called once per frame
    void Update() {
        mainAttack();
        checkLife();
    }

    private void mainAttack() {
        /* If the variable active_attack is true, GOD attacks */
        if (active_attack) {
            /* Get in the list every object that is in the area of attack */
            players = new List<Collider>(Physics.OverlapSphere(transform.position, radius_attack));
            /* For each object of the list, check the tag
             * if the tag is "Player", substract life and check
             * if life == 0, player dies, if not, reappear at last saved checkpoint
             */
            for (int i = 0; i < players.Count; i++) {
                if (players[i].tag == "Player") {
                    PlayerController.life--;
                    player_script.getLife(PlayerController.life);
                    if (PlayerController.life < 1) {
                        player_script.diePlayer();
                    } else {
                        player_script.changePosition(PlayerController.checkpoint);
                    }
                }
            }

        } else {
            /* Attack after 1s of inactivity */
            delay_attack += Time.deltaTime;
            if (delay_attack >= 1f) {
                delay_attack = 0;
                active_attack = true;

                /* After a second, call the function stopAttack so the player doesn't attack anymore
                 * Invoke calls a function after a time set (in seconds)
                 */
                Invoke("stopAttack", 1f);
            }
        }
    }

    private void stopAttack() {
        active_attack = false;
        players.Clear();
    }

    private void checkLife() {
        /* If GOD has no lives, load the menu scene */
        if (life <= 0) {
            SceneManager.LoadScene(1);
        }
    }

}
