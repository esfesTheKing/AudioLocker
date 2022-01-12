using System.Windows.Forms;
using System.Reflection;
using System.Text.Json;
using System.IO;

public static class FileHelper
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public readonly static string dataFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\data.json";

    public static ProcContainer ReadDataFile()
    {
        string jsonData = File.ReadAllText(dataFilePath);
        return JsonSerializer.Deserialize<ProcContainer>(jsonData);
    }

    public static void SaveAppsToDataFile(ProcContainer audioProcs)
    {
        JsonSerializerOptions options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(audioProcs, options);
        _logger.Info("Saving audio processes to data file.");
        File.WriteAllText(dataFilePath, jsonString);
    }
}