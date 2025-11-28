namespace PlainFiles.Core;

public class ManualCsvHelper
{
    public void WriteCsv(string path, List<string[]> records)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("El path no puede ser nulo o vacío.", nameof(path));

        if (records is null)
            throw new ArgumentNullException(nameof(records));

        EnsureFileAndDirectory(path);

        using var sw = new StreamWriter(path);
        foreach (var record in records)
        {
            var line = string.Join(",", record);
            sw.WriteLine(line);
        }
    }

    public List<string[]> ReadCsv(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("El path no puede ser nulo o vacío.", nameof(path));

        EnsureFileAndDirectory(path);

        var result = new List<string[]>();
        using var sr = new StreamReader(path);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var fields = line.Split(',');
            result.Add(fields);
        }
        return result;
    }

    private static void EnsureFileAndDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (!File.Exists(path))
        {
            // Create an empty file and close it immediately
            using var fs = File.Create(path);
        }
    }
}