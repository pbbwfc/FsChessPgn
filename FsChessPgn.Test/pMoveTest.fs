#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data
open FsChessPgn.NET

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type pMoveTest()=

    let NormalGame = @"
[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]
[WhiteElo """"]
[BlackElo """"]
[ECO ""C49""]

1.e4 e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"

    [<TestMethod>]
    member this.pMove_simple_pawn_move() =
        let bd = Board.Start
        let gm = NormalGame|>PgnReader.ReadFromString|>List.head
        let mvte1 = gm.MoveText.Head
        let (HalfMoveEntry (mn,ic,mv,_)) = mvte1
        Assert.AreEqual(1,mn.Value)
        Assert.AreEqual(false,ic)
        let amv=mv|>pMove.ToaMove bd
        Assert.AreEqual(E2,amv.Mv|>Move.From)
        Assert.AreEqual(E4,amv.Mv|>Move.To)
