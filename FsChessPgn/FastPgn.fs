namespace PgnLib

open System
open System.Text
open System.IO
open System.Text.RegularExpressions

module FastPgn =
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
    
        let rec docomm cl (s : string) = 
            if s = "" then InComment(cl), ""
            else 
                let c = s.[0]
            
                let ncl = 
                    if c = '}' then cl - 1
                    elif c = '{' then cl + 1
                    else cl
                if ncl = 0 then Unknown, s.[1..]
                else docomm ncl s.[1..]
    
        let rec dorav cl (s : string) = 
            if s = "" then InRAV(cl), ""
            else 
                let c = s.[0]
            
                let ncl = 
                    if c = ')' then cl - 1
                    elif c = '(' then cl + 1
                    else cl
                if ncl = 0 then Unknown, s.[1..]
                else dorav ncl s.[1..]
    
        let getwd (s : string) = 
            let eloc = s.IndexOf(" ")
        
            let tok = 
                if eloc = -1 then s
                else s.[..eloc - 1]
        
            let es = 
                if eloc = -1 then ""
                else s.[eloc + 1..]
        
            tok, es

        let getnm (s : string) = 
            let eloc = s.IndexOf(".")
        
            let tok = 
                if eloc = -1 then s
                else s.[..eloc - 1]
        
            let es = 
                if eloc = -1 then ""
                else s.[eloc + 1..]
        
            tok, es

    
        let donag s = 
            let _, es = s |> getwd
            es
    
        let isEnd s = s = "1/2-1/2" || s = "1-0" || s = "0-1"
    
        let donum s = 
            let tok, es = s |> getnm
            if tok |> isEnd then FinishedOK, es
            else Unknown, es
    
        let rec doinv (s : string) = 
            if s = "" then Invalid
            else 
                let tok, es = s |> getwd
                if tok |> isEnd then FinishedInvalid
                else doinv es
    
        let dohdr (s : string) = 
            //Active pattern to parse header string
            let (|Header|_|) s = 
                let m = Regex("\[([\w]+)\s+\"([\s\S]*)\"\]").Match(s)
                if m.Success then Some(m.Groups.[1].Value, m.Groups.[2].Value)
                else None
        
            let eloc = s.IndexOf("]")
            if eloc = -1 then Invalid, ""
            else 
                let tok = s.[..eloc - 1]
                let es = s.[eloc + 1..]
            
                let k, v = 
                    match ("[" + tok + "]") with
                    | Header(k, v) -> k, v
                    | _ -> failwith ("not a valid pgn header: " + "[" + tok + "]")
                if k = "FEN" then Invalid, es
                else 
                    Unknown, es
    
        let domv s = 
            let tok, es = s |> getwd
            es
    
        let rec proclin st s = 
            if s = "" then st
            else 
                match st with
                | InComment(cl) -> 
                    let nst, ns = docomm cl s
                    proclin nst ns
                | InRAV(cl) -> 
                    let nst, ns = dorav cl s
                    proclin nst ns
                | InNAG -> 
                    let ns = s |> donag
                    proclin Unknown ns
                | InNum -> 
                    let nst, ns = s |> donum
                    proclin nst ns
                | Invalid -> 
                    let nst = s |> doinv
                    nst
                | InHeader -> 
                    let nst, ns = dohdr s
                    proclin nst ns
                | InMove -> 
                    let ns = domv s
                    proclin Unknown ns
                | FinishedOK | FinishedInvalid -> st
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
                    proclin st ns
    
        let rec getgm st gm = 
            let lin = sr.ReadLine()
            if lin |> isNull then []
            else 
                let ngm = lin::gm
                let nst = proclin st lin
                if nst = FinishedOK then ngm |> List.rev
                elif nst = FinishedInvalid then []
                else getgm nst ngm
    
        let gm = getgm Unknown []
        if gm.Length > 0 then gm |> Some
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
    

