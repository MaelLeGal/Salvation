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

    [SerializeField]
    private Transform _selectionCube;

    [SerializeField]
    private Transform _hoverCube;

    [System.Serializable]
    public class ConstructEventArgs : System.EventArgs
    {
        public enum Type
        {
            None,
            Grass,
            Neutral,
            DryGround
        }
        [Tooltip("Tile type in which neighbors will be changed into (None : no changes)")]
        public Type type;

        [Tooltip("Radius of the changing tiles, around the building")]
        public float radius;

        [HideInInspector]
        public Vector3 position;
    }

    public static event System.EventHandler<ConstructEventArgs> OnConstruct;

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
        // Green hover selection preview
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                mouseRay.GetPoint(hit.distance);

                _tmpSelection = _map.WorldToCell(hit.point);
                _tmpSelectionWorld = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, 0, Mathf.Floor(hit.point.z) + 0.5f);

                // Displays the hover if not hovering the selection
                if (_tmpSelection != _selection)
                    _hoverCube.position = _tmpSelectionWorld;
                else
                    _hoverCube.localPosition = Vector3.zero;
            }
            else
            {
                // No hovered tile
                _hoverCube.localPosition = Vector3.zero;
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

                _selectionCube.position = _selectionWorld;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _selection.z = -1;
            _selectedTile = null;

            _selectionCube.localPosition = Vector3.zero;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            // Get the clicked building and disable it
        }
    }

    public void Construct(string filename)
    {
        BuildingData b = JsonUtility.FromJson<BuildingData>(System.IO.File.ReadAllText("Assets/Resources/JSON/" + filename + ".json"));

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
        GameObject go = Instantiate(Resources.Load<GameObject>("Buildings/" + b.Name), _selectionWorld + Vector3.up, Quaternion.identity, Tilemap_Building);
        var building = go.AddComponent<Building>();
        building.LoadData(b); //Load data from the JSON file into the building
        _selectedTile.GetComponent<TileDataContainer>().isOccupied = true;

        b.ConstructEvent.position = _selectionWorld;
        OnConstruct.Invoke(this, b.ConstructEvent);
    }
}
