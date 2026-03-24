using Newtonsoft.Json;
using CobolAiLib.constants;
using CobolAiLib.interfaces;

namespace CobolAiLib
{
    public class InteropUtil
    {
        static IRunScript? runScript;

        public static MessageRecord CreateMessage(string name, params object[] args)
        {
            return new MessageRecord(name, args);
        }

        public static string? CreateEncodedMessageStr64(string name, params object[] args)
        {
            return EncodeMessage64(CreateMessage(name, args));
        }

        public static MessageRecord? DecodeMessage64(string messageStr64)
        {
            try
            {
                var jsonStr = Base64Util.Decode64(messageStr64);
                return JsonConvert.DeserializeObject<MessageRecord>(jsonStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string? EncodeMessage64(MessageRecord message)
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(message);
                return Base64Util.Encode64(jsonStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void Log(string text)
        {
            RunScript($"console.log('{text}')");
        }

        public static void RunScript(string script)
        {
            runScript?.RunScript(script);
        }

        public static void SendMessage(string template, string message_name, params object[] args)
        {
            var message = CreateMessage(message_name, args);
            var messageStr64 = EncodeMessage64(message);
            var script = string.Format(template, messageStr64);
            Log($"Script: {script}");
            RunScript(script);
        }

        public static void SendAppMessage(MessageRecord message)
        {
            SendMessage(JavaScriptConstants.SEND_APP_MESSAGE_TEMPLATE, message.Name, message.Args);
        }

        public static void SetRunScript(IRunScript runScriptArg)
        {
            runScript = runScriptArg;
        }

    }
}