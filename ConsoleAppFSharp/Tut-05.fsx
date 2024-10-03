// for to

open Microsoft.FSharp.Core

for i = 0 to 20 do
    printf $"i : %i{i} \n"

// for downto
for i = 20 downto 1 do
    printf $"i : %i{i} \n"
 
// for in
let list = [1..20]
for i in list do
    printf $"i : %i{i} \n"
 
// while do
let mutable a = 10
while (a < 20) do
    printf $"a: %i{a} \n"
    a <- a + 1
 
 // function
let rec fib n = if n < 2 then 1 else fib (n - 1) + fib (n - 2)
for i = 1 to 10 do
   printfn $"Fibonacci %d{i}: %d{fib i}"

// List
// cons (::) Operator
let list' = 1::2::3::4::5::6::7::8::9::10::[];;

(* using init method *)
let list'' = List.init 5 (fun index -> (index, index * index, index * index * index))

// yield operator
// As the yield keyword pushes a single value into a list
let list2 = [ for a in 1 .. 10 do yield (a * a) ]

// using yield! operator
// yield!, pushes a collection of values into the list.
let list8 = [for a in 1 .. 3 do yield! [ a .. a + 3 ] ]

// Sequence
let emptySeq = Seq.empty
let seq1 = Seq.singleton 20

printfn"The singleton sequence:"
printfn "%A " seq1
printfn"The init sequence:"

let seq2 = Seq.init 5 (fun n -> n * 3)
Seq.iter (fun i -> printf "%d " i) seq2
printfn""

// Converting an array to sequence by using cast 
printfn"The array sequence 1:"
let seq3 = [| 1 .. 10 |] :> seq<int>
Seq.iter (fun i -> printf "%d " i) seq3
printfn""

// Converting an array to sequence by using Seq.ofArray 
printfn"The array sequence 2:"
let seq4 = [| 2..2.. 20 |] |> Seq.ofArray
Seq.iter (fun i -> printf "%d " i) seq4
printfn""

// Set
// Do not allow duplicate entries to be inserted into the collection.
let setA = Set.ofSeq [ 1 ..2.. 20 ]
let setB = Set.ofSeq [ 1 ..3 .. 20 ]
let setC = Set.intersect setA setB
let setD = Set.union setA setB
let setE = Set.difference setA setB

printfn "Set a: "
Set.iter (fun x -> printf $"{x} ") setA
printfn""

// Maps
// A map is a special kind of set that associates the values with key

// Create a map
let students =
    Map.empty
        .Add("Zara Ali", "1501")
        .Add("Rishita Gupta", "1502")
        .Add("Robin Sahoo", "1503")
        .Add("Gillian Megan", "1504")

// Convert a list to Map
let capitals =
   [ "Argentina", "Buenos Aires";
      "France ", "Paris";
      "Chili", "Santiago";
      "Malaysia", " Kuala Lumpur";
      "Switzerland", "Bern" ]
   |> Map.ofList

// Find the element in the Map
let found = capitals.TryFind "Argentina"
match found with
| Some x -> printfn $"Found %s{x}."
| None -> printfn "Did not find the specified value."

// Array
// Using create and set 
let array1 = Array.create 10 ""
for i in 0 .. array1.Length - 1 do
   Array.set array1 i (i.ToString())
for i in 0 .. array1.Length - 1 do
   printf "%s " (Array.get array1 i)
printfn " "

// using the init and zeroCreate
let array2 = Array.init 10 (fun index -> index * index)
printfn $"Array of squares: %A{array2}"

let array3 : float array = Array.zeroCreate 10
let (myZeroArray : float array) = Array.zeroCreate 10
printfn $"Float Array: %A{array3}"

// Sub Array
let array1' = [| 0 .. 50 |]
// array startIndex count
let array2' = Array.sub array1' 5 15

// Append Array
let array3' = [| 1; 2; 3; 4; 9|]
let array4' = [| 5 .. 9 |]
let array5' = Array.append array3' array4'

// Choose function
let array6 = [| 1 .. 20 |]
let array7 =
    Array.choose
        (fun elem -> if elem % 3 = 0 then Some(float (elem)) else None)
        array6

// Mutable List
let booksList = System.Collections.Generic.List<string>()
booksList.Add("Gone with the Wind")
booksList.Add("Atlas Shrugged")
booksList.Add("Fountainhead")
booksList.Add("Thorn birds")
booksList.Add("Rebecca")
booksList.Add("Narnia")

booksList |> Seq.iteri (fun index item -> printfn $"%i{index}: %s{booksList.[index]}")
booksList.Insert(2, "Roots")

// Mutable Dictionary
let dict = System.Collections.Generic.Dictionary<string, string>()
dict.Add("1501", "Zara Ali")
dict.Add("1502","Rishita Gupta")
dict.Add("1503","Robin Sahoo")
dict.Add("1504","Gillian Megan")
printfn $"Dictionary - students: %A{dict}"

// Delegate
type MyClass() =
   static member add(a : int, b : int) =
      a + b
   static member sub (a : int) (b : int) =
      a - b
   member x.Add(a : int, b : int) =
      a + b
   member x.Sub(a : int) (b : int) =
      a - b

// Delegate1 works with tuple arguments.
type Delegate1 = delegate of (int * int) -> int

// Delegate2 works with curried arguments.
type Delegate2 = delegate of int * int -> int

let InvokeDelegate1 (dlg : Delegate1) (a : int) (b: int) =
   dlg.Invoke(a, b)
let InvokeDelegate2 (dlg : Delegate2) (a : int) (b: int) =
   dlg.Invoke(a, b)
