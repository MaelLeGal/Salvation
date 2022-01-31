using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

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

    public GameObject BuildingInfoPanel;
    Dictionary<string,GameObject> BuildingInfoPanelChildren = new Dictionary<string, GameObject>();

    private void Start()
    {
        _map = CorruptionManager.Map;
        _selection.z = -1; //no selection

        foreach (Transform child in BuildingInfoPanel.transform)
        {
            foreach (Transform childOfChild in child.transform)
            {
                BuildingInfoPanelChildren.Add(childOfChild.name, childOfChild.gameObject);
                
                foreach(Transform icon in childOfChild.transform)
                {
                    BuildingInfoPanelChildren.Add(icon.name, icon.gameObject);
                }
            }
        }
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
            Debug.LogWarning(name + " : Il m'est impossible de construire un bâtiment ici, aucune case n'est sélectionnée ! " + _selection);
            return;
        }

        // Check if the building is placable on this tile
        switch (_selectedTile.name)
        {
            default:
            case "Grass":
                if (!b.PlaceableOnGrass)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce bâtiment ici ! " + _selection);
                    return;
                }
                break;

            case "Neutral":
                if (!b.PlaceableOnNeutral)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce bâtiment ici ! " + _selection);
                    return;
                }
                break;

            case "Dry_Ground":
                if (!b.PlaceableOnDryGround)
                {
                    Debug.LogWarning(name + " : Il m'est impossible de construire ce bâtiment ici ! " + _selection);
                    return;
                }
                break;
        }

        // Check if the tile is already occupied
        if (_selectedTile.GetComponent<TileDataContainer>().isOccupied)
        {
            Debug.LogWarning(name + " : Il m'est impossible de construire un bâtiment ici, la case est déjà occupée ! " + _selection);
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

    public void DisplayPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }

    public void DisplayBuildingInfoPanel(string buildingName)
    {
        try
        {
            BuildingData b = JsonUtility.FromJson<BuildingData>(System.IO.File.ReadAllText("Assets/Resources/JSON/" + buildingName + ".json"));
            if (b.Name != null)
            {
                BuildingInfoPanelChildren["Name"].SetActive(true);
                BuildingInfoPanelChildren["Name"].GetComponent<TextMeshProUGUI>().text = b.Name;
            }

            if (b.ConstructionCosts.GetEnumerator().MoveNext())
            {
                foreach (Building.Costs.Cost cost in b.ConstructionCosts)
                {
                    BuildingInfoPanelChildren["ConstructionCosts"].SetActive(true);
                    BuildingInfoPanelChildren["ConstructionCosts"].GetComponent<TextMeshProUGUI>().text = "Construction Cost : " + cost.Price.ToString();
                    Debug.Log(Resources.Load<Sprite>("Sprite/person-logo"));
                    switch (cost.Resource)
                    {
                        case Resource.ResourceType.People:
                            BuildingInfoPanelChildren["ConstructionCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/person-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Energy:
                            BuildingInfoPanelChildren["ConstructionCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/lightning-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Food:
                            BuildingInfoPanelChildren["ConstructionCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/fork_logo", typeof(Sprite));
                            break;
                    }
                    
                }
            }
            else
            {
                BuildingInfoPanelChildren["ConstructionCosts"].SetActive(false);
            }

            if (b.TickCosts.GetEnumerator().MoveNext())
            {
                foreach (Building.Costs.Cost cost in b.TickCosts)
                {
                    BuildingInfoPanelChildren["TickCosts"].SetActive(true);
                    BuildingInfoPanelChildren["TickCosts"].GetComponent<TextMeshProUGUI>().text = "Tick Cost : " + cost.Price.ToString();

                    switch (cost.Resource)
                    {
                        case Resource.ResourceType.People:
                            BuildingInfoPanelChildren["TickCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/person-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Energy:
                            BuildingInfoPanelChildren["TickCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/lightning-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Food:
                            BuildingInfoPanelChildren["TickCostsIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/fork_logo", typeof(Sprite));
                            break;
                    }
                }
            }
            else
            {
                BuildingInfoPanelChildren["TickCosts"].SetActive(false);
            }

            if (b.TickProductions.GetEnumerator().MoveNext())
            {
                foreach (Building.Costs.Cost cost in b.TickProductions)
                {
                    BuildingInfoPanelChildren["TickProductions"].SetActive(true);
                    BuildingInfoPanelChildren["TickProductions"].GetComponent<TextMeshProUGUI>().text = "Tick Production : " + cost.Price.ToString();

                    switch (cost.Resource)
                    {
                        case Resource.ResourceType.People:
                            BuildingInfoPanelChildren["TickProductionIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/person-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Energy:
                            BuildingInfoPanelChildren["TickProductionIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/lightning-logo", typeof(Sprite));
                            break;
                        case Resource.ResourceType.Food:
                            BuildingInfoPanelChildren["TickProductionIcon"].GetComponent<Image>().sprite = (Sprite)Resources.Load("Sprite/fork_logo", typeof(Sprite));
                            break;
                    }
                }
            }
            else
            {
                BuildingInfoPanelChildren["TickProductions"].SetActive(false);
            }

            if (b.Capacity != 0)
            {
                BuildingInfoPanelChildren["Capacity"].SetActive(true);
                BuildingInfoPanelChildren["Capacity"].GetComponent<TextMeshProUGUI>().text = "Capacity : " + b.Capacity;
            }
            else
            {
                BuildingInfoPanelChildren["Capacity"].SetActive(false);
            }

            if(b.Description != null)
            {
                BuildingInfoPanelChildren["Description"].SetActive(true);
                BuildingInfoPanelChildren["Description"].GetComponent<TextMeshProUGUI>().text = b.Description;
            }
            else
            {
                BuildingInfoPanelChildren["Description"].SetActive(false);
            }

            BuildingInfoPanel.SetActive(!BuildingInfoPanel.activeSelf);

        }
        catch
        {
            Debug.Log("No File found");
        }
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
