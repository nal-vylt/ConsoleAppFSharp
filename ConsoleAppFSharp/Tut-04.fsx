// record type
// tuple
// anonymous type

open System
open System.Collections.Generic

type Day = { DayOfTheMonth: int; Month: int }
type Person =
    { Name: string; Age: int }
    with
        static member (+) ({Name = n1; Age = a1},{Name = n2; Age = a2}) =
            {Name = n1 + n2; Age = a1 + a2}

let women = { Name = "Women"; Age = 30 }
let women' = { Name = "Women'"; Age = 29 }

let incrementAge person =
    { person with Age = person.Age + 1 }
    
incrementAge women

women > women'

type Duo = { Person1: Person; Person2: Person }

let brothers = { Person1 = women; Person2 = women' }

// tuple
(women, women')

// Anonymous record

let duo' = {|Person1 = women; Person2 = women'|}
let trio = {|duo' with Person3 = women|}

// Sum Types

// Discriminated unions

type Suit =
    | Hearts
    | Clubs
    | Diamonds

let yesOrNo bool =
    if bool
        then "Yes"
        else "No"

let yesOrNo' bool =
    match bool with
    | true -> "Yes"
    | false -> "No"
    
// function keyword
let yesOrNo'' = function
    | true -> "Yes"
    | false -> "No"
    
let isEven = function
    | x when x % 2 = 0 -> true
    | _ -> false
    
let translateFizzBuzz = function
    | "Fizz" -> string 3
    | "Buzz" -> string 5
    | "FizzBuzz" -> string 15
    | x -> x
    
translateFizzBuzz "FizzBuzz"
translateFizzBuzz "Dog"

type Rectangle = { Height: double; Base: double }
type Shape =
    | Rectangle of Rectangle
    | Triangle of height: double * _base: double
    | Circle of radius: double

module Shape =
    let area shape =
        match shape with
        | Rectangle rect -> rect.Height * rect.Base
        | Triangle (h,b) -> (h * b) / 2.
        | Circle r -> r ** 2. * System.Math.PI

let circle = Circle 1
Shape.area circle

type OrderId = OrderId of int

let incrementOrderId (OrderId id) =
    OrderId <| id + 1

let incrementOrderId' =
    fun (OrderId id) ->
        OrderId <| id + 1
        
 
// Option type
// Have a value or none
// type Option<'a> =
//     | Some of 'a
//     | None
    
    
let translateFizzBuzz' = function
    | "Fizz" -> Some 3
    | "Buzz" -> Some 5
    | "FizzBuzz" -> Some 15
    | x -> None
 
translateFizzBuzz' "Fizz"

let hasValue = function
    | Some _ -> true
    | None -> false

// Inline keyword
// Improve performance for small function
let inline add x y = x + y


// Collection

// Array
// Fixed size
// Mutable
[|1;2;3;4;5;6;7;8;9|]
let arr = [|1..10|]
arr.[0] <- 5

// List
// Immutable
// Linked list
[1;2;3;4;5;6]
[1..2..15]

// type LinkedList<'a> =
//     | ([])
//     | (::) of head:'a * tail:'a
    
let empty = []

let addToList x xs =
    x::xs

let sampleList = [2..4]
addToList 1 sampleList

let getFirstItemList = function
    | x::_ -> Some x
    | _ -> None

// List.head
let getFirstItemList' list =
    List.head

let x: int list = List.head []

let rec printEveryItem = function
    | x::xs ->
        printfn "%O" x
        printEveryItem xs
    | [] -> ()
    
printEveryItem [1..10]

let rec doWithEveryItem f = function
    | x::xs ->
        f x
        doWithEveryItem f xs
    | [] -> ()
    
let printEveryItem' list =
    doWithEveryItem (printfn "%O") list

printEveryItem' [1..10]

// List.iter
// for each
let printEveryItem'' list =
    list
        |> List.iter (printfn "%O")
        
// List.map
let stringifyList (list:int list) =
    list
    |> List.map string

[1..10]
|> stringifyList

// List.fold
let sumList list =
    list
    // |> List.fold (fun accumulator currentItem -> accumulator + currentItem)
    |> List.fold (+) 0
    
[1..10]
|> sumList

// List.reduce
let reduceList list =
    list
    |> List.reduce(+)

// List.sum
let inline sumList' list =
    List.sum list
    
[1..10]
|> sumList'

// bind
let divideInteger numerator denominator =
    match numerator % denominator with
    | 0 -> Some <| numerator / denominator
    | _ -> None
    
let divideBy2 = divideInteger 2

let bind f = function
    | Some x -> f x
    | None -> None 

let divide x y =
    if y = 0 then None
    else Some (x / y)

let safeDivide a b =
    Option.bind (divide a) b

let result1 = safeDivide 10 (Some 2)  // Some 5
let result2 = safeDivide 10 (Some 0)  // None

printfn $"Result 1: %A{result1}"
printfn $"Result 2: %A{result2}"


// Exception
exception CannotConnectException of System.Uri

let handleException f =
    try
        f()
    with
    | CannotConnectException uri -> ()
    | :? System.ArgumentException as e -> printf "%s" e.Message    

raise (CannotConnectException (Uri("http://google.ca")))

type WithdrawalError =
    | InsufficientFunds of double
    | WrongPIN

type Result<'Ok, 'Error> =
    | Ok of 'Ok
    | Error of 'Error
    
type Option<'Ok, 'Error> =
    | Some of 'Ok
    | None
let result = Error (InsufficientFunds 10.)


    
    






        