let del1 : Delegate1 = new Delegate1( MyClass.add )
let del2 : Delegate2 = new Delegate2( MyClass.sub )
let mc = MyClass()
let del3 : Delegate1 = new Delegate1( mc.Add )
let del4 : Delegate2 = new Delegate2( mc.Sub )

for (a, b) in [ (400, 200); (100, 45) ] do
   printfn $"%d{a} + %d{b} = %d{InvokeDelegate1 del1 a b}"
   printfn $"%d{a} - %d{b} = %d{InvokeDelegate2 del2 a b}"
   printfn $"%d{a} + %d{b} = %d{InvokeDelegate1 del3 a b}"
   printfn $"%d{a} - %d{b} = %d{InvokeDelegate2 del4 a b}"
   
// Enumeration
type Days =
   | Sun = 0
   | Mon = 1
   | Tues = 2
   | Wed = 3
   | Thurs = 4
   | Fri = 5
   | Sat = 6

// Use of an enumeration
let weekend' : Days = Days.Sat
let weekend'' : Days = Days.Sun

// Classes
// Struct
type Line =
   struct
      val X1 : float
      val Y1 : float
      val X2 : float
      val Y2 : float
      
      new (x1, y1, x2, y2) =
         {X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;}
end

let calcLength(a : Line)=
   let sqr a = a * a
   sqrt(sqr(a.X1 - a.X2) + sqr(a.Y1 - a.Y2) )

let aLine = new Line(1.0, 1.0, 4.0, 5.0)
let length = calcLength aLine

//implementing a complex class with +, and - operators
//overloaded
type Complex(x: float, y : float) =
   member this.x = x
   member this.y = y
   //overloading + operator
   static member (+) (a : Complex, b: Complex) =
      Complex(a.x + b.x, a.y + b.y)

   //overloading - operator
   static member (-) (a : Complex, b: Complex) =
      Complex(a.x - b.x, a.y - b.y)

   // overriding the ToString method
   override this.ToString() =
      this.x.ToString() + " " + this.y.ToString()
      
// Inheritance
type Person(name) =
   member x.Name = name
   member x.Greet() = printfn $"Hi, I'm %s{x.Name}"

type Student(name, studentID : int) =
   inherit Person(name)
   let mutable _GPA = 0.0
   member x.StudentID = studentID
   member x.GPA
      with get() = _GPA
      and set value = _GPA <- value

type Teacher(name, expertise : string) =
   inherit Person(name)
   let mutable _salary = 0.0
   member x.Salary
      with get() = _salary
      and set value = _salary <- value
   member x.Expertise = expertise

//using the subclasses
let p = new Person("Mohan")
let st = new Student("Zara", 1234)
let tr = new Teacher("Mariam", "Java")

p.Greet()
st.Greet()
tr.Greet()

// Abstract class
[<AbstractClass>]
type Person'(name) =
   member x.Name = name
   abstract Greet : unit -> unit

type Student'(name, studentID : int) =
   inherit Person'(name)
   let mutable _GPA = 0.0
   member x.StudentID = studentID
   member x.GPA
      with get() = _GPA
      and set value = _GPA <- value
   override x.Greet() = printfn $"Student %s{x.Name}"

let st' = new Student'("Zara", 1234)

//Overriden Greet
st'.Greet()

// Interface
type IPerson =
   abstract Name : string
   abstract Enter : unit -> unit
   abstract Leave : unit -> unit
   
type Student''(name : string, id : int) =
   member this.ID = id
   interface IPerson with
      member this.Name = name
      member this.Enter() = printfn "Student entering premises!"
      member this.Leave() = printfn "Student leaving premises!"
     
let s = new Student''("Zara", 1234)
(s :> IPerson).Enter()

// Inherit multiple interface
type Interface1 =
   abstract member doubleIt: int -> int

type Interface2 =
   abstract member tripleIt: int -> int

type Interface3 =
   inherit Interface1
   inherit Interface2
   abstract member printIt: int -> string

type multiplierClass() =
   interface Interface3 with
      member this.doubleIt(a) = 2 * a
      member this.tripleIt(a) = 3 * a
      member this.printIt(a) = a.ToString()

let ml = multiplierClass()
printfn $"%d{(ml:>Interface3).tripleIt(5)}"

// Event
type Worker(name : string, shift : string) =
   let mutable _name = name;
   let mutable _shift = shift;

   let nameChanged = new Event<unit>() (* creates event *)
   let shiftChanged = new Event<unit>() (* creates event *)

   member this.NameChanged = nameChanged.Publish (* exposed event handler *)
   member this.ShiftChanged = shiftChanged.Publish (* exposed event handler *)

   member this.Name
      with get() = _name
      and set(value) = 
         _name <- value
         nameChanged.Trigger() (* invokes event handler *)

   member this.Shift
      with get() = _shift
      and set(value) = 
         _shift <- value
         shiftChanged.Trigger() (* invokes event handler *)

let wk = new Worker("Wilson", "Evening")
wk.NameChanged.Add(fun () -> printfn $"Worker changed name! New name: %s{wk.Name}")
wk.Name <- "William"
wk.NameChanged.Add(fun () -> printfn "-- Another handler attached to NameChanged!")
wk.Name <- "Bill"

wk.ShiftChanged.Add(fun () -> printfn $"Worker changed shift! New shift: %s{wk.Shift}")
wk.Shift <- "Morning"
wk.ShiftChanged.Add(fun () -> printfn "-- Another handler attached to ShiftChanged!")
wk.Shift <- "Night"
   
        
