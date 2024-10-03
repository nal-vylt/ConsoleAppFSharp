#r "nuget: Newtonsoft.Json"
#r "nuget: SpreadsheetGear"

open System.IO
open Newtonsoft.Json
open SpreadsheetGear
open System.Threading.Tasks

type Person =
    {
      Id: int
      FullName: string
      Age: int
      PhoneNumber: string
      Email: string
      Revenue: decimal
      Profit: int
      Department: string
    }

// Function to read JSON file
let readJsonFileAsync (filePath: string) : Async<Person[]> =
    async {
        let! jsonData = File.ReadAllTextAsync(filePath) |> Async.AwaitTask
        printf $"Json Data: %s{jsonData}"
        return JsonConvert.DeserializeObject<Person[]>(jsonData)
    }

// Write headers
let writeHeaders (worksheet: IWorksheet) =
    worksheet.Cells.[0, 0].Value <- "Statistics"
    worksheet.Cells.[1, 0].Value <- "Id"
    worksheet.Cells.[1, 1].Value <- "Full Name"
    worksheet.Cells.[1, 2].Value <- "Age"
    worksheet.Cells.[1, 3].Value <- "Phone Number"
    worksheet.Cells.[1, 4].Value <- "Email"
    worksheet.Cells.[1, 5].Value <- "Department"
    worksheet.Cells.[1, 6].Value <- "Revenue"
    worksheet.Cells.[1, 7].Value <- "Profit"
    
    // Style for headers
    let title = worksheet.Cells.[0,0,0,7]
    title.Merge()
    title.HorizontalAlignment <- HAlign.Center
    title.VerticalAlignment <- VAlign.Center
    title.Font.Bold <- true
    title.Font.Size <- title.Font.Size + 2.5
    title.Font.Color <- Colors.Red
    
    let headerRange = worksheet.Cells.[1, 0, 1, 7]
    headerRange.HorizontalAlignment <- HAlign.Center
    headerRange.VerticalAlignment <- VAlign.Center
    headerRange.Font.Bold <- true
    headerRange.Font.Size <- headerRange.Font.Size + 1.
    headerRange.Font.Color <- Colors.Blue
    headerRange.Borders.LineStyle <- LineStyle.Continuous
    headerRange.Borders.Color <- Colors.Black
    
// Write data
let writeData (worksheet: IWorksheet) (data: Person[]) =
    for i in 0 .. data.Length - 1 do
        let row = (+) i 2
        worksheet.Cells.[row, 0].Value <- data.[i].Id
        worksheet.Cells.[row, 1].Value <- data.[i].FullName
        worksheet.Cells.[row, 2].Value <- data.[i].Age
        worksheet.Cells.[row, 3].Value <- data.[i].PhoneNumber
        worksheet.Cells.[row, 4].Value <- data.[i].Email
        worksheet.Cells.[row, 5].Value <- data.[i].Department
        worksheet.Cells.[row, 6].Value <- data.[i].Revenue
        worksheet.Cells.[row, 7].Value <- data.[i].Profit
        
    // Apply currency format for Revenue and Profit columns
    let revenueRange = worksheet.Cells.[2, 6, data.Length + 1, 7]
    revenueRange.NumberFormat <- "$#,##0"
    
    // Style data
    let dataRange = worksheet.Cells.[1, 0, data.Length + 1, 7]
    dataRange.Borders.LineStyle <- LineStyle.Continuous
    dataRange.Borders.Color <- Colors.Black
    dataRange.Columns.AutoFit()

// Function to create a chart
let createChart (worksheet: IWorksheet) (data: Person[]) =
    let windowInfo = worksheet.WindowInfo

    // Set the chart's position and size
    let left = windowInfo.ColumnToPoints(9.)
    let top = windowInfo.RowToPoints(2.)

    // Add a chart shape
    let chartShape = worksheet.Shapes.AddChart(left, top, 400, 300)
    let chart = chartShape.Chart

    // Set the chart's source data
    let source = worksheet.Cells.[1, 6, data.Length + 1 , 7]
    chart.SetSourceData(source, SpreadsheetGear.Charts.RowCol.Columns)

    // Set the chart type
    chart.ChartType <- SpreadsheetGear.Charts.ChartType.ColumnClustered

    // Add a chart title
    chart.HasTitle <- true
    chart.ChartTitle.Text <- "Revenue and Profit Report"
    chart.ChartTitle.Font.Size <- 12

    // Configure legend
    chart.Legend.Position <- SpreadsheetGear.Charts.LegendPosition.Bottom
    chart.Legend.Font.Bold <- true
// Function to write data to Excel file
let writeToExcelAsync (data: Person[]) (outputPath: string) : Async<Unit> =
    async {
        // Create a new workbook
        let workbook = Factory.GetWorkbook()
        let worksheet = workbook.Worksheets.[0]
        
        // Set the worksheet name and turn off the default gridlines.
        worksheet.Name <- "Sales Report";
        worksheet.WindowInfo.DisplayGridlines <- false;
        
        // Write headers and data to the worksheet
        writeHeaders worksheet
        writeData worksheet data
        createChart worksheet data
        
        // Save the workbook
        do! Task.Run(fun () -> workbook.SaveAs(outputPath, FileFormat.OpenXMLWorkbook)) |> Async.AwaitTask
    }

// Run the program
let inputJsonFile = "Data.json"
let outputXlsxFile = "Output.xlsx"

let resultAsync =
    async {
        try
            let! data = readJsonFileAsync inputJsonFile
            do! writeToExcelAsync data outputXlsxFile
            printfn $"Data successfully imported to %s{outputXlsxFile}"
        with
            | :? FileNotFoundException as ex ->
                printfn $"File not found: %s{ex.Message}"
            | :? JsonException as ex ->
                printfn $"Error parsing JSON: %s{ex.Message}"
            | ex ->
                printfn $"An unexpected error occurred: %s{ex.Message}"
    }

Async.RunSynchronously(resultAsync)


   
  