using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{
    public static int bullet = 0;
    public Camera playerCam;
    public GameObject projectile;
    public static bool canFire = true;
    public static bool gameOver = false;
    // Start is called before the first frame update
    private GameObject fpscontroller;
    private GameObject fpschar;
    private UnityEngine.UI.Text gamestate;
    private UnityEngine.UI.Text bulletCount;
    void Start()
    {
        fpscontroller = this.transform.GetChild(0).gameObject;
        fpschar = fpscontroller.transform.GetChild(0).gameObject;
        gamestate = GameObject.Find("GameState").GetComponent<UnityEngine.UI.Text>();
        gamestate.text = "";
        bulletCount = GameObject.Find("BulletCount").GetComponent<UnityEngine.UI.Text>();
        bulletCount.text = "" + 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        bulletCount.text = "" + bullet;
        if(Input.GetKeyDown(KeyCode.Mouse0) && canFire && bullet > 0)
        {
            
            GameObject bullet = Instantiate(projectile, playerCam.transform.position + playerCam.transform.forward, Quaternion.identity) as GameObject;

            // bullet.GetComponent<Rigidbody>().AddForce(new Vector3(Mathf.Cos(angle_y) * Mathf.Cos(angle_x), 
            // Mathf.Sin(-angle_x), Mathf.Sin(angle_y) * Mathf.Cos(angle_x)) * 1000);
            bullet.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * 4000);
            
            canFire = false;
            Character.bullet -= 1;
            Grid.proCount -= 1;
            if(Character.bullet <= 0 && Grid.proCount <= 0)
            {
                gameOver = true;
            }
        }


        // If the player falls, the game is over
        if(fpscontroller.transform.position.y < -5)
        {
            gameOver = true;
        }

        if(gameOver)
        {
             fpscontroller.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
             gamestate.text = "Game Over! You lost";
        }
    }

    
}
