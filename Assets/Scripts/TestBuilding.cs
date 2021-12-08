using UnityEngine;
using UnityEditor;

public class TestBuilding : Building
{

    // DEBUG
    private void OnEnable()
    { 
        Construct();
    }

    // DEBUG
    private void OnDisable()
    {
        Disable();
    }

}
