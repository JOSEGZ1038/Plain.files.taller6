namespace PlainFiles.Core;

public class SimpleTextFile
{
    private readonly string _path;

    public SimpleTextFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));
        }

        _path = path;

        // Ensure directory exists
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create file if it doesn't exist
        if (!File.Exists(_path))
        {
            using (File.Create(_path))
            {
            }
        }
    }

    public void WriteAllLines(string[] lines)
    {
        File.WriteAllLines(_path, lines);
    }

    public string[] ReadAllLines()
    {
        return File.ReadAllLines(_path);
    }
}
