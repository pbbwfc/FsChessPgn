namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveGenerateTest()=

    [<TestMethod>]
    member this.Generate_initial_moves() =
        let bd = Board.Start
        let fen = bd|>Board.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen)
        let mvs = bd|>MoveGenerate.GenMovesLegal
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.Desc
        let bd1 = bd|>Board.MoveApply mvh
        let fen1 = bd1|>Board.ToStr

        Assert.AreEqual(20, mvs.Length)
        Assert.AreEqual("h2h4",mvhstr)
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/7P/8/PPPPPPP1/RNBQKBNR b KQkq h3 0 1",fen1)

    [<TestMethod>]
    member this.MoveGenerate_IsMate() =
        let bd = Board.Start
        Assert.AreEqual(false, bd|>MoveGenerate.IsDrawByStalemate)
        Assert.AreEqual(false, bd|>MoveGenerate.IsMate)