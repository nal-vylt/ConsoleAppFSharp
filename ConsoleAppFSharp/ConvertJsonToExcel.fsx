#r "nuget: Newtonsoft.Json"
#r "nuget: SpreadsheetGear"

open System.IO
open Newtonsoft.Json
open SpreadsheetGear
open System.Threading.Tasks

type Sale =
    {
      Id: int
      EmployeeId: string
      Revenue: decimal
      Cost: decimal
      Profit: decimal
    }

// Function to read JSON file
let readJsonFileAsync (filePath: string) : Async<list<Sale>> =
    async {
        let! jsonData = File.ReadAllTextAsync(filePath) |> Async.AwaitTask
        printf $"Json Data: %s{jsonData}"
        return JsonConvert.DeserializeObject<list<Sale>>(jsonData)
    }

// Write headers and deserialize into a list of Sale
let writeHeaders (worksheet: IWorksheet) =
    worksheet.Cells.[0, 0].Value <- "Statistics"
    worksheet.Cells.[1, 0].Value <- "Id"
    worksheet.Cells.[1, 1].Value <- "Employee Id"
    worksheet.Cells.[1, 2].Value <- "Revenue"
    worksheet.Cells.[1, 3].Value <- "Cost"
    worksheet.Cells.[1, 4].Value <- "Profit"
    
    // Style for title
    let titleCellRange = worksheet.Cells.[0,0,0,4]
    titleCellRange.Merge()
    titleCellRange.HorizontalAlignment <- HAlign.Center
    titleCellRange.VerticalAlignment <- VAlign.Center
    titleCellRange.Font.Bold <- true
    titleCellRange.Font.Size <- titleCellRange.Font.Size + 2.5
    titleCellRange.Font.Color <- Colors.Red
    
    // Style for headers
    let headerCellRange = worksheet.Cells.[1, 0, 1, 4]
    headerCellRange.HorizontalAlignment <- HAlign.Center
    headerCellRange.VerticalAlignment <- VAlign.Center
    headerCellRange.Font.Bold <- true
    headerCellRange.Font.Size <- headerCellRange.Font.Size + 1.
    headerCellRange.Font.Color <- Colors.Blue
    headerCellRange.Borders.LineStyle <- LineStyle.Continuous
    headerCellRange.Borders.Color <- Colors.Black
    
// Function to write sales data to the worksheet
let lastHeaderRow = 2;
let writeData (worksheet: IWorksheet) (sales: list<Sale>) =
    sales
    |> List.iteri (fun i sale ->
        let row = (+) i lastHeaderRow
        worksheet.Cells.[row, 0].Value <- sale.Id
        worksheet.Cells.[row, 1].Value <- sale.EmployeeId
        worksheet.Cells.[row, 2].Value <- sale.Revenue
        worksheet.Cells.[row, 3].Value <- sale.Cost
        worksheet.Cells.[row, 4].Value <- sale.Profit
      )
    
    let totalRowData = sales.Length + 2;
    // Calculate total of revenue and profit
    let totalLabelCellRange = worksheet.Cells[totalRowData, 0, totalRowData, 1]
    totalLabelCellRange.Merge()
    totalLabelCellRange.HorizontalAlignment <- HAlign.Center
    totalLabelCellRange.VerticalAlignment <- VAlign.Center
    totalLabelCellRange.Value <- "Total"
    totalLabelCellRange.Font.Size <- totalLabelCellRange.Font.Size + 1.
    totalLabelCellRange.Font.Bold <- true
    totalLabelCellRange.Font.Color <- Colors.Red
    
    // Formulas for total Revenue, Cost, and Profit
    worksheet.Cells.[totalRowData, 2].Formula <- $"=SUM(C{(lastHeaderRow+1)}:C{totalRowData})"
    worksheet.Cells.[totalRowData, 3].Formula <- $"=SUM(D{(lastHeaderRow+1)}:D{totalRowData})"
    worksheet.Cells.[totalRowData, 4].Formula <- $"=SUM(E{(lastHeaderRow+1)}:E{totalRowData})"
    
    // Style for total values
    let totalValueRange = worksheet.Cells[totalRowData, 2, totalRowData, 4]
    totalValueRange.Font.Bold <- true
    totalValueRange.Font.Size <- totalValueRange.Font.Size + 1.
    
    // Apply currency format
    let currencyRange = worksheet.Cells.[lastHeaderRow, 2, totalRowData, 4]
    currencyRange.NumberFormat <- "$#,##0"
    
    // Style data range
    let dataCellRange = worksheet.Cells.[1, 0, totalRowData, 4]
    dataCellRange.Borders.LineStyle <- LineStyle.Continuous
    dataCellRange.Borders.Color <- Colors.Black
    dataCellRange.Columns.AutoFit()

// Function to create a sales chart in the worksheet
let createChart (worksheet: IWorksheet) (sales: list<Sale>) =
    let windowInfo = worksheet.WindowInfo

    // Set the chart's position and size
    let left = windowInfo.ColumnToPoints(6.)
    let top = windowInfo.RowToPoints(2.)

    // Add a chart shape
    let chartShape = worksheet.Shapes.AddChart(left, top, 600, 300)
    let chart = chartShape.Chart

    // Set the chart's source data
    let dataSourceRange = worksheet.Cells.[1, 1, sales.Length + 1 , 4]
    chart.SetSourceData(dataSourceRange, SpreadsheetGear.Charts.RowCol.Columns)

    // Set the chart type
    chart.ChartType <- SpreadsheetGear.Charts.ChartType.ColumnClustered

    // Add a chart title
    chart.HasTitle <- true
    chart.ChartTitle.Text <- "Sales Report"
    chart.ChartTitle.Font.Size <- 12

    // Configure legend
    chart.Legend.Position <- SpreadsheetGear.Charts.LegendPosition.Bottom
    chart.Legend.Font.Bold <- true
    
// Function to write sales data to an Excel file
let writeToExcelAsync (sales: list<Sale>) (outputPath: string) : Async<Unit> =
    async {
        // Create a new workbook
        let workbook = Factory.GetWorkbook()
        let worksheet = workbook.Worksheets.[0]
        
        // Set the worksheet name and turn off the default gridlines.
        worksheet.Name <- "Sales Report";
        worksheet.WindowInfo.DisplayGridlines <- false;
        
        // Write headers and data to the worksheet
        writeHeaders worksheet
        writeData worksheet sales
        
        // Create a chart to the worksheet
        createChart worksheet sales
        
        // Save the workbook as an Excel file
        do! Task.Run(fun () -> workbook.SaveAs(outputPath, FileFormat.OpenXMLWorkbook)) |> Async.AwaitTask
    }

// Main function to run the program
let inputJsonFile = "Data.json"
let outputXlsxFile = "Output.xlsx"

let resultAsync =
    async {
        try
            let! salesData = readJsonFileAsync inputJsonFile
            do! writeToExcelAsync salesData outputXlsxFile
            printfn $"Data successfully imported to %s{outputXlsxFile}"
        with
            | :? FileNotFoundException as ex ->
                printfn $"File not found: %s{ex.Message}"
            | :? JsonException as ex ->
                printfn $"Error parsing JSON: %s{ex.Message}"
            | ex ->
                printfn $"An unexpected error occurred: %s{ex.Message}"
    }
    |> Async.RunSynchronously