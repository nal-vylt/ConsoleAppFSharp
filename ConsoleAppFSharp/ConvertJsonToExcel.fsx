#r "nuget: Newtonsoft.Json"
#r "nuget: SpreadsheetGear"

open System
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

// Function to read Excel template file
let readExcelFile (filePath: string) : IWorkbook =
    let workbook = Factory.GetWorkbook(filePath)
    workbook
    
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
    
    let lastDataRow = sales.Length + 1;
    
    // Formulas for total Revenue, Cost, and Profit
    worksheet.Cells.["G3"].Formula <- $"=SUM(C{(lastHeaderRow+1)}:C{lastDataRow+1})"
    worksheet.Cells.["H3"].Formula <- $"=SUM(D{(lastHeaderRow+1)}:D{lastDataRow+1})"
    worksheet.Cells.["I3"].Formula <- $"=SUM(E{(lastHeaderRow+1)}:E{lastDataRow+1})"
    
    // Apply currency format
    let currencyRange = worksheet.Cells.[lastHeaderRow, 2, lastDataRow, 4]
    currencyRange.NumberFormat <- "$#,##0"
    
    let totalRange = worksheet.Cells.["G3:I3"]
    totalRange.NumberFormat <- "$#,##0"
    totalRange.Columns.AutoFit()
    
    // Style data range
    let dataCellRange = worksheet.Cells.[1, 0, lastDataRow, 4]
    dataCellRange.Borders.LineStyle <- LineStyle.Continuous
    dataCellRange.Borders.Color <- Colors.Black
    dataCellRange.Columns.AutoFit()

// Function to create a sales chart in the worksheet
let createChart (worksheet: IWorksheet) (sales: list<Sale>) =
    let windowInfo = worksheet.WindowInfo

    // Set the chart's position and size
    let left = windowInfo.ColumnToPoints(10.)
    let top = windowInfo.RowToPoints(2.)

    // Add a chart shape
    let chartShape = worksheet.Shapes.AddChart(left, top, 500, 300)
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
let updateExcelTemplateAsync
    (sales: list<Sale>)
    (excelTemplatePath: string)
    (outputPath: string) : Async<Unit> =
    async {
        // Read template file
        let workbook = readExcelFile excelTemplatePath
        let worksheet = workbook.Worksheets.[0]
        
        // Write data to the worksheet
        writeData worksheet sales
        
        // Create a chart to the worksheet
        createChart worksheet sales
        
        // Save the workbook as an Excel file
        do! Task.Run(fun () -> workbook.SaveAs(outputPath, FileFormat.OpenXMLWorkbook)) |> Async.AwaitTask
    }

// Main function to run the program
let inputJsonFile = "Data.json"
let excelTemplateFile = "Template.xlsx"

let datetime = DateTime.UtcNow.ToString("yyyyMMdd");
let outputXlsxFile = $"Output_{datetime}.xlsx"

let resultAsync =
    async {
        try
            let! salesData = readJsonFileAsync inputJsonFile
            do! updateExcelTemplateAsync salesData excelTemplateFile outputXlsxFile
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