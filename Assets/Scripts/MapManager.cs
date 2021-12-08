using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject corruptedTile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                mouseRay.GetPoint(hit.distance);

                Vector3Int mapPos = map.WorldToCell(hit.point);
                Debug.Log(mapPos);

                Vector3Int gridPos = grid.WorldToCell(hit.point);
                Debug.Log(gridPos);

                Vector3 pos = new Vector3(Mathf.Floor(hit.point.x)+0.5f, 0,Mathf.Floor(hit.point.z) + 0.5f);
                Destroy(hit.transform.gameObject);
                Instantiate(corruptedTile, pos, Quaternion.identity, map.transform);
            }
        }*/
    }
}
