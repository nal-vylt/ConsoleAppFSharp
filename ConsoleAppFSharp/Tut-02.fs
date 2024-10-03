namespace FSharpBasics

// Module
module Arithmetic =
    module Addition =
        let add x y = x + y

module Other =
    open Arithmetic
    let program = Addition.add 10 3