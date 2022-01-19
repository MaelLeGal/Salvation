using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    private Tilemap _map;
    private Vector3Int _tmpSelection;
    private Vector3 _tmpSelectionWorld;
    private GameObject _selectedTile;
    private Vector3Int _selection; // tile position for the selection (_selection.z == -1 if no selection)
    private Vector3 _selectionWorld; // world position for the selected tile

    public CorruptionManager CorruptionManager;
    public Transform Tilemap_Building;
    public GameObject TestBuildingPrefab;
    public GameObject TableSacrificePrefab;

    private void Start()
    {
        _map = CorruptionManager.Map;
        _selection.z = -1; //no selection
    }

    private void Update()
    {
        // Green selection preview
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                mouseRay.GetPoint(hit.distance);

                _tmpSelection = _map.WorldToCell(hit.point);
                _tmpSelectionWorld = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                mouseRay.GetPoint(hit.distance);

                _selection = _map.WorldToCell(hit.point);
                _selectedTile = hit.collider.gameObject;
                _selectionWorld = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _selection.z = -1;
            _selectedTile = null;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            // Get the clicked building and disable it
        }
    }

    public void Construct(GameObject asset)
    {
        Building b = asset.GetComponent<Building>();

        // No currently selected tile
        if (_selection.z == -1)
        {
            Debug.LogWarning(name + " : Il m'est impossible de construire un b�timent ici, aucune case n'est s�lectionn�e ! " + _selection);
            return;
        }

        // Check if the building is placable on this tile
        switch (_selectedTile.name)
        {
            default:
            case "Grass":
                if (!b.PlaceableOnGrass)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce b�timent ici ! " + _selection);
                    return;
                }
                break;

            case "Neutral":
                if (!b.PlaceableOnNeutral)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce b�timent ici ! " + _selection);
                    return;
                }
                break;

            case "Dry_Ground":
                if (!b.PlaceableOnDryGround)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce b�timent ici ! " + _selection);
                    return;
                }
                break;
        }

        // Check if the tile is already occupied
        if (_selectedTile.GetComponent<TileDataContainer>().isOccupied)
        {
            Debug.LogWarning(name + " : Il m'est impossible de construire un b�timent ici, la case est d�j� occup�e ! " + _selection);
            return;
        }

        // If every check passed, the building can be built
        Instantiate(asset, _selectionWorld + Vector3.up, Quaternion.identity, Tilemap_Building);
        _selectedTile.GetComponent<TileDataContainer>().isOccupied = true;
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
