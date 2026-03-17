namespace UtilLib
{
    public class Message
    {
        public string Name;
        public object[] Args;

        public Message(string name, params object[] args)
        {
            Name = name;
            Args = args;
        }

    }
}
