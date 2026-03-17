namespace UtilLib
{
    public class DataManager
    {
        public static void WriteModel(string fileName, IAsJson model)
        {
            FileUtil.SaveFileText(fileName, model.AsJson());
        }
    }
}
