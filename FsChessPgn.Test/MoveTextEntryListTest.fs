namespace FsChessPgn.Test

open FsChessPgn
open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveTextEntryListTest()=

    let TestGameString =
        @"[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]

1.e4 {...} e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"
    let mv = pMove.Create(MoveType.Simple,OUTOFBOUNDS,None)

    [<TestMethod>]
    member this.FullMoveCount_should_return_move_count_without_comments() =
        let mtel =
            [CommentEntry("foo");
             NAGEntry(1);
             RAVEntry([HalfMoveEntry(None,false,mv,None); HalfMoveEntry(None,false,mv,None)]);
             HalfMoveEntry(None,false,mv,None);
             HalfMoveEntry(None,false,mv,None);
             HalfMoveEntry(None,false,mv,None);
             HalfMoveEntry(None,false,mv,None);
             GameEndEntry(GameResult.Draw)]

        Assert.AreEqual(2, mtel|>Game.FullMoveCount)
        Assert.AreEqual(4, mtel|>Game.MoveCount)

    [<TestMethod>]
    member this.FullMoveCount_should_not_count_single_halfmoves() =
        let mtel =
            [HalfMoveEntry(None,false,mv,None)]

        Assert.AreEqual(0, mtel|>Game.FullMoveCount)
        Assert.AreEqual(1, mtel|>Game.MoveCount)

    [<TestMethod>]
    member this.FullMoveCount_should_count_pairwise_halfmoves() =
        let mtel =
            [HalfMoveEntry(None,false,mv,None);HalfMoveEntry(None,false,mv,None)]

        Assert.AreEqual(1, mtel|>Game.FullMoveCount)
        Assert.AreEqual(2, mtel|>Game.MoveCount)

    [<TestMethod>]
    member this.Moves_should_return_an_enumeration_of_moves() =
        let gml= Games.ReadFromString TestGameString
        Assert.AreEqual(1, gml.Length)
        let game = gml.Head

        let mtel = game.MoveText
        let mvs = mtel|>Game.GetMoves

        Assert.AreEqual(23, mtel|>Game.FullMoveCount)
        Assert.AreEqual(47, mtel|>Game.MoveCount)
        Assert.AreEqual(47, mvs.Length)
