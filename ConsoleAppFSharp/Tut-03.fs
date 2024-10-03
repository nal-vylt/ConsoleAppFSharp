namespace FSharpBasics

open System

module Program =
    [<EntryPoint>]
    let main args =
        printf "Hi, What is your name?"
        let name = Console.ReadLine()
        printf $"Hi %s{name} \n"
        
        let currentTime () = DateTime.Now
        currentTime ()
        |> printf("Time = %O")
        
        0