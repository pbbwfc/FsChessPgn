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

    [<TestMethod>]
    member this.MoveGenerate_Moves1() =
        let bd = Board.FromStr "rnbqkbnr/ppp2ppp/3p4/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 3"
        let mvs = bd|>MoveGenerate.GenMovesLegal
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.Desc
        Assert.AreEqual(27, mvs.Length)
        Assert.AreEqual("f3e5",mvhstr)

    [<TestMethod>]
    member this.MoveGenerate_Moves2() =
        let bd = Board.FromStr "rnbqkbnr/ppp2ppp/3p4/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 3"
        let mvs = bd|>MoveGenerate.GenMovesLegal
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.Desc
        Assert.AreEqual(6, mvs.Length)
        Assert.AreEqual("c7c6",mvhstr)

    [<TestMethod>]
    member this.MoveGenerate_Moves3() =
        let bd = Board.FromStr "r1bqkb1r/ppp2ppp/2Bp1n2/4p3/4P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 0 5"
        let mvs = bd|>MoveGenerate.GenMovesLegal
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.Desc
        Assert.AreEqual(5, mvs.Length)
        Assert.AreEqual("b7c6",mvhstr)

