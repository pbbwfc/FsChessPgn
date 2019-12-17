namespace FsChessPgn.Data

open System.Text
open System.Text.RegularExpressions

module MoveUtil = 
    let ParseFilter (board : Brd) attackto piece file rank = 
        let rec getfits posl fitl = 
            if List.isEmpty posl then fitl
            else 
                let pos:Square = posl.Head
                if piece <> Piece.EMPTY && piece <> board.PieceAt.[int(pos)] then getfits posl.Tail fitl
                elif rank <> RANK_EMPTY && rank <> (pos|>Square.ToRank) then getfits posl.Tail fitl
                elif file <> FILE_EMPTY && file <> (pos|>Square.ToFile) then getfits posl.Tail fitl
                else getfits posl.Tail (pos :: fitl)
        
        let attacksTo = board|>Board.AttacksTo2 attackto board.WhosTurn
        let fits = getfits (attacksTo|>Bitboard.ToPositions) []
        if fits.Length <> 1 then 
            let rec getfits (mvl : Move list) fitl = 
                if List.isEmpty mvl then fitl
                else 
                    let mv = mvl.Head
                    if mv|>Move.To <> attackto then getfits mvl.Tail fitl
                    elif board.PieceAt.[int(mv|>Move.From)] <> piece then getfits mvl.Tail fitl
                    elif file <> FILE_EMPTY && ((mv|>Move.From)|>Square.ToFile) <> file then getfits mvl.Tail fitl
                    elif rank <> RANK_EMPTY && (mv|>Move.From|>Square.ToRank) <> rank then getfits mvl.Tail fitl
                    else getfits mvl.Tail ((mv|>Move.From) :: fitl)
            
            let fits = getfits (MoveGenerate.MovesLegal(board) |> Seq.toList) []
            if fits.Length = 1 then fits.Head
            else failwith "invalid move input"
        else fits.Head
    
    let Parse (board : Brd) (movetext : string) :Move = 
        let promote = Piece.EMPTY
        let mFrom = OUTOFBOUNDS
        let mTo = OUTOFBOUNDS
        let regex = new Regex("")
        let movetext = movetext.Replace("+", "")
        let movetext = movetext.Replace("x", "")
        let movetext = movetext.Replace("#", "")
        let movetext = movetext.Replace("=", "")
        let me = board.WhosTurn
        
        let mypawn = 
            if board.WhosTurn = Player.White then Piece.WPawn
            else Piece.BPawn
        
        let myknight = 
            if board.WhosTurn = Player.White then Piece.WKnight
            else Piece.BKnight
        
        let mybishop = 
            if board.WhosTurn = Player.White then Piece.WBishop
            else Piece.BBishop
        
        let myrook = 
            if board.WhosTurn = Player.White then Piece.WRook
            else Piece.BRook
        
        let myqueen = 
            if board.WhosTurn = Player.White then Piece.WQueen
            else Piece.BQueen
        
        let myking = 
            if board.WhosTurn = Player.White then Piece.WKing
            else Piece.BKing
        
        let mynorth = 
            if board.WhosTurn = Player.White then Direction.DirN
            else Direction.DirS
        
        let mysouth = 
            if board.WhosTurn = Player.White then Direction.DirS
            else Direction.DirN
        
        let myrank4 = 
            if board.WhosTurn = Player.White then Rank4
            else Rank5
        
        if Regex.IsMatch(movetext, "^[abcdefgh][12345678][abcdefgh][12345678]$", RegexOptions.IgnoreCase) then 
            let mFrom = Square.Parse(movetext.Substring(0, 2))
            let mTo = Square.Parse(movetext.Substring(2, 2))
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[abcdefgh][12345678][abcdefgh][12345678][BNRQK]$", RegexOptions.IgnoreCase) then 
            let mFrom = Square.Parse(movetext.Substring(0, 2))
            let mTo = Square.Parse(movetext.Substring(2, 2))
            let promote = movetext.[4]|>Piece.ParseAsPiece(me)
            Move.CreateProm mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)] (promote|>Piece.ToPieceType)
        elif movetext = "0-0" || movetext = "O-O" || movetext = "o-o" then 
            if me = Player.White then 
                let mFrom = E1
                let mTo = G1
                Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
            else 
                let mFrom = E8
                let mTo = G8
                Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif movetext = "0-0-0" || movetext = "O-O-O" || movetext = "o-o-o" then 
            if me = Player.White then 
                let mFrom = E1
                let mTo = C1
                Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
            else 
                let mFrom = E8
                let mTo = C8
                Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext)
            let tmppos = mTo|>Square.PositionInDirection(mysouth)
            if board.PieceAt.[int(tmppos)] = mypawn then 
                let mFrom = tmppos
                Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
            elif board.PieceAt.[int(tmppos)] = Piece.EMPTY && (mTo|>Square.ToRank) = myrank4 then 
                let tmppos = tmppos|>Square.PositionInDirection(mysouth)
                if board.PieceAt.[int(tmppos)] = mypawn then 
                    let mFrom = tmppos
                    Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
                else failwith ("no pawn can move to " + movetext)
            else failwith ("no pawn can move to " + movetext)
        elif Regex.IsMatch(movetext, "^[abcdefgh][12345678][BNRQK]$") then 
            let mTo = Square.Parse(movetext.Substring(0, 2))
            let tmppos = mTo|>Square.PositionInDirection(mysouth)
            if board.PieceAt.[int(tmppos)] = mypawn then 
                let mFrom = tmppos
                let promote = movetext.[2]|>Piece.ParseAsPiece(me)
                Move.CreateProm mFrom mTo board.PieceAt.[int(mFrom)] 
                                        board.PieceAt.[int(mTo)] (promote|>Piece.ToPieceType)
            else failwith ("no pawn can promoted to " + movetext.Substring(0, 2))
        elif Regex.IsMatch(movetext, "^[abcdefgh][abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext.Substring(1, 2))
            let tmpfile = File.Parse(movetext.[0])
            let mFrom = ParseFilter board mTo mypawn tmpfile RANK_EMPTY
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[abcdefgh][abcdefgh][12345678][BNRQK]$") then 
            let mTo = Square.Parse(movetext.Substring(1, 2))
            let tmpfile = File.Parse(movetext.[0])
            let mFrom = ParseFilter board mTo mypawn tmpfile RANK_EMPTY
            let promote = movetext.[3]|>Piece.ParseAsPiece(me)
            Move.CreateProm mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)] (promote|>Piece.ToPieceType)
        elif Regex.IsMatch(movetext, "^[BNRQK][abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext.Substring(1, 2))
            let tmppiece = movetext.[0]|>Piece.ParseAsPiece(me)
            let mFrom = ParseFilter board mTo tmppiece FILE_EMPTY RANK_EMPTY
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[BNRQK][abcdefgh][abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext.Substring(2, 2))
            let tmppiece = movetext.[0]|>Piece.ParseAsPiece(me)
            let tmpfile = File.Parse(movetext.[1])
            let mFrom = ParseFilter board mTo tmppiece tmpfile RANK_EMPTY
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[BNRQK][12345678][abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext.Substring(2, 2))
            let tmppiece = movetext.[0]|>Piece.ParseAsPiece(me)
            let tmprank = Rank.Parse(movetext.[1])
            let mFrom = ParseFilter board mTo tmppiece FILE_EMPTY tmprank
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        elif Regex.IsMatch(movetext, "^[BNRQK][abcdefgh][12345678][abcdefgh][12345678]$") then 
            let mTo = Square.Parse(movetext.Substring(3, 2))
            let tmppiece = movetext.[0]|>Piece.ParseAsPiece(me)
            let tmpfile = File.Parse(movetext.[1])
            let tmprank = Rank.Parse(movetext.[2])
            let mFrom = ParseFilter board mTo tmppiece tmpfile tmprank
            Move.Create mFrom mTo board.PieceAt.[int(mFrom)] board.PieceAt.[int(mTo)]
        else failwith "invalid move format"
    
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
                    ((board|>Board.AttacksTo2 (move|>Move.To) (piece|>Piece.PieceToPlayer))|>Bitboard.ToPositions)
            sb.Append(sPiece) |> ignore
            if pu then ()
            elif fu then sb.Append(sFile) |> ignore
            elif ru then sb.Append(sRank) |> ignore
            else sb.Append(sFile + sRank) |> ignore
            if iscap then sb.Append("x") |> ignore
            sb.Append(sTo) |> ignore
        let board = board|>Board.MoveApply(move)
        if board|>Board.IsChk then 
            if MoveGenerate.MovesLegal(board) |> Seq.isEmpty then sb.Append("#") |> ignore
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
        let mvs = MoveGenerate.MovesLegal bd
        let fmvs = mvs|>List.filter(fun m -> m|>Desc=uci)
        if fmvs.Length=1 then Some(fmvs.Head) else None
