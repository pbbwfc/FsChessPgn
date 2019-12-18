#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type GameTest() =
    let testGame1 = "
[Event \"Breslau\"]
[Site \"Breslau\"]
[Date \"1879.??.??\"]
[Round \"?\"]
[White \"Tarrasch, Siegbert\"]
[Black \"Mendelsohn, J.\"]
[Result \"1-0\"]
[WhiteElo \"\"]
[BlackElo \"\"]
[ECO \"C49\"]

1.e4 e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"

    let testGame2= "
[Event \"Braingames WCC\"]
[Site \"London ENG\"]
[Date \"2000.11.02\"]
[Round \"15\"]
[White \"Kasparov,G\"]
[Black \"Kramnik,V\"]
[Result \"1/2-1/2\"]
[WhiteElo \"2849\"]
[BlackElo \"2770\"]
[ECO \"E05\"]

1.d4 Nf6 2.c4 e6 3.g3 d5 4.Bg2 Be7 5.Nf3 O-O 6.O-O dxc4 7.Qc2 a6 8.Qxc4 b5
9.Qc2 Bb7 10.Bd2 Be4 11.Qc1 Bb7 12.Bf4 Bd6 13.Nbd2 Nbd7 14.Nb3 Bd5 15.Rd1 Qe7
16.Ne5 Bxg2 17.Kxg2 Nd5 18.Nc6 Nxf4+ 19.Qxf4 Qe8 20.Qf3 e5 21.dxe5 Nxe5 22.Nxe5 Qxe5
23.Rd2 Rae8 24.e3 Re6 25.Rad1 Rf6 26.Qd5 Qe8 27.Rc1 g6 28.Rdc2 h5 29.Nd2 Rf5
30.Qe4 c5 31.Qxe8 Rxe8 32.e4 Rfe5 33.f4 R5e6 34.e5 Be7 35.b3 f6 36.Nf3 fxe5
37.Nxe5 Rd8 38.h4 Rd5  1/2-1/2"
    let gm1 = testGame1|>FsChessPgn.NET.PgnReader.ReadFromString|>List.head
    let gm2 = testGame2|>FsChessPgn.NET.PgnReader.ReadFromString|>List.head

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game1() =
        let ngm1 = gm1|>Game.SetpMoves
        Assert.AreEqual(gm1.MoveText.Length,ngm1.MoveText.Length)
        let last = ngm1.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("4rk2/p1p2ppQ/3q4/2p2N2/p7/2P5/PP2bPPP/4R1K1 b - - 3 24",lastfen)

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game2() =
        let ngm2 = gm2|>Game.SetpMoves
        Assert.AreEqual(gm2.MoveText.Length,ngm2.MoveText.Length)
        let last = ngm2.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("6k1/4b3/p3r1p1/1pprN2p/5P1P/1P4P1/P1R3K1/2R5 w - - 1 39",lastfen)
