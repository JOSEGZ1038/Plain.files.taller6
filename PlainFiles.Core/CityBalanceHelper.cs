using CsvHelper;
using System.Globalization;

namespace PlainFiles.Core;

public class CityBalanceHelper
{
    public void Write(string path, IEnumerable<City> cities)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var sw = new StreamWriter(path);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(cities);
    }

    public IEnumerable<City> Read(string path)
    {
        if (!File.Exists(path))
        {
            return new List<City>();
        }

        using var sr = new StreamReader(path);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        var records = cr.GetRecords<City>().ToList();
        return records;
    }
}
