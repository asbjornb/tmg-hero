namespace tmg_hero.Model;

internal class Resource
{
    public Resource(string name, int amount, int cap, double income)
    {
        Name = name;
        Amount = amount;
        Cap = cap;
        Income = income;
    }

    public string Name { get; }
    public int Amount { get; }
    public int Cap { get; }
    public double Income { get; }

    public bool IsCapped => Amount >= Cap;
}
