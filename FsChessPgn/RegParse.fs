namespace FsChessPgn.Data

open System
open System.Text
open System.IO
open System.Text.RegularExpressions

module RegParse = 
    type private State = 
        | Unknown
        | InHeader
        | InMove
        | InComment of int
        | InRAV of int
        | InNAG
        | InNum
        | FinishedOK
        | Invalid
        | FinishedInvalid
    
    let private NextGameRdr(sr : StreamReader) = 

        let rec proclin st cstr s gm = 
            if s = "" then st, gm
            else 
                let hd = s.[0]
                let tl = s.[1..]
                match st with
                | InComment(cl) -> 
                    proclin st cstr tl gm
                | InRAV(cl) -> 
                    proclin st cstr tl gm
                | InNAG -> 
                    proclin st cstr tl gm
                | InNum -> 
                    proclin st cstr tl gm
                | Invalid -> 
                    proclin st cstr tl gm
                | InHeader -> 
                    proclin st cstr tl gm
                | InMove -> 
                    proclin st cstr tl gm
                | FinishedOK | FinishedInvalid -> st, gm
                | Unknown -> 
                    let st, ns = 
                        match s.[0] with
                        | '[' -> InHeader, s.[1..]
                        | '{' -> InComment(1), s.[1..]
                        | '(' -> InRAV(1), s.[1..]
                        | '$' -> InNAG, s.[1..]
                        | '*' -> FinishedOK, s.[1..]
                        | c when System.Char.IsNumber(c) || c = '.' -> InNum, s
                        | ' ' -> Unknown, s.[1..]
                        | _ -> InMove, s
                    proclin st cstr tl gm
    
        let rec getgm st gm = 
            let lin = sr.ReadLine()
            if lin |> isNull then { gm with MoveText = (gm.MoveText |> List.rev) }
            else 
                let nst, ngm = proclin st "" lin gm
                if nst = FinishedOK then { ngm with MoveText = (ngm.MoveText |> List.rev) }
                elif nst = FinishedInvalid then GameEMP
                else getgm nst ngm
    
        let gm = getgm Unknown GameEMP
        if gm.MoveText.Length > 0 then gm |> Some
        else None
    
    let private AllGamesRdr(sr : System.IO.StreamReader) = 
        seq { 
            while not sr.EndOfStream do
                let gm = NextGameRdr(sr)
                if gm.IsSome then yield gm.Value
        }
    
    let private ReadFromStream(stream : Stream) = 
        let sr = new StreamReader(stream)
        let db = AllGamesRdr(sr)
        db
    
    let ReadFromFile(file : string) = 
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadFromString(str : string) = 
        let byteArray = Encoding.ASCII.GetBytes(str)
        let stream = new MemoryStream(byteArray)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadOneFromString(str : string) = 
        let gms = str|>ReadFromString
        gms.Head

