using LicenseCalculator.App;
using LicenseCalculator.App.Services;

const int ApplicationId = 374;

// Resolve the data folder: argument > default (./data relative to cwd)
string dataFolder = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Directory.GetCurrentDirectory(), "data");

// ── Enumerate all source files ────────────────────────────────────────────
var csvFiles = Directory
    .EnumerateFiles(dataFolder, "*.csv", SearchOption.AllDirectories)
    .OrderBy(f => f)
    .ToList();

Console.WriteLine($"Data folder      : {dataFolder}");
Console.WriteLine($"CSV files found  : {csvFiles.Count}");
foreach (var file in csvFiles)
    Console.WriteLine($"  - {Path.GetFileName(file)}  ({new FileInfo(file).Length / 1024.0 / 1024.0:F1} MB)");

Console.WriteLine($"Application ID   : {ApplicationId}");
Console.WriteLine(new string('-', 50));

// ── Read installations from CSV (streamed line-by-line for large files) ────
var calculator = new LicenseCalculatorService();

var csvRecords = new CsvInstallationDataProvider(dataFolder).GetInstallations(ApplicationId);

Console.WriteLine($"Records from CSV   : {csvRecords.Count}");
Console.WriteLine($"Total raw records  : {csvRecords.Count}");
Console.WriteLine();

int minimumCopies = calculator.CalculateMinimumCopies(csvRecords);
Console.WriteLine();
Console.WriteLine(new string('=', 50));
Console.WriteLine($"  MINIMUM COPIES REQUIRED: {minimumCopies}");
Console.WriteLine(new string('=', 50));
