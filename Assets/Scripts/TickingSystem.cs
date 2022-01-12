using System.Collections.Generic;
using UnityEngine;

public sealed class TickingSystem : MonoBehaviour
{

    private static TickingSystem _instance;
    private float _tickingClock;
    private List<Building> _buildings;

    [Range(1, 5)]
    public int SecondsPerTick = 5;

    private TickingSystem() { }

    public static TickingSystem GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _tickingClock = Time.time;
        _buildings = new List<Building>();
    }

    // Called each x seconds, manages resources
    public void Tick()
    {
        // Manages resources production and loss
        foreach (Building b in _buildings)
            b.Tick();

        //Debug.Log("TS : Tick @" + Time.time);
    }

    private void Update()
    {
        // Manages ticking each x seconds
        if (Time.time >= _tickingClock + SecondsPerTick)
        {
            Tick();

            _tickingClock = Time.time;
        }
    }

    public void AddBuilding(Building b)
    {
        _buildings.Add(b);
        Debug.Log("TS : Got a new building !");
    }

    public void RemoveBuilding()
    {
        // To do
    }

}
