#r "nuget: Newtonsoft.Json"
#r "nuget: SpreadsheetGear, 8.4.4"

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

// set spreads sheet gear license
let setSpreadsheetGearLicense() =
        try
            SpreadsheetGear.Factory.SetSignedLicense("")
        with
        | ex when ex.Message.Contains("must be the first method called in the Factory class") ->()
        | ex -> eprintfn $"error signing license %s{ex.Message}"

// Function to read JSON file
let readJsonFileAsync (filePath: string) : Async<list<Sale>> =
    async {
        let! jsonData = File.ReadAllTextAsync(filePath) |> Async.AwaitTask
        return JsonConvert.DeserializeObject<list<Sale>>(jsonData)
    }

// Function to read an Excel template file
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
        
        // Check if this is the last item then delete the next row
        if (i = sales.Length - 1) then
            let entireRow = worksheet.Cells.[row + 1, 0]
            entireRow.EntireRow.Delete()
        // Insert a new row
        else
            let row = worksheet.Cells.[row + 1, 0, row + 1, 4]
            row.EntireRow.Insert(InsertShiftDirection.Down)
      )
    
    let lastDataRow = sales.Length + 1;
    
    // Style data range
    let dataCellRange = worksheet.Cells.[1, 0, lastDataRow + 1, 4]
    dataCellRange.Columns.AutoFit()

// Function to create a sales chart in the worksheet
let createChart (worksheet: IWorksheet) (sales: list<Sale>) =
    let windowInfo = worksheet.WindowInfo

    // Set the chart's position and size
    let left = windowInfo.ColumnToPoints(6.)
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
let generateExcelFileAsync
    (sales: list<Sale>)
    (excelTemplatePath: string)
    (outputPath: string) : Async<Unit> =
    async {
        // Set spread sheet gear license
        setSpreadsheetGearLicense()
        
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
            
            if salesData.Length <= 0 then printf "Don't have any data to export!"
            
            do! generateExcelFileAsync salesData excelTemplateFile outputXlsxFile
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