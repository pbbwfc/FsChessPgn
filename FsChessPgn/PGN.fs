namespace PgnLib

open System
open System.Text
open System.IO
open System.Text.RegularExpressions

module PGN = 
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
    
    type GameResult = 
        | Draw
        | WhiteWins
        | BlackWins
        | NotKnown
        
        override x.ToString() = 
            match x with
            | Draw -> "1/2-1/2"
            | WhiteWins -> "1-0"
            | BlackWins -> "0-1"
            | NotKnown -> "*"
        
        member x.ToByte() = 
            match x with
            | Draw | NotKnown -> byte (1)
            | WhiteWins -> byte (2)
            | BlackWins -> byte (0)
        
        static member FromByte b = 
            match b with
            | b when b = byte (1) -> Draw
            | b when b = byte (2) -> WhiteWins
            | b when b = byte (0) -> BlackWins
            | _ -> failwith "invalid byte for GameResult"
    
    /// Game - holds key headers and list of moves
    type Game = 
        { Id : int
          Event : string
          Site : string
          Year : int16 option
          Month : byte option
          Day : byte option
          Round : string
          White : string
          Black : string
          Result : GameResult
          Moves : Move list }
        
        member x.DateStr = 
            (if x.Year.IsNone then "????"
             else x.Year.Value.ToString("0000")) + "." + (if x.Month.IsNone then "??"
                                                          else x.Month.Value.ToString("00")) + "." 
            + (if x.Day.IsNone then "??"
               else x.Day.Value.ToString("00"))
        
        override x.ToString() = 
            let nl = Environment.NewLine
            let sb = new StringBuilder()
            
            let rec mvs2txt ct mvl = 
                if List.isEmpty mvl then sb.ToString()
                elif mvl.Length = 1 then 
                    sb.Append(ct.ToString() + ". " + mvl.Head.ToString() + " ") |> ignore
                    sb.ToString()
                else 
                    let w = mvl.Head.ToString()
                    let b = mvl.Tail.Head.ToString()
                    let rest = mvl.Tail.Tail
                    sb.Append(ct.ToString() + ". " + w + " " + b + " ") |> ignore
                    mvs2txt (ct + 1) rest
            (x.Event |> Game.FormatTag "Event") + (x.Site |> Game.FormatTag "Site") 
            + (x.DateStr |> Game.FormatTag "Date") + (x.Round |> Game.FormatTag "Round") 
            + (x.White |> Game.FormatTag "White") + (x.Black |> Game.FormatTag "Black") 
            + (x.Result.ToString() |> Game.FormatTag "Result") + nl + (x.Moves |> mvs2txt 1) + x.Result.ToString()
        
        member x.AddHdr(t, v) = 
            match t with
            | "Event" -> { x with Event = v }
            | "Site" -> { x with Site = v }
            | "Date" -> 
                let b = v.Split('.')
                if b.Length = 3 then 
                    let tfy, y = Int16.TryParse(b.[0])
                    let tfm, m = Byte.TryParse(b.[1])
                    let tfd, d = Byte.TryParse(b.[2])
                    { x with Year = 
                                 if tfy then y |> Some
                                 else None
                             Month = 
                                 if tfm then m |> Some
                                 else None
                             Day = 
                                 if tfd then d |> Some
                                 else None }
                elif b.Length = 1 then 
                    let tfy, y = Int16.TryParse(b.[0])
                    { x with Year = 
                                 if tfy then y |> Some
                                 else None }
                else x
            | "Round" -> { x with Round = v }
            | "White" -> { x with White = v }
            | "Black" -> { x with Black = v }
            | "Result" -> 
                let res = 
                    match v with
                    | "1/2-1/2" -> Draw
                    | "1-0" -> WhiteWins
                    | "0-1" -> BlackWins
                    | _ -> NotKnown
                { x with Result = res }
            | _ -> x
        
        static member FormatTag name value = "[" + name + " \"" + value + "\"]" + Environment.NewLine
        static member Blank() = 
            { Id = -1
              Event = "?"
              Site = "?"
              Year = None
              Month = None
              Day = None
              Round = "?"
              White = "?"
              Black = "?"
              Result = NotKnown
              Moves = [] }
    
    let private NextGameRdr(sr : StreamReader) = 
        let pos = Pos.Start()
        
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
        
        let dohdr (gm : Game) (s : string) = 
            //Active pattern to parse header string
            let (|Header|_|) s = 
                let m = Regex("\[([\w]+)\s+\"([\s\S]*)\"\]").Match(s)
                if m.Success then Some(m.Groups.[1].Value, m.Groups.[2].Value)
                else None
            
            let eloc = s.IndexOf("]")
            if eloc = -1 then Invalid, gm, ""
            else 
                let tok = s.[..eloc - 1]
                let es = s.[eloc + 1..]
                
                let k, v = 
                    match ("[" + tok + "]") with
                    | Header(k, v) -> k, v
                    | _ -> failwith ("not a valid pgn header: " + "[" + tok + "]")
                if k = "FEN" then Invalid, gm, es
                //elif k = "Result" && v = "*" then Invalid, gm, es
                else 
                    let ngm = (k, v) |> gm.AddHdr
                    Unknown, ngm, es
        
        let domv s = 
            let tok, es = s |> getwd
            let mv = pos.GetMv tok
            pos.DoMv mv
            mv, es
        
        let rec proclin st s gm = 
            if s = "" then st, gm
            else 
                match st with
                | InComment(cl) -> 
                    let nst, ns = docomm cl s
                    proclin nst ns gm
                | InRAV(cl) -> 
                    let nst, ns = dorav cl s
                    proclin nst ns gm
                | InNAG -> 
                    let ns = s |> donag
                    proclin Unknown ns gm
                | InNum -> 
                    let nst, ns = s |> donum
                    proclin nst ns gm
                | Invalid -> 
                    let nst = s |> doinv
                    nst, gm
                | InHeader -> 
                    let nst, ngm, ns = dohdr gm s
                    proclin nst ns ngm
                | InMove -> 
                    let move, ns = domv s
                    proclin Unknown ns { gm with Moves = (move :: gm.Moves) }
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
                    proclin st ns gm
        
        let rec getgm st gm = 
            let lin = sr.ReadLine()
            if lin |> isNull then { gm with Moves = (gm.Moves |> List.rev) }
            else 
                let nst, ngm = proclin st lin gm
                if nst = FinishedOK then { ngm with Moves = (ngm.Moves |> List.rev) }
                elif nst = FinishedInvalid then Game.Blank()
                else getgm nst ngm
        
        let gm = getgm Unknown (Game.Blank())
        if gm.Moves.Length > 0 then gm |> Some
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
