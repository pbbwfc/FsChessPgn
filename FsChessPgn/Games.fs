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

    
    let CreateIndex (gml:Game list) =
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
                    let pc = mv|>Move.MovingPiece
                    if pc=Piece.WPawn then
                        let sq = mv|>Move.From
                        let rnk = sq|>Square.ToRank
                        if rnk=Rank2 then 
                            let nsql = sq::sql
                            let cvl = dct.[nsql|>Set.ofList]
                            let nvl = id::cvl
                            dct.[nsql|>Set.ofList] <- nvl
                            addgm id nsql nbd imtel.Tail
                        else
                            addgm id sql nbd imtel.Tail
                    elif pc=Piece.BPawn then
                        let sq = mv|>Move.From
                        let rnk = sq|>Square.ToRank
                        if rnk=Rank7 then 
                            let nsql = sq::sql
                            let cvl = dct.[nsql|>Set.ofList]
                            let nvl = id::cvl
                            dct.[nsql|>Set.ofList] <- nvl
                            addgm id nsql nbd imtel.Tail
                        else
                            addgm id sql nbd imtel.Tail
                    else
                        addgm id sql nbd imtel.Tail
                |_ -> addgm id sql cbd imtel.Tail
        
        let dogm i gm =
            let bd = if gm.BoardSetup.IsNone then Board.Start else gm.BoardSetup.Value
            addgm i [] bd gm.MoveText
        gml|>List.iteri dogm
        //dct
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        let stream = new FileStream(@"D:\MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None)
        formatter.Serialize(stream, dct)
        stream.Close()
        dct
    
    let FindBoard (bd:Brd) (gml:Game list) =
        //get index of which games have what combination of pawns moved
        let formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        let stream = new FileStream(@"D:\MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read)
        let dct:System.Collections.Generic.Dictionary<Set<Square>,int list> = formatter.Deserialize(stream):?>System.Collections.Generic.Dictionary<Set<Square>,int list>
        stream.Close()
        
        let gmfnds = gml|>List.map (Game.GetBoard bd)
    
        gmfnds|>List.filter fst|>List.map snd