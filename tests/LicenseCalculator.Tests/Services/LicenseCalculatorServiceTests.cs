using LicenseCalculator.App.Models;
using LicenseCalculator.App.Services;

namespace LicenseCalculator.Tests.Services;

public class LicenseCalculatorServiceTests
{
    private readonly LicenseCalculatorService _sut = new();

    // ── Problem.md Example 1 ────────────────────────────────────────────────
    // User 1: 1 laptop + 1 desktop → 1 copy

    [Fact]
    public void CalculateMinimumCopies_Example1_Returns1()
    {
        var installations = new List<InstallationRecord>
        {
            new(ComputerId: 1, UserId: 1, ApplicationId: 374, ComputerType: ComputerType.Laptop),
            new(ComputerId: 2, UserId: 1, ApplicationId: 374, ComputerType: ComputerType.Desktop),
        };

        int result = _sut.CalculateMinimumCopies(installations);

        Assert.Equal(1, result);
    }

    // ── Problem.md Example 2 ────────────────────────────────────────────────
    // User 1: 1 laptop + 1 desktop → 1 copy
    // User 2: 2 desktops (no laptop) → 2 copies
    // Total: 3 copies

    [Fact]
    public void CalculateMinimumCopies_Example2_Returns3()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Laptop),
            new(2, 1, 374, ComputerType.Desktop),
            new(3, 2, 374, ComputerType.Desktop),
            new(4, 2, 374, ComputerType.Desktop),
        };

        int result = _sut.CalculateMinimumCopies(installations);

        Assert.Equal(3, result);
    }

    // ── Problem.md Example 3 ────────────────────────────────────────────────
    // Rows 2 and 3 are duplicates (same ComputerID + ComputerType after normalisation)
    // User 1: 1 laptop → 1 copy
    // User 2: 1 desktop (after dedup) → 1 copy
    // Total: 2 copies

    [Fact]
    public void CalculateMinimumCopies_Example3_DedupesAndReturns2()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Laptop),
            new(2, 2, 374, ComputerType.Desktop),  // original
            new(2, 2, 374, ComputerType.Desktop),  // duplicate (already normalised to enum)
        };

        int result = _sut.CalculateMinimumCopies(installations);

        Assert.Equal(2, result);
    }

    // ── Deduplication ───────────────────────────────────────────────────────

    [Fact]
    public void CalculateMinimumCopies_IdenticalRecords_CountedOnce()
    {
        var installations = Enumerable.Repeat(
            new InstallationRecord(1, 1, 374, ComputerType.Laptop), 5).ToList();

        int result = _sut.CalculateMinimumCopies(installations);

        Assert.Equal(1, result);
    }

    // ── Single computer per user ─────────────────────────────────────────────

    [Fact]
    public void CalculateMinimumCopies_SingleLaptopPerUser_Returns1()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Laptop),
        };

        Assert.Equal(1, _sut.CalculateMinimumCopies(installations));
    }

    [Fact]
    public void CalculateMinimumCopies_SingleDesktopPerUser_Returns1()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Desktop),
        };

        Assert.Equal(1, _sut.CalculateMinimumCopies(installations));
    }

    // ── Multiple laptops ─────────────────────────────────────────────────────

    [Fact]
    public void CalculateMinimumCopies_TwoLaptopsSameUser_Returns1()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Laptop),
            new(2, 1, 374, ComputerType.Laptop),
        };

        Assert.Equal(1, _sut.CalculateMinimumCopies(installations));
    }

    [Fact]
    public void CalculateMinimumCopies_ThreeLaptopsSameUser_Returns2()
    {
        var installations = new List<InstallationRecord>
        {
            new(1, 1, 374, ComputerType.Laptop),
            new(2, 1, 374, ComputerType.Laptop),
            new(3, 1, 374, ComputerType.Laptop),
        };

        Assert.Equal(2, _sut.CalculateMinimumCopies(installations));
    }

    // ── Multiple users ───────────────────────────────────────────────────────

    [Fact]
    public void CalculateMinimumCopies_MultipleUsers_SumsCorrectly()
    {
        var installations = new List<InstallationRecord>
        {
            // User 1: 1 laptop + 1 desktop → 1 copy
            new(1, 1, 374, ComputerType.Laptop),
            new(2, 1, 374, ComputerType.Desktop),
            // User 2: 3 desktops → 3 copies
            new(3, 2, 374, ComputerType.Desktop),
            new(4, 2, 374, ComputerType.Desktop),
            new(5, 2, 374, ComputerType.Desktop),
            // User 3: 2 laptops + 1 desktop → 2 copies (laptop+desktop, laptop alone)
            new(6, 3, 374, ComputerType.Laptop),
            new(7, 3, 374, ComputerType.Laptop),
            new(8, 3, 374, ComputerType.Desktop),
        };

        // 1 + 3 + 2 = 6
        Assert.Equal(6, _sut.CalculateMinimumCopies(installations));
    }

    // ── Empty input ──────────────────────────────────────────────────────────

    [Fact]
    public void CalculateMinimumCopies_EmptyList_Returns0()
    {
        Assert.Equal(0, _sut.CalculateMinimumCopies([]));
    }
}
