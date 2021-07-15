namespace Victorina
{
    public class ServerEventArgument
    {
        private string _stringValue;
        private int _intValue;
        
        public ServerEventArgumentType Type { get; private set; }

        public ServerEventArgument()
        {
            Type = ServerEventArgumentType.Empty;
        }
        
        public void SetString(string stringValue)
        {
            _stringValue = stringValue;
            Type = ServerEventArgumentType.String;
        }

        public void SetInt(int intValue)
        {
            _intValue = intValue;
            Type = ServerEventArgumentType.Int;
        }
        
        public string AsString()
        {
            return _stringValue;
        }

        public int AsInt()
        {
            return _intValue;
        }

        public override string ToString()
        {
            return $"[ServerEventArgument: {Type}, {_stringValue}, {_intValue}]";
        }
    }
}