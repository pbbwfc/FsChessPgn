namespace FsChessPgn

open FsChess
open System.IO

module PgnWrite =

    let ResultString = GameResult.ToStr

    let Piece(pieceType: PieceType option) =
        if pieceType.IsNone then ""
        else 
            match pieceType.Value with
            |PieceType.Pawn -> ""
            |PieceType.Knight -> "N"
            |PieceType.Bishop -> "B"
            |PieceType.Rook -> "R"
            |PieceType.Queen -> "Q"
            |PieceType.King -> "K"
            |_ -> ""
            
    let MoveTarget(move:pMove) =
        if move.TargetSquare <> OUTOFBOUNDS then
            SQUARE_NAMES.[int(move.TargetSquare)]
        else ""

    let MoveOrigin(move:pMove) =
        let piece = Piece(move.Piece)
        let origf = if move.OriginFile.IsSome then FILE_NAMES.[int(move.OriginFile.Value)] else ""
        let origr = if move.OriginRank.IsSome then RANK_NAMES.[int(move.OriginRank.Value)] else ""
        piece + origf + origr    
    
    let CheckAndMateAnnotation(move:pMove) =
        if move.IsCheckMate then "#"
        elif move.IsDoubleCheck then "++"
        elif move.IsCheck then "+"
        else ""

    let Move(mv:pMove, writer:TextWriter) =
        match mv.Mtype with
        | Simple -> 
            let origin = MoveOrigin(mv)
            let target = MoveTarget(mv)
            writer.Write(origin)
            writer.Write(target)
            if mv.PromotedPiece.IsSome then
                writer.Write("=")
                writer.Write(Piece(mv.PromotedPiece))
            writer.Write(CheckAndMateAnnotation(mv))
        | Capture -> 
            let origin = MoveOrigin(mv)
            let target = MoveTarget(mv)
            writer.Write(origin)
            writer.Write("x")
            writer.Write(target)
            if mv.PromotedPiece.IsSome then
                writer.Write("=")
                writer.Write(Piece(mv.PromotedPiece))
            writer.Write(CheckAndMateAnnotation(mv))
        | CastleKingSide -> 
            writer.Write("O-O")
            writer.Write(CheckAndMateAnnotation(mv))
        | CastleQueenSide ->
            writer.Write("O-O-O")
            writer.Write(CheckAndMateAnnotation(mv))

    let MoveStr(mv:pMove) =
        let writer = new StringWriter()
        Move(mv,writer)
        writer.ToString()

    let rec MoveTextEntry(entry:MoveTextEntry, writer:TextWriter) =
        match entry with
        |HalfMoveEntry(mn,ic,mv,amv) -> 
            if mn.IsSome then
                writer.Write(mn.Value)
                writer.Write(if ic then "... " else ". ")
            Move(mv, writer)
            writer.Write(" ")
        |CommentEntry(str) -> 
            writer.WriteLine()
            writer.Write("{" + str + "} ")
        |GameEndEntry(gr) -> writer.Write(ResultString(gr))
        |NAGEntry(cd) -> 
            writer.Write("$" + (cd|>int).ToString())
            writer.Write(" ")
        |RAVEntry(ml) -> 
            writer.WriteLine()
            writer.Write("(")
            MoveText(ml, writer)
            writer.WriteLine(")")
    
    and MoveText(ml:MoveTextEntry list, writer:TextWriter) =
        let doent i m =
            MoveTextEntry(m,writer)
            //if i<ml.Length-1 then writer.Write(" ")

        ml|>List.iteri doent
    
    let MoveTextEntryStr(entry:MoveTextEntry) =
        let writer = new StringWriter()
        MoveTextEntry(entry,writer)
        writer.ToString()

    let MoveTextStr(ml:MoveTextEntry list) =
        let writer = new StringWriter()
        MoveText(ml,writer)
        writer.ToString()

    let Tag(name:string, value:string, writer:TextWriter) =
        writer.Write("[")
        writer.Write(name + " \"")
        writer.Write(value)
        writer.WriteLine("\"]")

    let Game(game:Game, writer:TextWriter) =
        Tag("Event", game.Event, writer)
        Tag("Site", game.Site, writer)
        Tag("Date", game|>DateUtil.ToStr, writer)
        Tag("Round", game.Round, writer)
        Tag("White", game.WhitePlayer, writer)
        Tag("Black", game.BlackPlayer, writer)
        Tag("Result", ResultString(game.Result), writer)
        Tag("WhiteElo", game.WhiteElo, writer)
        Tag("BlackElo", game.BlackElo, writer)

        for info in game.AdditionalInfo do
            Tag(info.Key, info.Value, writer)

        writer.WriteLine();
        MoveText(game.MoveText, writer)
        writer.WriteLine();

    let GameStr(game:Game) =
        let writer = new StringWriter()
        Game(game,writer)
        writer.ToString()


