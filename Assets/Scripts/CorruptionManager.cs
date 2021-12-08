using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    private List<GameObject> corruptedTiles = new List<GameObject>();
    private List<GameObject> frontCorruptedTiles;

    private Dictionary<GameObject, List<GameObject>> frontNeighborsDictionary = new Dictionary<GameObject, List<GameObject>>();

    private float timeElapsed = 0;
    

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
        frontCorruptedTiles = corruptedTiles.Where(tile => forwardCorruption(tile)).ToList();

        Debug.Log(Time.realtimeSinceStartup);
        float timeOnCorruptionCurve = Time.realtimeSinceStartup / (maxTimer * 60);
        Debug.Log(timeOnCorruptionCurve);
        Debug.Log(corruptionRate.Evaluate(timeOnCorruptionCurve));
        Debug.Log(debitRate / corruptionRate.Evaluate(timeOnCorruptionCurve));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                mouseRay.GetPoint(hit.distance);

                Vector3Int mapPos = map.WorldToCell(hit.point);
                Debug.Log(mapPos);

                //Vector3Int gridPos = grid.WorldToCell(hit.point);
                //Debug.Log(gridPos);

                Vector3 pos = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);
                Destroy(hit.transform.gameObject);
                Instantiate(corruptionTile, pos, Quaternion.identity, map.transform);
            }
        }

        timeElapsed += Time.deltaTime;
        float timeOnCorruptionCurve = Time.realtimeSinceStartup / (maxTimer * 60);
        //Debug.Log(timeElapsed);
        //Debug.Log(debitRate / corruptionRate.Evaluate(timeOnCorruptionCurve));
        if (timeElapsed >= debitRate / corruptionRate.Evaluate(timeOnCorruptionCurve))
        {
            spread();
            timeElapsed = 0;
        }
    }

    /*
     * AU SECOURS C'EST MOCHE
     * Il y a plein de problème à faire ça comme ça 
     * (seul point positif c'est plus rapide que parcourir toutes les tiles et comparer)
     * Check si on peut pas limiter le hit a seulement le sol et pas les batiments etc ...
     * */
    bool forwardCorruption(GameObject tile)
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
        int count = 0;
        List<GameObject> neigbors = new List<GameObject>();
        foreach (Vector3 pos in positions)
        {
            
            ray = new Ray(pos + originOffset, direction);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name != "Dry_Ground")
                {
                    neigbors.Add(hit.transform.gameObject);
                    count++;
                }
            }
            
        }
        
        if (count > 0)
        {
            frontNeighborsDictionary.Add(tile, neigbors);
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
        //Debug.Log(randomIndex);
        //Debug.Log(frontNeighborsDictionary.Count);
        GameObject randomKeyInDict = frontNeighborsDictionary.ElementAt(randomIndex).Key;

        int randomIndexValue = Random.Range(0, (int)randomValueInDict.Count);
        //Debug.Log(randomIndexValue);
        //Debug.Log(randomValueInDict.Count);

        GameObject tile = randomValueInDict.ElementAt(randomIndexValue);

        List<Vector3> positions = new List<Vector3>();
        positions.Add(tile.transform.position + new Vector3(1, 0, 0));
        positions.Add(tile.transform.position + new Vector3(-1, 0, 0));
        positions.Add(tile.transform.position + new Vector3(0, 0, 1));
        positions.Add(tile.transform.position + new Vector3(0, 0, -1));


        Vector3 originOffset = new Vector3(0, 1, 0);
        Vector3 direction = new Vector3(0, -1, 0);
        Ray ray;
        RaycastHit hit;
        foreach (Vector3 position in positions)
        {

            ray = new Ray(position + originOffset, direction);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "Dry_Ground")
                {
                    frontNeighborsDictionary[hit.transform.gameObject].Remove(tile);
                    if(frontNeighborsDictionary[hit.transform.gameObject].Count == 0)
                    {
                        frontNeighborsDictionary.Remove(hit.transform.gameObject);
                    }
                }
            }

        }



        Vector3 pos = new Vector3(Mathf.Floor(tile.transform.position.x) + 0.5f, 0, Mathf.Floor(tile.transform.position.z) + 0.5f);
        //randomValueInDict.RemoveAt(randomIndexValue);
        Destroy(tile);
        GameObject newTile = Instantiate(corruptionTile, pos, Quaternion.identity, map.transform);
        forwardCorruption(newTile);
        if (randomValueInDict.Count == 0)
        {
            frontNeighborsDictionary.Remove(randomKeyInDict);
        }
        else
        {
            frontNeighborsDictionary[randomKeyInDict] = randomValueInDict;
        }
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


