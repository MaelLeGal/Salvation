public class Resource
{

    public enum ResourceType
    {
        People,
        Energy,
        Food
    }

    // Current amount of the resource
    private float _amount;
    public float Amount
    { get => _amount; }

    public Resource(float amount)
    {
        _amount = amount;
    }

    // Produces x amount of the resource
    public void Produce(float x)
    {
        _amount += x;
    }

    // Consumes x amount of the resource and returns whether there is enough resource or not
    public bool Consume(float x)
    {
        if (_amount - x < 0f) return false;

        _amount -= x;
        return true;
    }

}
