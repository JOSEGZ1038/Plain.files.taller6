using PlainFiles.Core;

// Simple authentication
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
string usersFile = "Users.txt";
if (!File.Exists(usersFile))
{
    Console.WriteLine("Users.txt file not found. Please create the file with user data.");
    return;
}
var users = File.ReadAllLines(usersFile)
    .Select(line => line.Split(','))
    .Where(parts => parts.Length == 3)
    .Select(parts => new { Username = parts[0], Password = parts[1], Active = parts[2].Trim().ToLower() == "true" })
    .ToList();
string? currentUser = null;
for (int attempt = 1; attempt <= 3; attempt++)
{
    Console.Write("User: ");
    var username = Console.ReadLine();
    Console.Write("Password: ");
    var password = Console.ReadLine();
    var user = users.FirstOrDefault(u => u.Username == username);
    if (user == null || !user.Active)
    {
        Console.WriteLine("Invalid or blocked user.");
        continue;
    }
    if (user.Password == password)
    {
        currentUser = user.Username;
        break;
    }
    Console.WriteLine($"Incorrect. Attempt {attempt}/3");
    if (attempt == 3)
    {
        BlockUser(usersFile, username);
        Console.WriteLine("User blocked.");
        return;
    }
}
if (currentUser == null)
{
    Console.WriteLine("Access denied.");
    return;
}
Console.WriteLine($"Welcome, {currentUser}!\n");

var manualCsv = new ManualCsvHelper();
Console.Write("List name (default 'people'): ");
var listName = Console.ReadLine();
if (string.IsNullOrEmpty(listName)) listName = "people";
var people = manualCsv.ReadCsv($"{listName}.csv");

var log = new LogWriter("log.txt");
var option = string.Empty;

do
{
    option = ShowMenu();
    switch (option)
    {
        case "1": ShowContent(); log.WriteLog("INFO", $"{currentUser} visualizó el contenido"); break;
        case "2": AddPerson(); log.WriteLog("INFO", $"{currentUser} agregó una persona"); break;
        case "3": SaveFile(people, listName); log.WriteLog("INFO", $"{currentUser} guardó los cambios"); Console.WriteLine("Saved."); break;
        case "4": EditPerson(); log.WriteLog("INFO", $"{currentUser} editó una persona"); break;
        case "5": DeletePerson(); log.WriteLog("INFO", $"{currentUser} borró una persona"); break;
        case "6": CityReport(); log.WriteLog("INFO", $"{currentUser} generó el informe por ciudad"); break;
        case "0": Console.WriteLine("Bye!"); break;
        default: Console.WriteLine("Invalid."); break;
    }
} while (option != "0");

