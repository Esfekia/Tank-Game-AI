using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void CollectHandler(GameObject crate);
    public event CollectHandler OnCollect;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Crate")
        {
            if (OnCollect != null)
            {
                // If we collided with something, we are going to let anybody who is listening know.
                OnCollect(other.gameObject);
            }
        }
    }
}
