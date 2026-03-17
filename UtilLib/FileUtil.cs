namespace UtilLib
{
    public class FileUtil
    {
        static string GetAppDataPath()
        {
            return $"{GetBasePath()}/app_data";
        }

        public static string GetBasePath()
        {
            var basePath = Directory.GetCurrentDirectory();
            basePath = basePath.Replace("\\", "/");
            if (basePath.EndsWith("bin/publish"))
            {
                basePath = basePath.Replace("bin/publish", "");
            }
            return basePath;
        }

        public static string GetCobol(string fileName)
        {
            if (!fileName.EndsWith(".cobol"))
            {
                fileName = $"{fileName}.cobol";
            }
            return GetFileText($"{GetCobolPath()}/{fileName}");
        }

        public static string[] GetCobolList()
        {
            if (!Directory.Exists(GetCobolPath()))
                return [];
            else
                return TrimFilePaths(GetCobolPaths());
        }

        public static string GetCobolPath()
        {
            return $"{GetAppDataPath()}/cobol";
        }

        public static string[] GetCobolPaths()
        {
            if (!Directory.Exists(GetCobolPath()))
                return [];
            else
                return Directory.GetFiles(GetCobolPath());
        }

        public static string GetFileText(string path)
        {
            if (!File.Exists(path))
                return $"NotFound({path})";
            return File.ReadAllText(path);
        }

        public static string GetModel(string fileName)
        {
            if (!fileName.EndsWith(".json"))
            {
                fileName = $"{fileName}.json";
            }
            return GetFileText($"{GetModelsPath()}/{fileName}");
        }

        public static string[] GetModelsList()
        {
            return TrimFilePaths(GetModelsPaths());
        }

        public static string GetModelsPath()
        {
            return $"{GetAppDataPath()}/models";
        }

        public static string[] GetModelsPaths()
        {
            if (!Directory.Exists(GetModelsPath()))
                return Array.Empty<string>();
            else
                return Directory.GetFiles(GetModelsPath());
        }


        public static string GetScript(string fileName)
        {
            if (!fileName.EndsWith(".py"))
            {
                fileName = $"{fileName}.py";
            }
            return GetFileText($"{GetScriptsPath()}/{fileName}");
        }

        public static List<FileInfo> GetScriptsInfo()
        {
            List<FileInfo> result = new List<FileInfo>();
            var paths = GetScriptsPaths();
            foreach (var path in paths)
            {
                FileInfo fi = new FileInfo(path);
                result.Add(fi);
            }
            return result;
        }

        public static string[] GetScriptsList()
        {
            return TrimFilePaths(GetScriptsPaths());
        }

        public static string[] GetScriptsPaths()
        {
            if (!Directory.Exists(GetScriptsPath()))
                return Array.Empty<string>();
            else
                return Directory.GetFiles(GetScriptsPath());
        }

        public static string GetScriptsPath()
        {
            return $"{GetAppDataPath()}/scripts";
        }

        public static string GetStdLibPath()
        {
            return $"{GetBasePath()}/lib";
        }

        public static bool SaveFileText(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string SaveModelText(string fileName, string text)
        {
            if (!fileName.EndsWith(".json"))
            {
                fileName = $"{fileName}.json";
            }
            var filePath = $"{GetModelsPath()}/{fileName}";
            var success = SaveFileText(filePath, text);
            var message = success ? "Saved" : "Could not save";
            return $"{message}: {fileName} {text.Length}";
        }

        static string[] TrimFilePaths(string[] paths)
        {
            string[] result = new string[paths.Length];
            for (var i = 0; i < paths.Length; i++)
            {
                var path = paths[i].Replace("\\", "/");
                int lastIndex = path.LastIndexOf('/');
                result[i] = path.Substring(lastIndex + 1);
            }
            return result;
        }
    }

}

