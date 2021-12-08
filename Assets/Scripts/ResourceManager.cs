using UnityEngine;
using UnityEngine.UI;

public sealed class ResourceManager : MonoBehaviour
{

    private static ResourceManager _instance;

    private Resource _people;
    private Resource _energy;
    private Resource _food;

    [Header("Initialization values")]

    public float StartingPeople = 10f;
    public float StartingEnergy = 10f;
    public float StartingFood = 10f;

    public Text[] texts;

    private ResourceManager() { }

    public static ResourceManager GetInstance()
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
        GetInstance()._people = new Resource(StartingPeople);
        GetInstance()._energy = new Resource(StartingEnergy);
        GetInstance()._food = new Resource(StartingFood);
    }

    // Called each x seconds, manages resources
    public void Tick()
    {
        // Manages resources production and loss
    }

    private void Update()
    {
        texts[(int)Resource.ResourceType.People].text = "People : " + _people.Amount;
        texts[(int)Resource.ResourceType.Energy].text = "Energy : " + _energy.Amount;
        texts[(int)Resource.ResourceType.Food].text = "Food : " + _food.Amount;
    }

    // Produces an amount of a given resource
    public void Produce(Resource.ResourceType r, float amount)
    {
        switch (r)
        {
            case Resource.ResourceType.People:
                GetInstance()._people.Produce(amount);
                break;

            case Resource.ResourceType.Energy:
                GetInstance()._energy.Produce(amount);
                break;

            case Resource.ResourceType.Food:
                GetInstance()._food.Produce(amount);
                break;

            default:
                break;
        }

        Debug.Log("RM : Producing " + amount + " of " + r);
    }

    // Consumes an amount of a given resource
    public bool Consume(Resource.ResourceType r, float amount)
    {
        Debug.Log("RM : Consuming " + amount + " of " + r);

        switch (r)
        {
            case Resource.ResourceType.People:
                return GetInstance()._people.Consume(amount);

            case Resource.ResourceType.Energy:
                return GetInstance()._energy.Consume(amount);

            case Resource.ResourceType.Food:
                return GetInstance()._food.Consume(amount);

            default:
                return false;
        }
    }

    // Approves a construction if the costs are acceptable
    public bool ApproveConstruct(Building.Costs cs)
    {
        foreach (Building.Costs.Cost c in cs)
        {
            switch (c.Resource)
            {
                case Resource.ResourceType.People:
                    if (_people.Amount < c.Price) return false;
                    break;

                case Resource.ResourceType.Energy:
                    if (_energy.Amount < c.Price) return false;
                    break;

                case Resource.ResourceType.Food:
                    if (_food.Amount < c.Price) return false;
                    break;

                default:
                    break;
            }
        }

        return true;
    }

}
