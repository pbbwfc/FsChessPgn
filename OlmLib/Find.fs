namespace Olm

module Find =
    let GetBoard (bd:Brd) (cp:ChessPack) (hdr:Hdr) =
        let rec getbd cbd (imvl:Move list) =
            if imvl.IsEmpty then false,MoveEmpty
            else
                let mv = imvl.Head
                //now check if a pawn move which is not on search board
                let pc = mv|>Move.MovingPiece
                if pc=Piece.WPawn then
                    let sq = mv|>Move.From
                    let rnk = sq|>Square.ToRank
                    if rnk=Rank2 && bd.[sq]=Piece.WPawn then false,MoveEmpty
                    else
                        let nbd = cbd|>Board.MoveApply mv
                        if nbd.PieceAt=bd.PieceAt then
                            if imvl.Tail.IsEmpty then false,MoveEmpty
                            else
                                let nmv = imvl.Tail.Head
                                true,nmv
                        else getbd nbd imvl.Tail
                elif pc=Piece.BPawn then
                    let sq = mv|>Move.From
                    let rnk = sq|>Square.ToRank
                    if rnk=Rank7 && bd.[sq]=Piece.BPawn then false,MoveEmpty
                    else
                        let nbd = cbd|>Board.MoveApply mv
                        if nbd.PieceAt=bd.PieceAt then
                            if imvl.Tail.IsEmpty then false,MoveEmpty
                            else
                                let nmv = imvl.Tail.Head
                                true,nmv
                        else getbd nbd imvl.Tail
                else
                    let nbd = cbd|>Board.MoveApply mv
                    if nbd.PieceAt=bd.PieceAt then
                        if imvl.Tail.IsEmpty then false,MoveEmpty
                        else
                            let nmv = imvl.Tail.Head
                            true,nmv
                    else getbd nbd imvl.Tail
    
        let initbd = Board.Start
        let mvs = cp.Mvss.[hdr.Num]|>List.ofArray
        let fnd,mv = getbd initbd mvs
        if fnd then Some(hdr,mv) else None

    let Board (bd:Brd) (cp:ChessPack) =
        let prnks = [|A2; B2; C2; D2; E2; F2; G2; H2; A7; B7; C7; D7; E7; F7; G7; H7|]
        let empties = prnks|>Array.filter(fun sq -> (sq|>Square.ToRank)=Rank2 && bd.[sq]<>Piece.WPawn || (sq|>Square.ToRank)=Rank7 && bd.[sq]<>Piece.BPawn)|>Set.ofArray
        let possibles = cp.Indx.[empties|>Set.map(Square.Name)|>Set.toArray|>Array.reduce(+)]
        let nhdrs = cp.Hdrs|>Array.filter (fun hdr -> possibles|>Array.contains hdr.Num)
        let gmfnds = nhdrs|>Array.choose (GetBoard bd cp)
        gmfnds


