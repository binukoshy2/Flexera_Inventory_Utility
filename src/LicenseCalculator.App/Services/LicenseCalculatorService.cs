using LicenseCalculator.App.Models;

namespace LicenseCalculator.App.Services;

/// <summary>
/// Calculates the minimum number of application copies required based on the licensing rule:
///
///   One copy covers up to two computers for the same user,
///   but only if at least one of those computers is a laptop.
///   A desktop paired with no laptop always requires its own copy.
///
/// Duplicate records (same ComputerID + UserID + ApplicationID + ComputerType) are
/// eliminated before calculation.
/// </summary>
public sealed class LicenseCalculatorService : ILicenseCalculatorService
{
    public int CalculateMinimumCopies(IReadOnlyList<InstallationRecord> installations)
    {
        // Deduplicate: ComputerType enum is already normalised (case-insensitive at parse time)
        var unique = installations
            .DistinctBy(r => (r.ComputerId, r.UserId, r.ApplicationId, r.ComputerType))
            .ToList();

        return unique
            .GroupBy(r => r.UserId)
            .Sum(group => CalculateCopiesForUser(group.ToList()));
    }

    private static int CalculateCopiesForUser(IReadOnlyList<InstallationRecord> userInstallations)
    {
        int laptops  = userInstallations.Count(r => r.ComputerType == ComputerType.Laptop);
        int desktops = userInstallations.Count(r => r.ComputerType == ComputerType.Desktop);

        // Each laptop can cover itself plus one other computer (desktop or another laptop).
        // Pair laptops with desktops first (most constrained resource), then pair remaining
        // laptops with each other. Any unpaired desktop needs its own copy.
        int laptopDesktopPairs = Math.Min(laptops, desktops);
        int remainingLaptops   = laptops  - laptopDesktopPairs;
        int remainingDesktops  = desktops - laptopDesktopPairs;

        int laptopLaptopPairs  = remainingLaptops / 2;
        int singleLaptops      = remainingLaptops % 2;

        return laptopDesktopPairs + laptopLaptopPairs + singleLaptops + remainingDesktops;
    }
}
