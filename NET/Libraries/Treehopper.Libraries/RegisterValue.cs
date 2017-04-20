namespace Treehopper.Libraries
{
    public class RegisterValue
    {
        private int _value;

        public delegate void ValueUpdatedHandler(RegisterValue register);

        public int Offset { get; private set; }
        public int Length { get; private set; }

        public RegisterDepth Depth { get; private set; }

        public RegisterValue(string name, int offset, int length = 1, RegisterDepth registerDepth = RegisterDepth.Unsigned)
        {
            Name = name;
            Offset = offset;
            Length = length;
            Depth = registerDepth;
        }

        public event ValueUpdatedHandler ValueUpdated;

        public string Name { get; set; }

        public int Value
        {
            get { return _value; }
            set { _value = value; ValueUpdated?.Invoke(this); }
        }

        public void updateValue(int newValue)
        {
            _value = newValue;
        }
    }
}