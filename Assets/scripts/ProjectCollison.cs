using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectCollison : MonoBehaviour
{
    public Grid grid;
    // Start is called before the first frame update
    Vector3 start_pos;

    void Start()
    {
        this.start_pos = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
        // Destroy this if the bullet has travelled too far
        if(Vector3.Distance(this.transform.position, this.start_pos) >= 100)
        {
            Debug.Log("Distance " + Vector3.Distance(this.transform.position, this.start_pos));
            Character.canFire = true;
            
            Destroy(this.gameObject);
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        // this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
        // this.GetComponent<Renderer>().enabled = false;
        if(other.tag == "Player")
        {
            // should probably restrict angle
            Character.bullet += 1;
            Debug.Log("Bullet: " + Character.bullet);
            this.GetComponent<Renderer>().enabled = false;
            this.transform.position += new Vector3(0, 100, 0);
        }

        else
        {
            
            Destroy(this.gameObject);
        }

        Character.canFire = true;
        
        
    }


}
