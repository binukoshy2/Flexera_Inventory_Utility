using LicenseCalculator.App.Models;

namespace LicenseCalculator.App.Services;

public interface ILicenseCalculatorService
{
    /// <summary>
    /// Calculates the minimum number of application copies the company must purchase,
    /// given a list of raw (potentially duplicate) installation records.
    /// </summary>
    int CalculateMinimumCopies(IReadOnlyList<InstallationRecord> installations);
}
