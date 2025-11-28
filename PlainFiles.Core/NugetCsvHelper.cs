using CsvHelper;
using System.Globalization;

namespace PlainFiles.Core;

public class NugetCsvHelper
{
    public void Write(string path, IEnumerable<Person> people)
    {
        using var sw = new StreamWriter(path);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(people);
    }

    public IEnumerable<Person> Read(string path)
    {
        using var sr = new StreamReader(path);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        var records = cr.GetRecords<Person>().ToList();
        return records;
    }
}
