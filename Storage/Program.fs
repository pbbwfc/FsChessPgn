open System
open Storage

[<EntryPoint>]
let main argv =
    let pgn = "recent10000.pgn"
    let s0 = DateTime.Now
    let igms0 = pgn|>T0.ReadIndexListFromFile
    let e0 = DateTime.Now
    let el0 = e0-s0
    Console.WriteLine("el0: " + el0.ToString())
    let s1 = DateTime.Now
    pgn|>T1.ReadFromPgn
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    ("T1_" + pgn)|>T1.ExportPgn
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    ("T1_" + pgn)|>T1.Save
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())
    let s1 = DateTime.Now
    ("T1_" + pgn)|>T1.Load
    let e1 = DateTime.Now
    let el1 = e1-s1
    Console.WriteLine("el1: " + el1.ToString())

    let str = FsChessPgn.MoveUtil.Descs (T1.CurPgn.Mvss.[0]|>List.ofArray) FsChess.Board.Start true
 
    0 // return an integer exit code
