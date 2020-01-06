namespace FsChessPgn.Test

open FsChess
open System.IO
open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveFormatterTest()=

    let _move1 = pMove.CreateOrig(MoveType.Capture,Sq(FileD, Rank5),None,Some(FileE),None)
    let _move2 = pMove.Create(MoveType.Simple,Sq(FileD, Rank4),PieceType.Knight|>Some)

    let TestGameString =
        @"[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]

{some moves} 1-0
"

    let _testGame = 
        {GameEMP with Event="Breslau";Site="Breslau";Year=Some(1879);WhitePlayer="Tarrasch, Siegbert";BlackPlayer="Mendelsohn, J.";Result=GameResult.WhiteWins;MoveText=[CommentEntry("some moves");GameEndEntry(GameResult.WhiteWins)]}

    [<TestMethod>]
    member this.PgnWrite_should_accept_TextWriter() =
        let writer = new StringWriter()
        writer.Write("Foo ")
        let move = pMove.Create(MoveType.Simple,Sq(FileC,Rank5),PieceType.Rook|>Some)
        PgnWrite.Move(move, writer)

        Assert.AreEqual("Foo Rc5", writer.ToString())

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_castling_moves() =
        let writer = new StringWriter()
        let mv1 = pMove.CreateCastle(MoveType.CastleKingSide)
        let mv2 = pMove.CreateCastle(MoveType.CastleQueenSide)

        Assert.AreEqual("O-O", PgnWrite.MoveStr(mv1))
        Assert.AreEqual("O-O-O", PgnWrite.MoveStr(mv2))

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_simple_target_only_move() =
        let move = pMove.Create(MoveType.Simple,Sq(FileC,Rank5),PieceType.Rook|>Some)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Rc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_simple_pawn_move() =
        let move = pMove.Create(MoveType.Simple,Sq(FileC,Rank5),None)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("c5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_simple_pawn_move_with_explict_pawn() =
        let move = pMove.Create(MoveType.Simple,Sq(FileC,Rank5),PieceType.Pawn|>Some)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("c5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_origin_to_target_move() =
        let move = pMove.CreateOrig(MoveType.Simple,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nb7c5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_origin_file_to_target_move() =
        let move = pMove.CreateOrig(MoveType.Simple,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),None)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nbc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_origin_rank_to_target_move() =
        let move = pMove.CreateOrig(MoveType.Simple,Sq(FileC,Rank5),PieceType.Knight|>Some,None,Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("N7c5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_capturing_move() =
        let move = pMove.Create(MoveType.Capture,Sq(FileC,Rank5),PieceType.Knight|>Some)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nxc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_pawn_capturing_move_with_origin_file_info() =
        let move = pMove.CreateOrig(MoveType.Capture,Sq(FileC,Rank6),None,Some(FileB),None)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("bxc6", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_capturing_move_with_origin_square() =
        let move = pMove.CreateOrig(MoveType.Capture,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nb7xc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_Nb7xc5() =
        let move = pMove.CreateOrig(MoveType.Capture,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nb7xc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_exd5() =
        let move = pMove.CreateOrig(MoveType.Capture,Sq(FileD,Rank5),PieceType.Pawn|>Some,Some(FileE),None)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("exd5", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_piece_promotion() =
        let move = pMove.CreateAll(MoveType.Simple,Sq(FileE,Rank8),PieceType.Pawn|>Some,None,None,PieceType.Queen|>Some,false,false,false)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("e8=Q", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_piece_promotion_after_capture() =
        let move = pMove.CreateAll(MoveType.Capture,Sq(FileE,Rank8),PieceType.Pawn|>Some,Some(FileD),Some(Rank7),PieceType.Queen|>Some,false,false,false)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("d7xe8=Q", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_check_annotation() =
        let move = pMove.CreateAll(MoveType.Capture,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),Some(Rank7),None,true,false,false)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nb7xc5+", str)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_any_annotation() =
        let move = pMove.CreateAll(MoveType.Capture,Sq(FileB,Rank8),PieceType.Rook|>Some,Some(FileB),Some(Rank1),None,false,false,true)
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Rb1xb8#", str)

    [<TestMethod>]
    member this.PgnWrite_should_ommit_redudant_piece_definition___N7c5_and_not_N7Nc5() =
        let move = pMove.CreateOrig(MoveType.Simple,Sq(FileC,Rank5),PieceType.Knight|>Some,None,Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("N7c5", str)

    [<TestMethod>]
    member this.PgnWrite_should_include_captured_piece_even_if_its_the_same() =
        let move = pMove.CreateOrig(MoveType.Capture,Sq(FileC,Rank5),PieceType.Knight|>Some,Some(FileB),Some(Rank7))
        let str = PgnWrite.MoveStr(move)

        Assert.AreEqual("Nb7xc5", str)

    [<TestMethod>]
    member this.PgnWrite_should_accept_TextWriter2() =
        let writer = new StringWriter()
        writer.Write("Foo ")
        let entry = HalfMoveEntry(None,false,_move1,None)
        PgnWrite.MoveTextEntry(entry, writer)
        writer.Write(" ")
        let entry = HalfMoveEntry(None,false,_move2,None)
        PgnWrite.MoveTextEntry(entry, writer)

        Assert.AreEqual("Foo exd5 Nd4", writer.ToString())

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_move_pair() =
        let entry1 = HalfMoveEntry(None,false,_move1,None)
        let entry2 = HalfMoveEntry(None,false,_move2,None)
        let act = PgnWrite.MoveTextEntryStr(entry1) + " " + PgnWrite.MoveTextEntryStr(entry2)

        Assert.AreEqual("exd5 Nd4", act)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_move_pair_with_number() =
        let entry1 = HalfMoveEntry(Some(6),false,_move1,None)
        let entry2 = HalfMoveEntry(None,false,_move2,None)
        let act = PgnWrite.MoveTextEntryStr(entry1) + " " + PgnWrite.MoveTextEntryStr(entry2)

        Assert.AreEqual("6. exd5 Nd4", act)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_starting_single_move() =
        let entry = HalfMoveEntry(Some(6),false,_move1,None)
        let act = PgnWrite.MoveTextEntryStr (entry)

        Assert.AreEqual("6. exd5", act)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_continued_single_move() =
        let entry = HalfMoveEntry(Some(6),true,_move2,None)
        let act = PgnWrite.MoveTextEntryStr (entry)

        Assert.AreEqual("6... Nd4", act)

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_GameEndEntry() =
        Assert.AreEqual("1-0", PgnWrite.MoveTextEntryStr(GameEndEntry(GameResult.WhiteWins)))
        Assert.AreEqual("0-1", PgnWrite.MoveTextEntryStr(GameEndEntry(GameResult.BlackWins)))
        Assert.AreEqual("*", PgnWrite.MoveTextEntryStr(GameEndEntry(GameResult.Open)))

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_CommentEntry() =
        Assert.AreEqual("{This is a test comment}", PgnWrite.MoveTextEntryStr(CommentEntry("This is a test comment")))

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_NAGEntry() =
        Assert.AreEqual("$5", PgnWrite.MoveTextEntryStr(NAGEntry(5)))

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_a_RAVEntry() =
        let entry = HalfMoveEntry(Some(6),true,_move2,None)
        let ravEntry = RAVEntry([entry])
        Assert.AreEqual("(6... Nd4)", PgnWrite.MoveTextEntryStr(ravEntry))

    [<TestMethod>]
    member this.PgnWrite_should_PgnWrite_move_text() =
        let entry1 = HalfMoveEntry(Some(37),false,pMove.CreateAll(MoveType.Capture,Sq(FileE, Rank5),PieceType.Knight|>Some,None,None,None,false,false,false),None)
        let entry2 = NAGEntry(13)
        let rav1 = CommentEntry("comment")
        let rav2 = HalfMoveEntry(Some(37),false,pMove.CreateAll(MoveType.Simple,Sq(FileE, Rank3),PieceType.Knight|>Some,None,None,None,false,false,false),None)
        let entry3 = RAVEntry([rav1;rav2])
        let entry4 = HalfMoveEntry(Some(37),true,pMove.Create(MoveType.Simple,Sq(FileD, Rank8),PieceType.Rook|>Some),None)
        let entry5a = HalfMoveEntry(Some(38),false,pMove.Create(MoveType.Simple,Sq(FileH, Rank4),PieceType.Pawn|>Some),None)
        let entry5b = HalfMoveEntry(None,false,pMove.Create(MoveType.Simple,Sq(FileD, Rank5),PieceType.Rook|>Some),None)
        let entry6 = GameEndEntry(GameResult.WhiteWins)
        let entry7 = CommentEntry("game ends in win, whooot")

        let ml = [entry1;entry2;entry3;entry4;entry5a;entry5b;entry6;entry7]
        Assert.AreEqual("37. Nxe5 $13 ({comment} 37. Ne3) 37... Rd8 38. h4 Rd5 1-0 {game ends in win, whooot}", PgnWrite.MoveTextStr(ml))

    [<TestMethod>]
    member this.ormat_should_deal_with_empty_move_text() =
        let ml = []
        Assert.AreEqual("", PgnWrite.MoveTextStr(ml))

    [<TestMethod>]
    member this.PgnWrite_should_accept_TextWriter3() =
        let writer = new StringWriter()
        PgnWrite.Game(_testGame, writer)
        let act = writer.ToString()
        Assert.AreEqual(TestGameString, act)

    [<TestMethod>]
    member this.PgnWrite_should_Game_correctly() =
        let act = PgnWrite.GameStr(_testGame)
        Assert.AreEqual(TestGameString, act)