// --- AUXILIARY FUNCTIONS ---
string ShowMenu()
{
    Console.WriteLine("\n1. Show content");
    Console.WriteLine("2. Add person");
    Console.WriteLine("3. Save changes");
    Console.WriteLine("4. Edit person");
    Console.WriteLine("5. Delete person");
    Console.WriteLine("6. City report");
    Console.WriteLine("0. Exit");
    Console.Write("Option: ");
    return Console.ReadLine() ?? string.Empty;
}
void AddPerson()
{
    int id;
    while (true)
    {
        Console.Write("ID: ");
        var idStr = Console.ReadLine();
        if (!int.TryParse(idStr, out id) || id < 0 || people.Any(p => p.Length > 0 && int.TryParse(p[0], out var pid) && pid == id))
        { Console.WriteLine("Invalid or duplicate."); continue; }
        break;
    }
    string name; do { Console.Write("First name: "); name = Console.ReadLine() ?? ""; } while (string.IsNullOrWhiteSpace(name));
    string lastName; do { Console.Write("Last name: "); lastName = Console.ReadLine() ?? ""; } while (string.IsNullOrWhiteSpace(lastName));
    string phone; do { Console.Write("Phone: "); phone = Console.ReadLine() ?? ""; } while (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{7,10}$"));
    Console.Write("City: "); var city = Console.ReadLine();
    string balanceStr; decimal balance; do { Console.Write("Balance: "); balanceStr = Console.ReadLine() ?? ""; } while (!decimal.TryParse(balanceStr, out balance) || balance < 0);
    people.Add(new string[] { id.ToString(), name, lastName, phone, city ?? "", balance.ToString() });
}
void ShowContent()
{
    if (people.Count == 0) { Console.WriteLine("No data."); return; }
    Console.WriteLine("==============================================");
    foreach (var p in people)
    {
        string id = p.Length > 0 ? p[0] : "";
        string name = p.Length > 1 ? p[1] : "";
        string lastName = p.Length > 2 ? p[2] : "";
        string phone = p.Length > 3 ? p[3] : "";
        string city = p.Length > 4 ? p[4] : "";
        string balanceStr = (p.Length > 5 && decimal.TryParse(p[5], out var balance)) ? balance.ToString("C2") : "";

        Console.WriteLine($"{id}\n    {name} {lastName}\n    Phone: {phone}\n    City: {city}\n    Balance:    {balanceStr}\n");
    }
    Console.WriteLine("==============================================");
}
void EditPerson()
{
    Console.Write("ID to edit: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id)) { Console.WriteLine("Invalid ID."); return; }
    var person = people.FirstOrDefault(p => p.Length > 0 && int.TryParse(p[0], out var pid) && pid == id);
    if (person == null) { Console.WriteLine("Not found."); return; }
    Console.Write($"First name ({person[1]}): "); var name = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(name)) person[1] = name;
    Console.Write($"Last name ({person[2]}): "); var lastName = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(lastName)) person[2] = lastName;
    Console.Write($"Phone ({person[3]}): "); var phone = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(phone)) person[3] = phone;
    Console.Write($"City ({person[4]}): "); var city = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(city)) person[4] = city;
    Console.Write($"Balance ({person[5]}): "); var balance = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(balance)) person[5] = balance;
    Console.WriteLine("Updated.");
}
void DeletePerson()
{
    Console.Write("ID to delete: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id)) { Console.WriteLine("Invalid ID."); return; }
    var person = people.FirstOrDefault(p => p.Length > 0 && int.TryParse(p[0], out var pid) && pid == id);
    if (person == null) { Console.WriteLine("Not found."); return; }
    Console.WriteLine($"{person[0]} {person[1]} {person[2]} {person[3]} {person[4]} {person[5]}");
    Console.Write("Delete? (y/n): ");
    var confirm = Console.ReadLine();
    if (confirm?.Trim().ToLower() == "y") { people.Remove(person); Console.WriteLine("Deleted."); } else { Console.WriteLine("Cancelled."); }
}
void CityReport()
{
    if (people.Count == 0) { Console.WriteLine("No data."); return; }
    var byCity = people.Where(p => p.Length > 4).GroupBy(p => p[4]).OrderBy(g => g.Key);
    decimal totalGeneral = 0;
    foreach (var cityGroup in byCity)
    {
        Console.WriteLine($"\nCiudad: {cityGroup.Key}\n");
        Console.WriteLine($"ID   Nombres           Apellidos         Saldo");
        Console.WriteLine($"--   ----------------  ----------------  ----------");
        decimal subtotal = 0;
        foreach (var p in cityGroup)
        {
            string id = p.Length > 0 ? p[0] : "";
            string name = p.Length > 1 ? p[1] : "";
            string lastName = p.Length > 2 ? p[2] : "";
            decimal bal = 0; if (p.Length > 5 && decimal.TryParse(p[5], out bal)) subtotal += bal;
            Console.WriteLine($"{id,-4}{name,-18}{lastName,-18}{bal,10:N2}");
        }
        Console.WriteLine($"                                 ========");
        Console.WriteLine($"Total: {cityGroup.Key,-24}{subtotal,10:N2}\n");
        totalGeneral += subtotal;
    }
    Console.WriteLine($"                                 ========");
    Console.WriteLine($"Total General:{totalGeneral,28:N2}");
}
void SaveFile(List<string[]> people, string? listName)
{
    new ManualCsvHelper().WriteCsv($"{listName}.csv", people);
}
// Release the log on exit
AppDomain.CurrentDomain.ProcessExit += (s, e) => log.Dispose();
void BlockUser(string usersFile, string? username)
{
    if (string.IsNullOrWhiteSpace(username)) return;
    var lines = File.ReadAllLines(usersFile);
    for (int i = 0; i < lines.Length; i++)
    {
        var parts = lines[i].Split(',');
        if (parts.Length == 3 && parts[0] == username)
        {
            parts[2] = "false";
            lines[i] = string.Join(",", parts);
        }
    }
    File.WriteAllLines(usersFile, lines);
}