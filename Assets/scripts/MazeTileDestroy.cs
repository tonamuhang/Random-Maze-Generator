using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTileDestroy : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider projectile)
    {
        if(projectile.gameObject.tag == "projectile")
        {
            int x = (int)this.transform.position.x/10;
            int z = (int)this.transform.position.z/10;
            Grid.grid[x, z] = null;
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
}
