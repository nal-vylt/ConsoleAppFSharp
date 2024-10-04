open System.Net.Http
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core
open System.IO

let work a b =
    async {
        do! Async.Sleep 2000
        return a + b
    }
    
let workTask = work 10 20 |> Async.StartAsTask

let resultAsync = async {
    let! result = workTask |> Async.AwaitTask
    do printf $"The result is {result}"
}

resultAsync |> Async.Start
// Start Functions
    // Async.Start - Fire and forget on thread pool
    // Async.StartImmediate - Fire and forget on current thread
    // Async.StartChild - More like async parallel
    // Async.StartAsTask - Fire a task on thread pool
    // Async.RunSynchronously - Fire and await sync on current thread

printf "Did this print first?"


let fetchDataAsync (url: string) : Async<string> =
    async {
        use client = new HttpClient()
        let! response = client.GetStringAsync(url) |> Async.AwaitTask
        return response
    }

let processDataAsync (url: string) : Async<Unit> =
    async {
        do! fetchDataAsync url |> Async.Ignore  // Use do! to wait for completion
        printfn $"Data fetched from %s{url}"
    }
    
let url = "https://api.github.com"
Async.RunSynchronously (processDataAsync url)

// Reading content from file
let asyncRead path = async {
    do printf $"Reading content from file {path}"
    let content = File.ReadAllText path
    do printf $"Read content {content} from file {path}"
    return content
}

let asyncWrite path content =
    async {
        do printf $"Writing content {content} to file {path}"
        do File.WriteAllText (path,content)
        do printfn $"Finished writing content {content} to the file {path}"
    }

let [<Literal>] File = "file"

let fileName num = $"{File}{num}.txt"

seq {1..10}
|> Seq.map (fun num -> fileName num, string num)
|> Seq.map (fun (fileName, content) -> asyncWrite fileName content)
|> Async.Parallel
|> Async.Ignore
|> Async.Start