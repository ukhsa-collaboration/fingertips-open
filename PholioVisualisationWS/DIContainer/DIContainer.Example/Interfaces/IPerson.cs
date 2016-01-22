namespace DIContainer.Example.Interfaces
{
    public interface IPerson
    {
        string Name { get; set; }
        string Speak();
        string Walk();
        string Sleep();
    }
}