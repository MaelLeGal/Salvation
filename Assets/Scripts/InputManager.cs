using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    private Tilemap _map;
    private Vector3Int _tmpSelection;
    private Vector3 _tmpSelectionWorld;
    private Vector3Int _selection; // tile position for the selection (_selection.z == -1 if no selection)
    private Vector3 _selectionWorld; // world position for the selected tile

    public CorruptionManager CorruptionManager;
    public Transform Tilemap_Building;
    public GameObject TestBuildingPrefab;

    private void Start()
    {
        _map = CorruptionManager.Map;
        _selection.z = -1; //no selection
    }

    private void Update()
    {
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                mouseRay.GetPoint(hit.distance);

                Vector3Int mapPos = _map.WorldToCell(hit.point);

                Vector3 pos = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);

                _tmpSelection = mapPos;
                _tmpSelectionWorld = pos;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                mouseRay.GetPoint(hit.distance);

                Vector3Int mapPos = _map.WorldToCell(hit.point);
                Debug.Log(name + " " + mapPos);

                Vector3 pos = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);

                _selection = mapPos;
                _selectionWorld = pos;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _selection.z = -1;
        }
    }

    public void ConstructTestBuilding()
    {
        if (_selection.z == -1)
        {
            Debug.LogWarning(name + " : Il m'est impossible de construire un bâtiment ici, aucune case n'est sélectionnée ! " + _selection);
            return;
        }

        /*
         * if (tile is not green)
         * {
         *     Debug.LogWarning(name + " : Il m'est impossible de construire un bâtiment ici, la case n'est pas verte ! " + _selection);
         *     return;
         * }
         */

        Instantiate(TestBuildingPrefab, _selectionWorld + Vector3.up, Quaternion.identity, Tilemap_Building);
    }

    private void OnDrawGizmos()
    {
        // Displays a green box at the cursor location (if it is different than the selected tile location) => hover
        if (_tmpSelection.z != -1 && _tmpSelection != _selection)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawCube(_tmpSelectionWorld, Vector3.one * 1.1f);
        }

        // Displays a red box at the selected tile location
        if (_selection.z != -1)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawCube(_selectionWorld, Vector3.one * 1.1f);
        }
    }
}
