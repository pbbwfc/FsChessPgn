namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveGenerateTest()=

    [<TestMethod>]
    member this.Generate_initial_moves() =
        let bd = Board.Start
        let fen = bd|>FEN.FromBd|>FEN.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen)
        let mvs = bd|>MoveGenerate.GenMovesLegal
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.Desc
        let bd1 = bd|>Board.MoveApply mvh
        let fen1 = bd1|>FEN.FromBd|>FEN.ToStr

        Assert.AreEqual(20, mvs.Length)
        Assert.AreEqual("h2h4",mvhstr)
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/7P/8/PPPPPPP1/RNBQKBNR b KQkq h3 0 1",fen1)

