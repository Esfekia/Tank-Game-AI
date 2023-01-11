using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public float gameDuration = 30.0f;    
    public float maxSpawnInterval = 2.0f;
    public float minSpawnInterval = 0.5f;
    public float crateLifeTime = 10.0f;

    public GameObject cratePrefab;
    public GameObject crateContainer;
    public GameObject navTileContainer;

    public MovableObject player;
    public AStar aStar;

    private float gameTimer;
    private float spawnTimer;
    private List<NavTile> navigableTiles;

        
    // Start is called before the first frame update
    void Start()
    {        
        spawnTimer = maxSpawnInterval;

        //Build a list of all the navigable tiles
        navigableTiles = new List<NavTile>();
        foreach (NavTile tile in navTileContainer.GetComponentsInChildren<NavTile>())
        {
            if (tile.navigable)
            {
                navigableTiles.Add(tile);
            }
        }

    }

    private void Update()
    {
        // Game Timer Logic
        gameTimer += Time.deltaTime;
        
        //Ensuring the game difficulty always stays between zero and one.
        float difficulty = Mathf.Min(gameTimer / gameDuration, 1.0f);

        // Spawn Logic
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            // Difficulty Setting.
            // The higher the difficulty, the lower the spawn interval.
            float spawnInterval = maxSpawnInterval - (maxSpawnInterval - minSpawnInterval) * difficulty;
            spawnTimer = spawnInterval;
            
            Vector3 spawnPosition = navigableTiles[Random.Range(0, navigableTiles.Count)].transform.position;

            //Quaternion.identity is "just dont change the rotation"
            GameObject crateInstance = Instantiate(cratePrefab, spawnPosition, Quaternion.identity , crateContainer.transform);
            Destroy(crateInstance, crateLifeTime);
        }

        // Input Logic

        if (Input.GetMouseButtonDown(0))
        {
            // Get the screen space of where you clicked and convert to world space.
            Vector3 screenPosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // Perform raycast "inside" to identify what tile is selected
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<NavTile>() != null)
                    Debug.Log("You clicked on a tile at " + hit.collider.gameObject.transform.position);
                // Do something with the tile.
                // For example, move the player to that tile.
                List<Node> path = aStar.FindPath(player.gameObject, hit.collider.gameObject);
                player.Move(path);
                break;
            }
        }

        
    }


}
