using DIContainer.Example.Interfaces;

namespace DIContainer.Example
{
    public class Person : IPerson
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public virtual string Speak()
        {
            return string.Format(" {0} speaking", Name);
        }

        public virtual string Walk()
        {
            return string.Format(" {0} walking", Name);
        }

        public virtual string Sleep()
        {
            return string.Format(" {0} sleeping", Name);
        }
    }
}