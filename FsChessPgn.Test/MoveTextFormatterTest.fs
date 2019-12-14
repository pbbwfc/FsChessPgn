namespace FsChessPgn.Test

open System.IO
open FsChessPgn.Data
open FsChessPgn.Data.PgnTextTypes

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveTextFormatterTest()=

    let _move1 = pMoveCreateOrig(pMoveType.Capture,None,Sq(FileD, Rank5),None,None,OUTOFBOUNDS,Some(FileE),None)
    let _move2 = pMoveCreate(pMoveType.Simple,None,Sq(FileD, Rank4),None,PieceType.Knight|>Some)

    [<TestMethod>]
    member this.Format_should_accept_TextWriter() =
        let writer = new StringWriter()
        writer.Write("Foo ")
        let entry = HalfMoveEntry(None,false,_move1)
        Formatter.FormatMoveTextEntry(entry, writer)
        writer.Write(" ")
        let entry = HalfMoveEntry(None,false,_move2)
        Formatter.FormatMoveTextEntry(entry, writer)

        Assert.AreEqual("Foo exd5 Nd4", writer.ToString())

    [<TestMethod>]
    member this.Format_should_format_move_pair() =
        let entry1 = HalfMoveEntry(None,false,_move1)
        let entry2 = HalfMoveEntry(None,false,_move2)
        let act = Formatter.FormatMoveTextEntryStr(entry1) + " " + Formatter.FormatMoveTextEntryStr(entry2)

        Assert.AreEqual("exd5 Nd4", act)

    [<TestMethod>]
    member this.Format_should_format_move_pair_with_number() =
        let entry1 = HalfMoveEntry(Some(6),false,_move1)
        let entry2 = HalfMoveEntry(None,false,_move2)
        let act = Formatter.FormatMoveTextEntryStr(entry1) + " " + Formatter.FormatMoveTextEntryStr(entry2)

        Assert.AreEqual("6. exd5 Nd4", act)

    [<TestMethod>]
    member this.Format_should_format_starting_single_move() =
        let entry = HalfMoveEntry(Some(6),false,_move1)
        let act = Formatter.FormatMoveTextEntryStr (entry)

        Assert.AreEqual("6. exd5", act)

    [<TestMethod>]
    member this.Format_should_format_continued_single_move() =
        let entry = HalfMoveEntry(Some(6),true,_move2)
        let act = Formatter.FormatMoveTextEntryStr (entry)

        Assert.AreEqual("6... Nd4", act)

    [<TestMethod>]
    member this.Format_should_format_a_GameEndEntry() =
        Assert.AreEqual("1-0", Formatter.FormatMoveTextEntryStr(GameEndEntry(GameResult.WhiteWins)))
        Assert.AreEqual("0-1", Formatter.FormatMoveTextEntryStr(GameEndEntry(GameResult.BlackWins)))
        Assert.AreEqual("*", Formatter.FormatMoveTextEntryStr(GameEndEntry(GameResult.Open)))

    [<TestMethod>]
    member this.Format_should_format_a_CommentEntry() =
        Assert.AreEqual("{This is a test comment}", Formatter.FormatMoveTextEntryStr(CommentEntry("This is a test comment")))

    [<TestMethod>]
    member this.Format_should_format_a_NAGEntry() =
        Assert.AreEqual("$5", Formatter.FormatMoveTextEntryStr(NAGEntry(5)))

    [<TestMethod>]
    member this.Format_should_format_a_RAVEntry() =
        let entry = HalfMoveEntry(Some(6),true,_move2)
        let ravEntry = RAVEntry([entry])
        Assert.AreEqual("(6... Nd4)", Formatter.FormatMoveTextEntryStr(ravEntry))

    [<TestMethod>]
    member this.Format_should_format_move_text() =
        let entry1 = HalfMoveEntry(Some(37),false,pMoveCreateAll(pMoveType.Capture,None,Sq(FileE, Rank5),None,PieceType.Knight|>Some,OUTOFBOUNDS,None,None,None,false,false,false,pMoveAnnotation.Good|>Some))
        let entry2 = NAGEntry(13)
        let rav1 = CommentEntry("comment")
        let rav2 = HalfMoveEntry(Some(37),false,pMoveCreateAll(pMoveType.Simple,None,Sq(FileE, Rank3),None,PieceType.Knight|>Some,OUTOFBOUNDS,None,None,None,false,false,false,pMoveAnnotation.Blunder|>Some))
        let entry3 = RAVEntry([rav1;rav2])
        let entry4 = HalfMoveEntry(Some(37),true,pMoveCreate(pMoveType.Simple,None,Sq(FileD, Rank8),None,PieceType.Rook|>Some))
        let entry5a = HalfMoveEntry(Some(38),false,pMoveCreate(pMoveType.Simple,None,Sq(FileH, Rank4),None,PieceType.Pawn|>Some))
        let entry5b = HalfMoveEntry(None,false,pMoveCreate(pMoveType.Simple,None,Sq(FileD, Rank5),None,PieceType.Rook|>Some))
        let entry6 = GameEndEntry(GameResult.WhiteWins)
        let entry7 = CommentEntry("game ends in win, whooot")

        let ml = [entry1;entry2;entry3;entry4;entry5a;entry5b;entry6;entry7]
        Assert.AreEqual("37. Nxe5! $13 ({comment} 37. Ne3??) 37... Rd8 38. h4 Rd5 1-0 {game ends in win, whooot}", Formatter.FormatMoveTextStr(ml))

    [<TestMethod>]
    member this.ormat_should_deal_with_empty_move_text() =
        let ml = []
        Assert.AreEqual("", Formatter.FormatMoveTextStr(ml))
