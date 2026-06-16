using LicenseCalculator.App.Models;

namespace LicenseCalculator.App.Services;

/// <summary>
/// Reads installation records from all .csv files found in a given folder.
/// Files are processed line-by-line (streamed) so that very large files
/// never need to be fully loaded into memory.
///
/// Expected CSV format (first row = header, case-insensitive):
///   ComputerID, UserID, ApplicationID, ComputerType, Comment
/// </summary>
public sealed class CsvInstallationDataProvider : IInstallationDataProvider
{
    private readonly string _dataFolder;

    public CsvInstallationDataProvider(string dataFolder)
    {
        if (string.IsNullOrWhiteSpace(dataFolder))
            throw new ArgumentException("Data folder path must not be empty.", nameof(dataFolder));
        if (!Directory.Exists(dataFolder))
            throw new DirectoryNotFoundException($"Data folder not found: {dataFolder}");

        _dataFolder = dataFolder;
    }

    public IReadOnlyList<InstallationRecord> GetInstallations(int applicationId)
    {
        var records = new List<InstallationRecord>();

        foreach (var file in Directory.EnumerateFiles(_dataFolder, "*.csv", SearchOption.AllDirectories))
        {
            long fileSizeMb = new FileInfo(file).Length / 1024 / 1024;
            Console.Write($"  Reading CSV: {Path.GetFileName(file)} ({fileSizeMb} MB) ... ");
            int count = 0;
            foreach (var record in StreamFile(file, applicationId))
            {
                records.Add(record);
                count++;
                // Print a dot every 1 000 matched records so the user sees progress
                if (count % 1_000 == 0)
                    Console.Write('.');
            }
            Console.WriteLine($" {count} record(s) matched.");
        }

        return records;
    }

    private static IEnumerable<InstallationRecord> StreamFile(string path, int applicationId)
    {
        // StreamReader reads one line at a time — safe for files of any size
        using var reader = new StreamReader(path);

        string? headerLine = reader.ReadLine();
        if (headerLine is null) yield break;

        var headers = ParseCsvLine(headerLine)
            .Select((name, index) => (Name: name.Trim().ToLowerInvariant(), Index: index))
            .ToDictionary(x => x.Name, x => x.Index);

        if (!headers.TryGetValue("computerid",    out int computerIdIdx) ||
            !headers.TryGetValue("userid",        out int userIdIdx)     ||
            !headers.TryGetValue("applicationid", out int appIdIdx)      ||
            !headers.TryGetValue("computertype",  out int typeIdx))
        {
            yield break;
        }

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cells = ParseCsvLine(line);

            if (!int.TryParse(GetCell(cells, computerIdIdx), out int computerId)) continue;
            if (!int.TryParse(GetCell(cells, userIdIdx),     out int userId))     continue;
            if (!int.TryParse(GetCell(cells, appIdIdx),      out int appId))      continue;
            if (appId != applicationId) continue;

            ComputerType computerType = GetCell(cells, typeIdx).ToUpperInvariant() == "LAPTOP"
                ? ComputerType.Laptop
                : ComputerType.Desktop;

            yield return new InstallationRecord(computerId, userId, appId, computerType);
        }
    }

    /// <summary>
    /// Splits a single CSV line respecting double-quoted fields that may contain commas.
    /// </summary>
    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // Handle escaped quotes ("")
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString().Trim());
        return fields;
    }

    private static string GetCell(List<string> cells, int index)
        => index < cells.Count ? cells[index] : string.Empty;
}
