namespace LicenseCalculator.App.Models;

/// <summary>
/// Represents a single installation record of an application on a computer for a user.
/// Equality is based on ComputerID, UserID, ApplicationID and ComputerType (case-insensitive)
/// so that duplicate records from different source systems are treated as one installation.
/// </summary>
public sealed record InstallationRecord(
    int ComputerId,
    int UserId,
    int ApplicationId,
    ComputerType ComputerType
);
