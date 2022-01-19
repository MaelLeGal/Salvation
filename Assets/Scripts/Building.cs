using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    [System.Serializable]
    public class Costs : IEnumerable
    {

        [System.Serializable]
        public class Cost
        {
            [SerializeField]
            private Resource.ResourceType _resource;
            public Resource.ResourceType Resource
            { get => _resource; }

            [SerializeField]
            private float _price;
            public float Price
            { get => _price; }

            public Cost(Resource.ResourceType r, float p)
            {
                _resource = r;
                _price = p;
            }
        }

        [SerializeField]
        private System.Collections.Generic.List<Cost> _costs;

        public Costs(params Cost[] cs)
        {
            _costs = new System.Collections.Generic.List<Cost>();

            foreach (Cost c in cs)
                _costs.Add(c);
        }

        public IEnumerator GetEnumerator()
        {
            return _costs.GetEnumerator();
        }
    }

    private bool _running;

    [SerializeField]
    private bool _placeableOnGrass = true;
    public bool PlaceableOnGrass { get => _placeableOnGrass; }

    [SerializeField]
    private bool _placeableOnNeutral;
    public bool PlaceableOnNeutral { get => _placeableOnNeutral; }

    [SerializeField]
    private bool _placeableOnDryGround;
    public bool PlaceableOnDryGround { get => _placeableOnDryGround; }

    [SerializeField]
    private Costs _constructionCosts;
    public Costs ConstructionCosts { get => _constructionCosts; }

    [SerializeField]
    private Costs _tickCosts;
    public Costs TickCosts { get => _tickCosts; }

    [SerializeField]
    private Costs _tickProductions;
    public Costs TickProductions { get => _tickProductions; }

    [SerializeField]
    private int _capacity;
    public int Capacity { get => _capacity; }

    public Building()
    {
        _constructionCosts = new Costs();
        _tickProductions = new Costs();
        _tickCosts = new Costs();
        _capacity = 0;
    }

    // Consumes the necessary costs in order to construct the building
    public void Construct()
    {
        if (!ResourceManager.GetInstance().ApproveConstruct(_constructionCosts))
        {
            gameObject.SetActive(false);
            Debug.Log(name + " : Construction not approved, I SLEEP");
            return;
        }

        _running = true;

        foreach (Costs.Cost c in _constructionCosts)
            ResourceManager.GetInstance().Consume(c.Resource, c.Price);

        TickingSystem.GetInstance().AddBuilding(this);

        Debug.Log(name + " : Construction");
    }

    // Produces resources and consumes resources
    public void Tick()
    {
        if (!_running) return;

        foreach (Costs.Cost c in _tickProductions)
            ResourceManager.GetInstance().Produce(c.Resource, c.Price);

        foreach (Costs.Cost c in _tickCosts)
            if (!ResourceManager.GetInstance().Consume(c.Resource, c.Price))
                Disable();
    }

    //DEBUG
    private void OnEnable()
    {
        Construct();
    }

    //DEBUG
    private void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        _running = true;

        Debug.Log(name + " : I wake");
    }

    public void Disable()
    {
        _running = false;

        Debug.Log(name + " : I sleep");
    }

}
