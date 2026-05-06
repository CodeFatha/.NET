using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var salesFiles = FindFiles(storesDirectory);

var salesTotal = Math.Round(CalculateSalesTotal(salesFiles), 2);
SalesSummaryReport(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    if (!Directory.Exists(folderName))
    {
        Console.WriteLine($"Directory '{folderName}' not found. Creating it...");
        Directory.CreateDirectory(folderName);
    }

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

void SalesSummaryReport(IEnumerable<string> salesFiles)
{
    StringBuilder report = new StringBuilder();
    report.AppendLine("Sales Summary Report");
    report.AppendLine("--------------------");
    report.AppendLine($"Total Sales: {salesTotal}{Environment.NewLine}");
    report.AppendLine($"Details: ");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        if (data?.Total > 0)
        {
            report.AppendLine($"{Path.GetFileName(Path.GetDirectoryName(file))}: {data?.Total}");
        }
    }

    File.AppendAllText(Path.Combine(salesTotalDir, "sales_summary.txt"), $"{report}");
}

record SalesData (double Total);
