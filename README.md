# Flexera Inventory Utility — License Calculator

A .NET 10 console application that reads software installation records from CSV files and calculates the **minimum number of license copies** a company must purchase.

---

## The Licensing Rule

> One license copy covers **up to two computers** for the **same user**, but **only if at least one** of those computers is a **laptop**.  
> A desktop with no laptop always requires its own copy.

| User's computers | Copies needed |
|---|---|
| 1 Laptop | 1 |
| 1 Desktop | 1 |
| 1 Laptop + 1 Desktop | 1 (paired) |
| 2 Laptops | 1 (paired together) |
| 2 Desktops | 2 (no laptop to pair with) |
| 2 Laptops + 1 Desktop | 2 (L+D paired, L alone) |

---

## Project Structure

```
Flexera_Inventory_Utility/
├── data/                              # Input CSV files
│   ├── sample-small.csv
│   └── sample-large.csv
│
├── src/
│   └── LicenseCalculator.App/         # Main console application
│       ├── Program.cs                 # Entry point
│       ├── Models/
│       │   ├── InstallationRecord.cs  # Data model (ComputerId, UserId, AppId, ComputerType)
│       │   └── ComputerType.cs        # Enum: Laptop | Desktop
│       └── Services/
│           ├── IInstallationDataProvider.cs    # Interface for data reading
│           ├── CsvInstallationDataProvider.cs  # Reads .csv files (streameid)
│           ├── ILicenseCalculatorService.cs    # Interface for calculation
│           └── LicenseCalculatorService.cs     # Core business logic
│
└── tests/
    └── LicenseCalculator.Tests/       # xUnit test project
        └── Services/
            └── LicenseCalculatorServiceTests.cs
```

---

## CSV Input Format

Files must be placed in the `data/` folder (or a custom path passed as an argument).

```
ComputerID,UserID,ApplicationID,ComputerType,Comment
1,1091,374,LAPTOP,Exported from System A
2,1091,374,DESKTOP,Exported from System B
3,2045,374,DESKTOP,Exported from System A
```

| Column | Type | Notes |
|---|---|---|
| ComputerID | integer | Unique computer identifier |
| UserID | integer | User the software is installed for |
| ApplicationID | integer | App being tracked (currently filtered to **374**) |
| ComputerType | string | `LAPTOP` or `DESKTOP` (case-insensitive) |
| Comment | string | Optional, ignored by the calculator |

> Rows with missing or non-numeric `ComputerID`, `UserID`, or `ApplicationID` are silently skipped.  
> Exact duplicate rows (same ComputerID + UserID + ApplicationID + ComputerType) are deduplicated before calculation.

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

---

## Getting Started

### 1. Clone / open the workspace

```
cd C:\Flexera_Inventory_Utility
```

### 2. Build

```
dotnet build
```

### 3. Run

Using the default `./data` folder:

```
dotnet run --project src/LicenseCalculator.App
```

Using a custom data folder:

```
dotnet run --project src/LicenseCalculator.App -- "C:\path\to\your\data"
```

### 4. Example output

```
Data folder      : C:\Flexera_Inventory_Utility\data
CSV files found  : 2
  - sample-large.csv  (1022.2 MB)
  - sample-small.csv  (9.8 MB)
Application ID   : 374
--------------------------------------------------
  Reading CSV: sample-large.csv (1022 MB) ... 22114 record(s) matched.
  Reading CSV: sample-small.csv (9 MB) ...  204 record(s) matched.
Records from CSV   : 22318
Total raw records  : 22318

==================================================
  MINIMUM COPIES REQUIRED: 14034
==================================================
```

---

## Running Tests

```
dotnet test
```

Or targeting only the test project:

```
dotnet test tests/LicenseCalculator.Tests
```

The test suite covers:

| Scenario | Expected |
|---|---|
| 1 Laptop + 1 Desktop, same user | 1 copy |
| 2 Desktops + 1 Laptop (2 users) | 3 copies |
| Duplicate rows in input | Deduplicated correctly |
| Same record repeated 5 times | 1 copy |
| Single laptop | 1 copy |
| Single desktop | 1 copy |
| 2 Laptops, same user | 1 copy |
| 3 Laptops, same user | 2 copies |
| 3 users with mixed devices | 6 copies |
| Empty input | 0 copies |

---

