namespace FsChessPgn.Data

open System.Text
open System.Text.RegularExpressions

module MoveUtil = 
    
    let Desc(move : Move) = 
        (move|>Move.From|>Square.Name).ToLower() + (move|>Move.To|>Square.Name).ToLower() 
        + (if move|>Move.Promote <> Piece.EMPTY then (move|>Move.Promote|>Piece.PieceToString).ToLower() else "")
    
    let DescBd (move : Move) (board : Brd) = 
        let sb = new StringBuilder()
        let piece = board.PieceAt.[int(move|>Move.From)]
        let fromrank = move|>Move.From|>Square.ToRank
        let fromfile = move|>Move.From|>Square.ToFile
        let isprom = move|>Move.Promote <> Piece.EMPTY
        let sTo = move|>Move.To|>Square.Name
        let sPiece = (piece|>Piece.PieceToString).ToUpper()
        let sRank = (fromrank|>Rank.RankToString).ToLower()
        let sFile = (fromfile|>File.FileToString).ToLower()
        
        let iscap = 
            if (move|>Move.To = board.EnPassant && (piece = Piece.WPawn || piece = Piece.BPawn)) then true
            else board.PieceAt.[int(move|>Move.To)] <> Piece.EMPTY
        
        let sProm = 
            if isprom then ((move|>Move.Promote)|>Piece.PieceToString).ToUpper()
            else ""
        
        if piece = Piece.WPawn || piece = Piece.BPawn then 
            if iscap then sb.Append(sFile + "x") |> ignore
            sb.Append(sTo) |> ignore
            if isprom then sb.Append(sProm) |> ignore
        elif piece = Piece.WKing && (move|>Move.From) = E1 && (move|>Move.To) = G1 then 
            sb.Append("O-O") |> ignore
        elif piece = Piece.BKing && (move|>Move.From) = E8 && (move|>Move.To) = G8 then 
            sb.Append("O-O") |> ignore
        elif piece = Piece.WKing && (move|>Move.From) = E1 && (move|>Move.To) = C1 then 
            sb.Append("O-O-O") |> ignore
        elif piece = Piece.BKing && (move|>Move.From) = E8 && (move|>Move.To) = C8 then 
            sb.Append("O-O-O") |> ignore
        else 
            let rec getuniqs pu fu ru attl = 
                if List.isEmpty attl then pu, fu, ru
                else 
                    let att = attl.Head
                    if att = (move|>Move.From) then getuniqs pu fu ru attl.Tail
                    else 
                        let otherpiece = board.PieceAt.[int(att)]
                        if otherpiece = piece then 
                            let npu = false
                            
                            let nru = 
                                if (att|>Square.ToRank) = fromrank then false
                                else ru
                            
                            let nfu = 
                                if (att|>Square.ToFile) = fromfile then false
                                else fu
                            
                            getuniqs npu nfu nru attl.Tail
                        else getuniqs pu fu ru attl.Tail
            
            let pu, fu, ru = 
                getuniqs true true true 
                    ((board|>Board.AttacksTo (move|>Move.To) (piece|>Piece.PieceToPlayer))|>Bitboard.ToPositions)
            sb.Append(sPiece) |> ignore
            if pu then ()
            elif fu then sb.Append(sFile) |> ignore
            elif ru then sb.Append(sRank) |> ignore
            else sb.Append(sFile + sRank) |> ignore
            if iscap then sb.Append("x") |> ignore
            sb.Append(sTo) |> ignore
        let board = board|>Board.MoveApply(move)
        if board|>Board.IsChk then 
            if MoveGenerate.AllMoves(board) |> Seq.isEmpty then sb.Append("#") |> ignore
            else sb.Append("+") |> ignore
        sb.ToString()
    
    let Descs moves (board : Brd) isVariation = 
        let sb = new StringBuilder()
        let rec getsb mvl ibd =
            if List.isEmpty mvl then ibd
            else
                let mv = mvl.Head
                if isVariation && ibd.WhosTurn = Player.White then sb.Append(ibd.Fullmove.ToString() + ". ") |> ignore
                sb.Append((DescBd mv ibd) + " ") |> ignore
                if isVariation then getsb mvl.Tail (ibd|>Board.MoveApply mv)
                else getsb mvl.Tail ibd
        board|>getsb moves|>ignore
        sb.ToString()

    let FindMv uci bd =
        let mvs = MoveGenerate.AllMoves bd
        let fmvs = mvs|>List.filter(fun m -> m|>Desc=uci)
        if fmvs.Length=1 then Some(fmvs.Head) else None

    ///Get a n encoded move from a SAN Move(move) such as Nf3 for this Board(bd)
    let fromSAN (bd : Brd) (move : string) = 
        move|>pMove.Parse|>pMove.ToMove bd

    ///Make a SAN Move(move) such as Nf3 for this Board(bd) and return the new Board
    let ApplySAN (move : string) (bd : Brd) = 
        let mv = move|>fromSAN bd
        bd|>Board.MoveApply mv