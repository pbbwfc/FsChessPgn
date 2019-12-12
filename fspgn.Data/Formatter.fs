namespace fspgn.Data

open System.IO
open System.Globalization
open PgnTextTypes

module Formatter =
    let (-?) (lhs:string option) rhs = (if lhs.IsNone then rhs else lhs.Value)
    let (|?) (lhs:int option) rhs = (if lhs.IsNone then rhs else lhs.Value.ToString(CultureInfo.InvariantCulture))

    let GetResultString(result:GameResult) =
        match result with
        |GameResult.WhiteWins -> "1-0" 
        |GameResult.BlackWins -> "0-1" 
        |GameResult.Draw -> "½-½" 
        |_ -> "*" 

    let FormatMove(mv:pMove, writer:TextWriter) =
        match mv.Mtype with
        | Simple -> ()
        | Capture -> ()
        | CaptureEnPassant -> ()
        | CastleKingSide -> ()
        | CastleQueenSide -> ()

    let rec FormatMoveTextEntry(entry:MoveTextEntry, writer:TextWriter) =
        match entry with
        |MovePairEntry(mn,mv1,mv2) ->
            if mn.IsSome then
                writer.Write(mn);
                writer.Write(". ");
            FormatMove(mv1, writer)
            writer.Write(" ");
            FormatMove(mv2, writer)
        |HalfMoveEntry(mn,ic,mv) -> 
            if mn.IsSome then
                writer.Write(mn)
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
    
    let FormatTag(name:string, value:string, writer:TextWriter) =
        writer.Write("[")
        writer.Write(name + " \"")
        writer.Write(value)
        writer.WriteLine("\"]")

    let FormatDate(game:pGame, writer:TextWriter) =
        writer.Write("[Date \"")
        writer.Write(game.Year |? "????")
        writer.Write(".")
        writer.Write(game.Month |? "??")
        writer.Write(".")
        writer.Write(game.Day |? "??")
        writer.WriteLine("\"]")

    let Format(game:pGame, writer:TextWriter) =
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



