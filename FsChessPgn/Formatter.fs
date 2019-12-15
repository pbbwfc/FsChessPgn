namespace FsChessPgn.Data

open System.IO
open System.Globalization

module Formatter =
    let (-?) (lhs:string option) rhs = (if lhs.IsNone then rhs else lhs.Value)
    let (|?) (lhs:int option) rhs = (if lhs.IsNone then rhs else lhs.Value.ToString(CultureInfo.InvariantCulture))

    let GetResultString(result:GameResult) =
        match result with
        |GameResult.WhiteWins -> "1-0" 
        |GameResult.BlackWins -> "0-1" 
        |GameResult.Draw -> "½-½" 
        |_ -> "*" 

    let GetPiece(pieceType: PieceType option) =
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
            
    let GetMoveTarget(move:pMove) =
        let piece = 
            match move.Mtype with
            |Simple -> "" 
            |_ -> GetPiece(move.TargetPiece)
        if move.TargetSquare <> OUTOFBOUNDS then
            piece + SQUARE_NAMES.[move.TargetSquare]
        elif move.TargetFile.IsSome then
            piece + FILE_NAMES.[move.TargetFile.Value]
        else ""

    let GetMoveOrigin(move:pMove) =
        let piece = GetPiece(move.Piece)
        if move.OriginSquare <> OUTOFBOUNDS then
            piece + SQUARE_NAMES.[move.OriginSquare]
        else 
            let origf = if move.OriginFile.IsSome then FILE_NAMES.[move.OriginFile.Value] else ""
            let origr = if move.OriginRank.IsSome then RANK_NAMES.[move.OriginRank.Value] else ""
            piece + origf + origr    
    
    let GetCheckAndMateAnnotation(move:pMove) =
        if move.IsCheckMate then "#"
        elif move.IsDoubleCheck then "++"
        elif move.IsCheck then "+"
        else ""

    let GetAnnotation(move:pMove) =
        if move.Annotation.IsNone then ""
        else
            match move.Annotation.Value with
            | MindBlowing -> "!!!"
            | Brilliant -> "!!"
            | Good -> "!"
            | Interesting -> "!?"
            | Dubious -> "?!"
            | Mistake -> "?"
            | Blunder -> "??"
            | Abysmal -> "???"
            | FascinatingButUnsound -> "!!?"
            | Unclear -> "∞"
            | WithCompensation -> "=/∞"
            | EvenPosition -> "="
            | SlightAdvantageWhite -> "+/="
            | SlightAdvantageBlack -> "=/+"
            | AdvantageWhite -> "+/−"
            | AdvantageBlack -> "−/+"
            | DecisiveAdvantageWhite -> "+−"
            | DecisiveAdvantageBlack -> "-+"
            | Space -> "○"
            | Initiative -> "↑"
            | Development -> "↑↑"
            | Counterplay -> "⇄"
            | Countering -> "∇"
            | Idea -> "Δ"
            | TheoreticalNovelty -> "N"
            | UnknownAnnotation -> ""

    let FormatMove(mv:pMove, writer:TextWriter) =
        match mv.Mtype with
        | Simple -> 
            let origin = GetMoveOrigin(mv)
            let target = GetMoveTarget(mv)
            writer.Write(origin)
            writer.Write(target)
            if mv.PromotedPiece.IsSome then
                writer.Write("=")
                writer.Write(GetPiece(mv.PromotedPiece))
            writer.Write(GetCheckAndMateAnnotation(mv))
            writer.Write(GetAnnotation(mv))
        | Capture -> 
            let origin = GetMoveOrigin(mv)
            let target = GetMoveTarget(mv)
            writer.Write(origin)
            writer.Write("x")
            writer.Write(target)
            if mv.PromotedPiece.IsSome then
                writer.Write("=")
                writer.Write(GetPiece(mv.PromotedPiece))
            writer.Write(GetCheckAndMateAnnotation(mv))
            writer.Write(GetAnnotation(mv))
        | CaptureEnPassant ->
            let origin = GetMoveOrigin(mv)
            let target = GetMoveTarget(mv)
            writer.Write(origin)
            writer.Write("x")
            writer.Write(target)
            writer.Write("e.p.")
            writer.Write(GetCheckAndMateAnnotation(mv))
            writer.Write(GetAnnotation(mv))
        | CastleKingSide -> 
            writer.Write("O-O")
            writer.Write(GetCheckAndMateAnnotation(mv))
            writer.Write(GetAnnotation(mv))
        | CastleQueenSide ->
            writer.Write("O-O-O")
            writer.Write(GetCheckAndMateAnnotation(mv))
            writer.Write(GetAnnotation(mv))

    let FormatMoveStr(mv:pMove) =
        let writer = new StringWriter()
        FormatMove(mv,writer)
        writer.ToString()

    let rec FormatMoveTextEntry(entry:MoveTextEntry, writer:TextWriter) =
        match entry with
        |HalfMoveEntry(mn,ic,mv) -> 
            if mn.IsSome then
                writer.Write(mn.Value)
                writer.Write(if ic then "... " else ". ")
            FormatMove(mv, writer)
        |CommentEntry(str) -> writer.Write("{" + str + "}")
        |GameEndEntry(gr) -> writer.Write(GetResultString(gr))
        |NAGEntry(cd) -> writer.Write("$" + cd.ToString())
        |RAVEntry(ml) -> 
            writer.Write("(")
            FormatMoveText(ml, writer)
            writer.Write(")")
    
    and FormatMoveText(ml:MoveTextEntry list, writer:TextWriter) =
        let doent i m =
            FormatMoveTextEntry(m,writer)
            if i<ml.Length-1 then writer.Write(" ")

        ml|>List.iteri doent
    
    let FormatMoveTextEntryStr(entry:MoveTextEntry) =
        let writer = new StringWriter()
        FormatMoveTextEntry(entry,writer)
        writer.ToString()

    let FormatMoveTextStr(ml:MoveTextEntry list) =
        let writer = new StringWriter()
        FormatMoveText(ml,writer)
        writer.ToString()

    let FormatTag(name:string, value:string, writer:TextWriter) =
        writer.Write("[")
        writer.Write(name + " \"")
        writer.Write(value)
        writer.WriteLine("\"]")

    let FormatDate(game:Game, writer:TextWriter) =
        writer.Write("[Date \"")
        writer.Write(game.Year |? "????")
        writer.Write(".")
        writer.Write(game.Month |? "??")
        writer.Write(".")
        writer.Write(game.Day |? "??")
        writer.WriteLine("\"]")

    let Format(game:Game, writer:TextWriter) =
        FormatTag("Event", game.Event, writer)
        FormatTag("Site", game.Site, writer)
        FormatDate(game, writer)
        FormatTag("Round", game.Round, writer)
        FormatTag("White", game.WhitePlayer, writer)
        FormatTag("Black", game.BlackPlayer, writer)
        FormatTag("Result", GetResultString(game.Result), writer)

        for info in game.AdditionalInfo do
            FormatTag(info.Name, info.Value, writer)

        writer.WriteLine();
        FormatMoveText(game.MoveText, writer)

    let FormatStr(game:Game) =
        let writer = new StringWriter()
        Format(game,writer)
        writer.ToString()


