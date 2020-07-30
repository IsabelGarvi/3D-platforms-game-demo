using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private static float speed /* how fast is the player moving */,
        jump_speed /* how high the player is jumping */,
        radius_attack /* where the attack is valid */;
    //time_attack /* how long the attack is valid */;
    private float delay_attack /* small time delay between attacks */,
        delay_throw /* delay time between throws */,
        change_scene /* timer to change scene */;
    private float gravity;
    private static int jump_count /* store how many jumps the player has done */,
        total_jumps = 2 /* number of total consecutive jumps the player can do */,
        energy /* cups of coffee that the player has caught */;
    /* the player can only double jump when this variable is TRUE. It will be when the player defeats his first enemy
     * Static so it keeps its state through all de levels */
    static private bool active_jump;
    private bool active_attack /* to know if the player can make the attack */;

    private CharacterController control;
    private Vector3 direction = Vector3.zero /* direction where the player is going */;
    private List<Collider> enemies = new List<Collider>(); /* save which enemies are in the area of attack */
    private RaycastHit foot; /* ray that collisions */
    

    /* Need to access these variables from another script */
    public static int life = 3 /* number of lives the player has */;
    public static Vector3 checkpoint /* store the position of the checkpoint */;
    public GameObject matraz, teleport;
    public List<GameObject> matraces = new List<GameObject>(); /* save the new matraces */
    public Text lifeText, energyText /* For the UI, add the number of lives and energy */;
    //public Animator anim;

    // Use this for initialization
    void Start() {
        control = GetComponent<CharacterController>();
        /* Initialize this variables so I don't have to change them constantly in the UI 
         */
        gravity = 20;
        speed = 10;
        jump_speed = 8;
        radius_attack = 4;
        checkpoint = transform.position;

        /* If the player is in the menu scene, reset the life value */
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            life = 3;
        }

        /* If the player is not in the menu scene, get the life and energy on screen */
        if (SceneManager.GetActiveScene().buildIndex > 1) {
            getLife(life);
            getEnergy(energy);
        }
        
    }

    // Update is called once per frame
    void Update() {

        checkEnergy(energy);
        /* If the player is on the ground acts different that if it's on the air 
         * On the air gravity affects the player movements, on the ground this does not happen
         */
        if (control.isGrounded) {
            /* Reset the counter when the player touches ground */
            jump_count = 0;
            directionPlayerGround();
        } else {
            directionPlayerAir();
        }

        /* Need to rotate the player */
        rotatePlayer();

        /* LAST TO COMPUTE
         * Now we add every change we made on the other functions and move the player through the screen
         */

        movePlayer();

        systemAttack();
    }

    private void directionPlayerGround() {
        /* create a new direction vector according to where the player wants to go, depending on which keys are pressed
         * The player can press A or D to move to the sides (locally to the player)
         * The player can press W or S to move forward or backward
         * The player can press keys to move horizontally and vertically at the same time
         */
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        /* So it respects where the player is looking and move accordingly to that (no need in this project for now) 
         * direction = transform.TransformDirection(direction); 
         */
        direction *= speed;

        /* When the Space key in the keyboard is pressed (or the Y button in a joystick), the player has to jump
         * Increase the counter so it takes into account that the player jumps from the ground
         */
        if (Input.GetButtonDown("Jump")) {
            direction.y = jump_speed;
            jump_count++;
        }

    }

    private void directionPlayerAir() {
        direction = new Vector3(Input.GetAxis("Horizontal") * speed, direction.y, Input.GetAxis("Vertical") * speed);

        /* When the Space key in the keyboard is pressed (or the Y button in a joystick), the player has to jump
         * Increase the counter so we know how many jumps is the player doing and keeping it from doing more than total jumps
         */
        if (Input.GetButtonDown("Jump")) {
            /* Not necessaty the active_jump variable, so comment the if
            if (active_jump) { */
                if (jump_count < total_jumps) {
                    direction.y = jump_speed;
                    jump_count++;
                }
            //}
        }
    }

    private void rotatePlayer() {
        /* Store direction in a new variable */
        Vector3 new_direction = direction;
        /* No need for the player to rotate when it is looking down, so we fix the "y" axis */
        new_direction.y = 0;

        /* If the new direction is not (0,0,0) means that the player is moving somewhere, this way it won't look back forward
         * when it has stop moving
         */
        if (new_direction != Vector3.zero) {
            /* If the player is moving in the "x" or "z" axis */
            if (direction.x != 0 || direction.z != 0) {
                /* Rotate the player to the direction it is going */
                transform.rotation = Quaternion.LookRotation(new_direction);
            }
        }
    }

    private void movePlayer() {
        /* Need to check the gravity to move the player */
        direction = checkGravity(direction);
        /* Move the player */
        control.Move(direction * Time.deltaTime);
    }

    private void systemAttack() {
        /* THROW OBJECT */
        delay_throw += Time.deltaTime;
        /* If Left mouse button or right ctrl key is pressed, FIRE */
        if (Input.GetButtonDown("THROW")) {
            if (delay_throw > 1.5f) {
                /* Instantiate an object and move it formard */
                GameObject new_matraz = Instantiate(matraz,
                    transform.position,
                    Quaternion.Euler(0, 0, 0));
                new_matraz.transform.parent = gameObject.transform;
                this.transform.GetChild(7).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * 15;
                delay_throw = 0;
            }            
        }

        /* JUMP KILL */
        /* Need to know if the player is colliding with something
         * at a distance of 1.5f 
         * and going from the player in the down direction
         */
        if (Physics.Raycast(transform.position, Vector3.down, out foot, 2f)) {
            /* If the thing the player is colliding with has the tag "Enemy"
             * Destroy that thing and jump
             */
            if (foot.collider.gameObject.tag == "Enemy") {
                Destroy(foot.collider.gameObject);
                direction.y = jump_speed;
            }
        }

        /* RAY SPHERE */
        /* If the variable active_attack is true, the player can attack */
        if (active_attack) {
            /* If right mouse button or right shift key is pressed, player attacks with an area attack */
            if (Input.GetButtonDown("RAY")) {
                energy -= 5;
                /* Get in the list every enemy that is in the area of attack */
                enemies = new List<Collider>(Physics.OverlapSphere(transform.position, radius_attack));
                /* For each object of the list, check the tag
                 * if the tag is "Enemy", destroy that object
                 */
                for (int i = 0; i < enemies.Count; i++) {
                    if (enemies[i].tag == "Enemy" || enemies[i].tag == "GOD") {
                        Destroy(enemies[i].gameObject);
                        /* If the enemy is GOD, charge the menu scene */
                        if (enemies[i].tag == "GOD") {
                            Invoke("changeScene", 1);
                        }
                    }
                }                                
                /* After a second, call the function stopAttack so the player doesn't attack anymore
                     * Invoke calls a function after a time set (in seconds)
                     */
                Invoke("stopAttack", 1);                
            }

        } /* else {
            delay_attack += Time.deltaTime;

            /* If the left button of the mouse (0) is pressed and the time between attacks is greater than 0.5 seconds 
            if(Input.GetButtonDown("Fire1")) {
                if(delay_attack >= 0.5f) {
                    delay_attack = 0;
                    //active_attack = true; -> not going to need this to perform the great attack

                    /* After a second, call the function stopAttack so the player doesn't attack anymore
                     * Invoke calls a function after a time set (in seconds)
                     
                    Invoke("stopAttack", 1);
                }
            }
        }*/
    }

    private void changeScene() {
        SceneManager.LoadScene(1);
    }

    private void stopAttack() {
        active_attack = false;
        enemies.Clear();
    }

    private Vector3 checkGravity(Vector3 dir) {
        /* Decrease the "y" component of the character controller, so it goes to the ground */
        dir.y -= gravity * Time.deltaTime;
        return dir;
    }

    public void diePlayer() {

        Destroy(gameObject, 0.5f);

        SceneManager.LoadScene(3);      
    }

    public void changePosition(Vector3 checkpoint) {
        transform.position = checkpoint;
    }

    public void getLife(int life) {
        lifeText.text = "LIFE: " + life.ToString();
    }

    private void getEnergy(int energy) {
        energyText.text = "ENERGY: " + energy.ToString();
    }

    private void checkEnergy(int energy) {
        if (energy <= 0) {
            active_attack = false;
        } else {
            active_attack = true;
        }
    }

    private void OnTriggerEnter(Collider other) {

        switch (other.tag) {
            case "Enemy":
                /* If the trigger is an enemy, destroy the enemy, substract one life
                 * if the player still has lives left, reappear where the last saved checkpoint was
                 * if not, die
                 */
                life--;
                getLife(life);
                if (life >= 1) {
                    changePosition(checkpoint);
                } else {
                    diePlayer();
                }
                break;
            case "Checkpoint":
                /* If the trigger is a checkpoint, store its position so the player can reappear there
                 * if hit by enemy
                 */
                checkpoint = other.gameObject.transform.position;
                break;
            case "Coffee":
                /* If the trigger is a cup of coffee, increase the energy so the player can perform the
                 * great attack 
                 * Destroy the cup of coffee
                 */
                Destroy(other.gameObject, 0.5f);
                energy++;
                checkEnergy(energy);
                getEnergy(energy);                
                break;
            case "Reiki":
                /* If the trigger is the thing thrown by the hand, destroy the object
                 * Damage player
                 * if the player still has lives left, reappear where the last saved checkpoint was
                 * if not, die
                 */
                life--;
                getLife(life);
                if (life >= 1) {
                    changePosition(checkpoint);
                } else {
                    diePlayer();
                }
                break;
            case "World1":
                /* Load the first lever scene */
                SceneManager.LoadScene(2);
                break;
            case "Teleport":
                /* Teleport the player to another position */
                transform.position = teleport.transform.position;
                break;
            case "GOD":
                /* If the player gets touched by GOD, take life. If life <= 0, die */
                life--;
                getLife(life);
                if (life >= 1) {
                    changePosition(checkpoint);
                } else {
                    diePlayer();
                }
                break;
        }
    }
}
