using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Linq;

public class CorruptionManager : MonoBehaviour
{
    /*
     * The curve of corruption rate.
     * */
    public AnimationCurve corruptionRate;

    /*
     * The number of second on which we base the spreading rate
     * Example : With an evaluation of the corruption rate at time T of 0.1 and a debit rate of 5sec, the time between each spread is around 50sec
     * same example but with a debit rate of 1sec, the time between each spread is around 10sec
     * */
    public int debitRate = 1;

    /*
     * The duration time of the game in minute
     * */
    public float maxTimer = 15;

    /*
     * The tilemap which contains the ground tiles
     * */
    [SerializeField]
    private Tilemap map;
    public Tilemap Map { get => map; }

    /*
     * The material used to represent the corruption;
     * */
    [SerializeField]
    private Material corruptionMaterial;

    /*
     * A list of corrupted tiles;
     * */
    private List<GameObject> corruptedTiles = new List<GameObject>();

    /*
     * A dictionnary of the corrupted tiles in the front (adjacent to non corrupted tiles) and their neighbors
     * */
    private Dictionary<GameObject, List<GameObject>> frontNeighborsDictionary = new Dictionary<GameObject, List<GameObject>>();

    /*
     * The time elapsed since the last spread
     * */
    private float timeElapsed = 0;

    /*
     * A boolean to check if the game is paused or not
     * */
    private bool isPaused = false;

    /*
     * An Event from the GameManager
     * Used to call Invoke and end the game
     * */
    [HideInInspector]
    public UnityEvent EndGameEvent;
    

    /*
     * Get all the corrupted tiles from the map
     * Sort the tiles and initialise the dictionnary with the forwardCorruption Method
     * */
    void Start()
    {
        foreach(Transform child in map.transform)
        {
            if(child.gameObject.GetComponent<TileDataContainer>().isCorrupted == true)
            {
                corruptedTiles.Add(child.gameObject);
            }
        }

        corruptedTiles.Sort(new ForwardCorruptionComparaison());
        corruptedTiles.Where(tile => forwardCorruption(tile)).ToList();
    }

    /*
     * Spread the corruption at intervals by using the corruptionRate curve
     * If the corruption can not spread further, pause the game and call the EndGameEvent
     * */
    void Update()
    {
        if (frontNeighborsDictionary.Count > 0)
        {
            timeElapsed += Time.deltaTime;
            float timeOnCorruptionCurve = Time.realtimeSinceStartup / (maxTimer * 60);
            if (timeElapsed >= debitRate / corruptionRate.Evaluate(timeOnCorruptionCurve))
            {
                spread();
                timeElapsed = 0;
            }
        }
        else if(frontNeighborsDictionary.Count <= 0 && !isPaused)
        {
            EndGameEvent.Invoke();
            isPaused = !isPaused;
        }
    }

    /*
     * Check if the tile is a front tile or not(adjacent to non corrupted tiles)
     * Add the tile to the dictionnary if it has at least one neighbor not corrupted and return true
     * if not, return false has it is not a front tile
     * param GameObject tile : The tile from which we want to check if it is a front tile or not
     * return bool isFront : true if the tile is a front tile, false otherwise
     * */
    bool forwardCorruption(GameObject tile)
    {
        List<GameObject> neighbors = GetNeighbors(tile);

        List<GameObject> neighborsNotCorrupted = neighbors.Where(neigh => neigh.GetComponent<TileDataContainer>().isCorrupted == false).ToList();

        if (neighborsNotCorrupted.Count > 0)
        {
            frontNeighborsDictionary.Add(tile, neighborsNotCorrupted);
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
     * Change a random tile from the non-corrupted neigbors of a random front tile into a corrupted tile.
     * */
    private void spread()
    {

        int randomIndex = Random.Range(0, (int)frontNeighborsDictionary.Count);
        List<GameObject> randomValueInDict = frontNeighborsDictionary.ElementAt(randomIndex).Value;
        GameObject randomKeyInDict = frontNeighborsDictionary.ElementAt(randomIndex).Key;
        
        int randomIndexValue = Random.Range(0, (int)randomValueInDict.Count);
        GameObject tile = randomValueInDict.ElementAt(randomIndexValue);

        List<GameObject> neighbors = GetNeighbors(tile);
        List<GameObject> neighborsNotCorrupted = neighbors.Where(neigh => neigh.GetComponent<TileDataContainer>().isCorrupted == false).ToList();
        List<GameObject> neighborsCorrupted = neighbors.Where(neigh => neigh.GetComponent<TileDataContainer>().isCorrupted == true).ToList();

        foreach(GameObject neigh in neighborsCorrupted)
        {
            frontNeighborsDictionary[neigh].Remove(tile);
            if (frontNeighborsDictionary[neigh].Count == 0)
            {
                frontNeighborsDictionary.Remove(neigh);
            }
        }

        tile.GetComponent<MeshRenderer>().material = corruptionMaterial;
        tile.name = "Dry_Ground";
        tile.GetComponent<TileDataContainer>().isCorrupted = true;
        tile.GetComponent<TileDataContainer>().type = 1;

        forwardCorruption(tile);
    }

    /*
     * Get the neighboring tiles
     * Cast a ray to each neighbor position to get the GameObject
     * param GameObject tile : The tile from which we want the neighbors
     * return List<GameObject> neighbors : The list of neighboring tiles
     * */
    private List<GameObject> GetNeighbors(GameObject tile)
    {
        List<Vector3> positions = new List<Vector3>();
        positions.Add(tile.transform.position + new Vector3(1, 0, 0));
        positions.Add(tile.transform.position + new Vector3(-1, 0, 0));
        positions.Add(tile.transform.position + new Vector3(0, 0, 1));
        positions.Add(tile.transform.position + new Vector3(0, 0, -1));


        Vector3 originOffset = new Vector3(0, 1, 0);
        Vector3 direction = new Vector3(0, -1, 0);
        Ray ray;
        RaycastHit hit;
        List<GameObject> neigbors = new List<GameObject>();
        foreach (Vector3 pos in positions)
        {

            ray = new Ray(pos + originOffset, direction);

            if (Physics.Raycast(ray, out hit))
            {
                neigbors.Add(hit.transform.gameObject);
            }

        }

        return neigbors;
    }

}

/*
 * Definition of a Comparer for the first check of front tiles
 * */
public class ForwardCorruptionComparaison : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        if(x.transform.position.z > y.transform.position.z)
        {
            return -1;
        }
        else if(x.transform.position.z == y.transform.position.z)
        {
            return 0;
        }
        else
        {
            return 1;
        }
        
    }

}


