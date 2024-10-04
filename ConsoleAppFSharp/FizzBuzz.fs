namespace FSharpBasics

open System
open Library

// Main program
module Program =
    [<EntryPoint>]
    let main args =
        Application.application Console.ReadLine Console.WriteLine ()
        0
        