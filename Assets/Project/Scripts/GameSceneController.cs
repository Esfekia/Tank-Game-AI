using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public MovableObject player;
    public AStar aStar;

        
    // Start is called before the first frame update
    void Start()
    {
        List<Node> path = aStar.FindPath();
        player.Move(path);
        
    }


}
