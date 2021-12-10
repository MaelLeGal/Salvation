using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Linq;

public class CorruptionManager : MonoBehaviour
{
    public AnimationCurve corruptionRate;
    public int debitRate = 1;
    public int maxTimer = 15;

    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private GameObject corruptionTile;

    [SerializeField]
    private Material corruptionMaterial;

    private List<GameObject> corruptedTiles = new List<GameObject>();
    private List<GameObject> frontCorruptedTiles = new List<GameObject>();

    private Dictionary<GameObject, List<GameObject>> frontNeighborsDictionary = new Dictionary<GameObject, List<GameObject>>();

    private float timeElapsed = 0;
    private bool isPaused = false;

    [HideInInspector]
    public UnityEvent EndGameEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in map.transform)
        {
            if(child.gameObject.name == "Dry_Ground")
            {
                corruptedTiles.Add(child.gameObject);
            }
        }

        corruptedTiles.Sort(new ForwardCorruptionComparaison());
        corruptedTiles.Where(tile => forwardCorruption(tile)).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                Vector3 pos = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);
                hit.transform.gameObject.GetComponent<MeshRenderer>().material = corruptionMaterial;
                hit.transform.gameObject.name = "Dry_Ground";
            }
        }*/


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

    bool forwardCorruption(GameObject tile)
    {
        List<GameObject> neighbors = GetNeighbors(tile);

        List<GameObject> neighborsNotCorrupted = neighbors.Where(neigh => neigh.name != "Dry_Ground").ToList();

        if (neighborsNotCorrupted.Count > 0)
        {
            frontNeighborsDictionary.Add(tile, neighborsNotCorrupted);
            //frontCorruptedTiles.Add(tile);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void spread()
    {

        int randomIndex = Random.Range(0, (int)frontNeighborsDictionary.Count);
        List<GameObject> randomValueInDict = frontNeighborsDictionary.ElementAt(randomIndex).Value;
        GameObject randomKeyInDict = frontNeighborsDictionary.ElementAt(randomIndex).Key;
        
        int randomIndexValue = Random.Range(0, (int)randomValueInDict.Count);
        GameObject tile = randomValueInDict.ElementAt(randomIndexValue);

        List<GameObject> neighbors = GetNeighbors(tile);
        List<GameObject> neighborsNotCorrupted = neighbors.Where(neigh => neigh.name != "Dry_Ground").ToList();
        List<GameObject> neighborsCorrupted = neighbors.Where(neigh => neigh.name == "Dry_Ground").ToList();

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

        forwardCorruption(tile);
    }

    /*
     * Get the neighboring tiles
     * Cast a ray to each neighbor position to get the GameObject
     * param GameObject tile : The tile from which we cant the neighbors
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


