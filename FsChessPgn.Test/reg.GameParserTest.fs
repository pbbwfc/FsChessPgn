#nowarn "25"
namespace FsChessPgn.Test

open FsChess
open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type GameRegParserTest() =
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

    let testGame3 = "
[Event \"?\"]
[Site \"?\"]
[Date \"2013.06.02\"]
[Round \"?\"]
[White \"\"]
[Black \"\"]
[Result \"*\"]
[SetUp \"1\"]
[FEN \"4k3/8/8/8/8/8/4P3/4K3 w - - 5 39\"]

*"

    [<TestMethod>]
    member this.pGame_should_accept_a_standard_pgn_game() =
        let gml= Games.ReadFromString testGame1
        Assert.AreEqual(1, gml.Length)
        let game = gml.Head
        Assert.AreEqual("Tarrasch, Siegbert", game.WhitePlayer)
        Assert.AreEqual("Mendelsohn, J.", game.BlackPlayer)
        Assert.AreEqual(null, game.BoardSetup)
        Assert.AreEqual("Breslau", game.Event)
        Assert.AreEqual("Breslau", game.Site)
        Assert.AreEqual(1879, game.Year.Value)
        Assert.IsFalse(game.Month.IsSome)
        Assert.IsFalse(game.Day.IsSome)
        Assert.AreEqual(48, game.MoveText.Length) //24 move pairs and finish tag
        let (GameEndEntry gr) = game.MoveText.[47]
        Assert.AreEqual(GameResult.WhiteWins, gr)

    [<TestMethod>]
    member this.pGame_should_accept_a_standard_pgn_game2() =
        let gml= Games.ReadFromString testGame2
        Assert.AreEqual(1, gml.Length)
        let game = gml.Head
        Assert.AreEqual("Braingames WCC", game.Event)
        Assert.AreEqual("London ENG", game.Site)
        Assert.AreEqual(2000, game.Year.Value)
        Assert.AreEqual(11, game.Month.Value)
        Assert.AreEqual(2, game.Day.Value)
        Assert.AreEqual("15", game.Round)
        Assert.AreEqual("Kasparov,G", game.WhitePlayer)
        Assert.AreEqual("Kramnik,V", game.BlackPlayer)
        Assert.AreEqual(3, game.AdditionalInfo.Count)
        Assert.AreEqual(77, game.MoveText.Length)
        Assert.AreEqual("1/2-1/2", game.Result|>PgnWrite.ResultString)
        Assert.AreEqual(GameResult.Draw, game.Result)

    [<TestMethod>]
    member this.pGame_should_accept_a_standard_pgn_game3() =
        let gml= Games.ReadFromString testGame3
        Assert.AreEqual(1, gml.Length)
        let game = gml.Head
        let setup= game.BoardSetup.Value
        Assert.AreEqual(Piece.BKing, setup.PieceAt.[Sq(FileE,Rank8)|>int])
        Assert.AreEqual(Piece.WPawn, setup.PieceAt.[Sq(FileE,Rank2)|>int])
        Assert.AreEqual(Piece.WKing, setup.PieceAt.[Sq(FileE,Rank1)|>int])
        Assert.AreEqual(Player.White, setup.WhosTurn)
        Assert.AreEqual(CstlFlgs.EMPTY, setup.CastleRights)
        Assert.AreEqual(OUTOFBOUNDS, setup.EnPassant)
        Assert.AreEqual(5, setup.Fiftymove)
        Assert.AreEqual(39, setup.Fullmove)

