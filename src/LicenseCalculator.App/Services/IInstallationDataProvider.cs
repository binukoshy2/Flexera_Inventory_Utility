using LicenseCalculator.App.Models;

namespace LicenseCalculator.App.Services;

public interface IInstallationDataProvider
{
    /// <summary>
    /// Returns all raw installation records for the given application ID.
    /// Deduplication is NOT the responsibility of this provider.
    /// </summary>
    IReadOnlyList<InstallationRecord> GetInstallations(int applicationId);
}
