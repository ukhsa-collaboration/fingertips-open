namespace DIContainer.Example
{
    public class SpecialPerson : Person
    {
        public SpecialPerson(string name)
            : base(name)
        {
        }

        public override string Speak()
        {
            return string.Format(" {0} speaking loudly", Name);
        }

        public override string Walk()
        {
            return string.Format(" {0} walking briskly", Name);
        }

        public override string Sleep()
        {
            return string.Format(" {0} sleeping soundly", Name);
        }
    }
}