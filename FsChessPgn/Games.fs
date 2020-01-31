namespace FsChessPgn

open FsChess
open System.IO
open System.Text

module Games =

    let ReadFromStream(stream : Stream) = 
        let sr = new StreamReader(stream)
        let db = RegParse.AllGamesRdr(sr)
        db

    let ReadSeqFromFile(file : string) = 
        let stream = new FileStream(file, FileMode.Open)
        let db = ReadFromStream(stream)
        db

    let ReadFromFile(file : string) = 
        let stream = new FileStream(file, FileMode.Open)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadIndexListFromFile(file : string) = ReadFromFile(file)|>List.indexed

    let ReadFromString(str : string) = 
        let byteArray = Encoding.ASCII.GetBytes(str)
        let stream = new MemoryStream(byteArray)
        let result = ReadFromStream(stream) |> Seq.toList
        stream.Close()
        result

    let ReadOneFromString(str : string) = 
        let gms = str|>ReadFromString
        gms.Head

    let SetaMoves(gml:Game list) =
        gml|>List.map Game.SetaMoves

    
    let CreateIndex (fn:string) =
        let binfn = fn + ".indx"
        if not (File.Exists(binfn))||(File.Exists(binfn) && File.GetLastWriteTime(binfn)<File.GetLastWriteTime(fn)) then
            //TODO: consider doing in chunks for very large files
            let gml = fn|>ReadFromFile
            let prnks = [|A2; B2; C2; D2; E2; F2; G2; H2; A7; B7; C7; D7; E7; F7; G7; H7|]
            //keys list of empty squares,values is list of game indexes 
            let dct0 = new System.Collections.Generic.Dictionary<Set<Square>,int list>()
            let n_choose_k k = 
                let rec choose lo hi =
                    if hi = 0 then [[]]
                    else
                        [for j=lo to (Array.length prnks)-1 do
                             for ks in choose (j+1) (hi-1) do
                                    yield prnks.[j] :: ks ]
                choose 0 k                           
            let full = [1..16]|>List.map(n_choose_k)|>List.concat
            let dct =
                full|>List.iter(fun sql -> dct0.Add(sql|>Set.ofList,[]))
                dct0
            let rec addgm id sql cbd (imtel:MoveTextEntry list) =
                if not imtel.IsEmpty then
                    let mte = imtel.Head
                    match mte with
                    |HalfMoveEntry(_,_,pmv,_) -> 
                        let mv = pmv|>pMove.ToMove cbd
                        let nbd = cbd|>Board.MoveApply mv
                        //now check if a pawn move which is not on search board
                        //need to also include captures of pawns on starts square
                        let pc = mv|>Move.MovingPiece
                        let sq = mv|>Move.From
                        let rnk = sq|>Square.ToRank
                        let cpc = mv|>Move.CapturedPiece
                        let sqto = mv|>Move.To
                        let rnkto = sqto|>Square.ToRank
                        if pc=Piece.WPawn && rnk=Rank2 || pc=Piece.BPawn && rnk=Rank7 then
                            let nsql = sq::sql
                            let cvl = dct.[nsql|>Set.ofList]
                            let nvl = id::cvl
                            dct.[nsql|>Set.ofList] <- nvl
                            addgm id nsql nbd imtel.Tail
                        elif cpc=Piece.WPawn && rnkto=Rank2 || cpc=Piece.BPawn && rnkto=Rank7 then
                                let nsql = sqto::sql
                                let cvl = dct.[nsql|>Set.ofList]
                                let nvl = id::cvl
                                dct.[nsql|>Set.ofList] <- nvl
                                addgm id nsql nbd imtel.Tail
                        else
                            addgm id sql nbd imtel.Tail
                    |_ -> addgm id sql cbd imtel.Tail
        
            let dogm i gm =
                let bd = if gm.BoardSetup.IsNone then Board.Start else gm.BoardSetup.Value
                addgm i [] bd gm.MoveText
            gml|>List.iteri dogm
            //now serialize
            let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
            let stream = new FileStream(binfn, FileMode.Create, FileAccess.Write, FileShare.None)
            formatter.Serialize(stream, dct)
            stream.Close()

    let GetIndex (fn:string) =
        let binfn = fn + ".indx"
        if File.Exists(binfn) then
            let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
            let stream = new FileStream(binfn, FileMode.Open, FileAccess.Read, FileShare.Read)
            let dct:System.Collections.Generic.Dictionary<Set<Square>,int list> = formatter.Deserialize(stream):?>System.Collections.Generic.Dictionary<Set<Square>,int list>
            stream.Close()
            dct
        else failwith "index missing"
    
    let private getInitStats (bd:Brd) (igml:(int * Game) list) =
        let rec getfirst (mtel:MoveTextEntry list) =
            if mtel.IsEmpty then "None"
            else
                let mte = mtel.Head
                match mte with
                |HalfMoveEntry(_,_,pmv,_) ->
                    pmv|>PgnWrite.MoveStr
                |_ -> getfirst mtel.Tail
        igml
        |>List.map(fun (i,gm) -> i,gm,(gm.MoveText|>getfirst))
        |>List.filter(fun (_,gm,_) -> gm.BoardSetup.IsNone)
    
    let FastFindBoard (bd:Brd) (dct:System.Collections.Generic.Dictionary<Set<Square>,int list>) (igml:(int * Game) list) =
        if bd=Board.Start then
            igml|>getInitStats bd
        else
            let prnks = [|A2; B2; C2; D2; E2; F2; G2; H2; A7; B7; C7; D7; E7; F7; G7; H7|]
            let empties = prnks|>Array.filter(fun sq -> (sq|>Square.ToRank)=Rank2 && bd.[sq]<>Piece.WPawn || (sq|>Square.ToRank)=Rank7 && bd.[sq]<>Piece.BPawn)|>Set.ofArray
            let possibles = dct.[empties]|>Array.ofList
            let ngml = igml|>List.filter(fun (i,gm) -> possibles|>Array.contains i)
            let gmfnds = ngml|>List.choose (Game.GetBoard bd)
            gmfnds
    
    let FindBoard (bd:Brd) (fn:string) =
        //TODO: consider doing in chunks for very large files
        let gml = fn|>ReadFromFile
        if bd=Board.Start then
            gml|>List.indexed|>getInitStats bd
        else
            //get index of which games have what combination of pawns moved
            let binfn = fn + ".bin"
            if File.Exists(binfn) then
                let dct = GetIndex fn
                FastFindBoard bd dct (gml|>List.indexed)
            else 
                let gmfnds = gml|>List.indexed|>List.choose (Game.GetBoard bd)
                gmfnds

