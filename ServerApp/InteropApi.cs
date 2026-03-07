using Npgsql;

namespace BlazorLib.Interop
{
    public interface IInteropApi
    {
        List<ScriptFile> CobolFiles { get; }
        List<ScriptFile> PythonFiles { get; }

        List<ScriptFile> GetCobolFiles();
        List<ScriptFile> GetPythonFiles();

        Task LoadCobolFilesAsync();
        Task LoadPythonFilesAsync();
    }

    public class InteropApi : IInteropApi
    {
        private const string ConnectionString = "Host=localhost;Port=5432;Database=cobol_studio;Username=admin;Password=chichi";
        private bool _cobolInitialized;
        private bool _pythonInitialized;

        public List<ScriptFile> CobolFiles { get; } = new();
        public List<ScriptFile> PythonFiles { get; } = new();

        public List<ScriptFile> GetCobolFiles()
        {
            LoadCobolFiles();
            return CobolFiles;
        }

        public List<ScriptFile> GetPythonFiles()
        {
            LoadPythonFiles();
            return PythonFiles;
        }

        public void LoadCobolFiles()
        {
            if (_cobolInitialized)
            {
                return;
            }

            _cobolInitialized = true;
            CobolFiles.Clear();

            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT file_name, content FROM cobol_source_files ORDER BY file_name", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CobolFiles.Add(new ScriptFile
                    {
                        FileName = reader.GetString(0),
                        Content = reader.GetString(1)
                    });
                }
            }
            catch
            {
                _cobolInitialized = false;
                throw;
            }
        }

        public void LoadPythonFiles()
        {
            if (_pythonInitialized)
            {
                return;
            }

            _pythonInitialized = true;
            PythonFiles.Clear();

            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT file_name, content FROM python_source_files ORDER BY file_name", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PythonFiles.Add(new ScriptFile
                    {
                        FileName = reader.GetString(0),
                        Content = reader.GetString(1)
                    });
                }
            }
            catch
            {
                _pythonInitialized = false;
                throw;
            }
        }

        public Task LoadCobolFilesAsync()
        {
            LoadCobolFiles();
            return Task.CompletedTask;
        }

        public Task LoadPythonFilesAsync()
        {
            LoadPythonFiles();
            return Task.CompletedTask;
        }
    }
}
