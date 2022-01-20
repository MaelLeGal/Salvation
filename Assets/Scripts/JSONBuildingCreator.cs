using UnityEngine;

public class JSONBuildingCreator : MonoBehaviour
{
    public BuildingData BuildingData;

    public bool SaveToJSON;

    private void OnValidate()
    {
        if (SaveToJSON)
        {
            System.IO.File.WriteAllText("Assets/Resources/JSON/" + BuildingData.Name + ".json", JsonUtility.ToJson(BuildingData));
            Debug.Log("Assets/Resources/JSON/" + BuildingData.Name + ".json créé !");

            SaveToJSON = false;
        }
    }
}
