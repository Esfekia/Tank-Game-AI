using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public float speed = 0.8f;
    
    private List<Node> currentPath;
    private Node targetNode;


    // Start is called before the first frame update
    void Start()
    {
        currentPath = new List<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        // Determine the target node.
        if (targetNode == null && currentPath.Count > 0)
        {
            targetNode = currentPath[0];
            currentPath.Remove(targetNode);

        }
        
        // Move towards the target node.
        if (targetNode != null)
        {
            Vector3 direction = (targetNode.value.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // When close enough force players position to be the node position.
            // Then clear targetNode so that a new loop can start above.
            if (Vector3.Distance(transform.position, targetNode.value.transform.position) < 0.01f)
            {
                transform.position = targetNode.value.transform.position;
                targetNode = null;
            }

        }
    }    
    public void Move(List<Node> path)
    {
        foreach (Node node in path)
        {
            currentPath.Add(node);
        }
    }
}
