namespace CobolAiLib
{
    public class MessageRecord
    {
        public string Name;
        public object[] Args;

        public MessageRecord(string name, params object[] args)
        {
            Name = name;
            Args = args;
        }

    }
}
