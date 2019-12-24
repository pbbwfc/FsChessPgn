namespace FsChessPgn.Test

open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type FENTest()=

    [<TestMethod>]
    member this.FEN_Start_ToStr() =
        let fen = FEN.Start
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen|>FEN.ToStr)

    [<TestMethod>]
    member this.FEN_Start_FromBd() =
        let bd = Board.Start
        let fen = bd|>FEN.FromBd
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen|>FEN.ToStr)

    [<TestMethod>]
    member this.FEN_Start_FromStr() =
        let str = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        let fen = str|>FEN.Parse
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen|>FEN.ToStr)
