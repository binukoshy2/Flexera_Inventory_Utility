using ClosedXML.Excel;

// Creates a second sample Excel file in the data folder to demonstrate
// that the app aggregates records across multiple files.

string dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");

string path = Path.Combine(dataFolder, "additional-installations.xlsx");

using var workbook = new XLWorkbook();
var sheet = workbook.AddWorksheet("Installations");

// Headers
sheet.Cell(1, 1).Value = "ComputerID";
sheet.Cell(1, 2).Value = "UserID";
sheet.Cell(1, 3).Value = "ApplicationID";
sheet.Cell(1, 4).Value = "ComputerType";
sheet.Cell(1, 5).Value = "Comment";

// User 4: 2 laptops → 1 copy  (laptop-laptop pair)
sheet.Cell(2, 1).Value = 10; sheet.Cell(2, 2).Value = 4; sheet.Cell(2, 3).Value = 374; sheet.Cell(2, 4).Value = "LAPTOP";  sheet.Cell(2, 5).Value = "File 2";
sheet.Cell(3, 1).Value = 11; sheet.Cell(3, 2).Value = 4; sheet.Cell(3, 3).Value = 374; sheet.Cell(3, 4).Value = "LAPTOP";  sheet.Cell(3, 5).Value = "File 2";

// User 5: 1 desktop → 1 copy
sheet.Cell(4, 1).Value = 12; sheet.Cell(4, 2).Value = 5; sheet.Cell(4, 3).Value = 374; sheet.Cell(4, 4).Value = "DESKTOP"; sheet.Cell(4, 5).Value = "File 2";

workbook.SaveAs(path);
Console.WriteLine($"Created: {path}");
