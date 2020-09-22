using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectCollison : MonoBehaviour
{
    public Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void OnTriggerEnter(Collider other)
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
        this.GetComponent<Renderer>().enabled = false;
        
    }


}
