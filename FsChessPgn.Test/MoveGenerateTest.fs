namespace FsChessPgn.Test

open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveGenerateTest()=

    [<TestMethod>]
    member this.Generate_initial_moves() =
        let bd = Board.Start
        let fen = bd|>Board.ToStr
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",fen)
        let mvs = bd|>MoveGenerate.AllMoves
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.toUci
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
        let mvs = bd|>MoveGenerate.AllMoves
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.toUci
        Assert.AreEqual(27, mvs.Length)
        Assert.AreEqual("h2h4",mvhstr)
        let kmvs = bd|>MoveGenerate.KingMoves
        let kmvh = kmvs.Head
        let kmvhstr = kmvh|>MoveUtil.toUci
        Assert.AreEqual(1, kmvs.Length)
        Assert.AreEqual("e1e2",kmvhstr)
        let cmvs = bd|>MoveGenerate.CastleMoves
        Assert.AreEqual(0, cmvs.Length)
        let nmvs = bd|>MoveGenerate.KnightMoves
        let nmvh = nmvs.Head
        let nmvhstr = nmvh|>MoveUtil.toUci
        Assert.AreEqual(7, nmvs.Length)
        Assert.AreEqual("b1c3",nmvhstr)
        let bmvs = bd|>MoveGenerate.BishopMoves
        let bmvh = bmvs.Head
        let bmvhstr = bmvh|>MoveUtil.toUci
        Assert.AreEqual(5, bmvs.Length)
        Assert.AreEqual("f1e2",bmvhstr)
        let rmvs = bd|>MoveGenerate.RookMoves
        let rmvh = rmvs.Head
        let rmvhstr = rmvh|>MoveUtil.toUci
        Assert.AreEqual(1, rmvs.Length)
        Assert.AreEqual("h1g1",rmvhstr)
        let qmvs = bd|>MoveGenerate.QueenMoves
        let qmvh = qmvs.Head
        let qmvhstr = qmvh|>MoveUtil.toUci
        Assert.AreEqual(1, qmvs.Length)
        Assert.AreEqual("d1e2",qmvhstr)
        let pmvs = bd|>MoveGenerate.PawnMoves
        let pmvh = pmvs.Head
        let pmvhstr = pmvh|>MoveUtil.toUci
        Assert.AreEqual(12, pmvs.Length)
        Assert.AreEqual("h2h4",pmvhstr)

    [<TestMethod>]
    member this.MoveGenerate_Moves2() =
        let bd = Board.FromStr "rnbqkbnr/ppp2ppp/3p4/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 3"
        let mvs = bd|>MoveGenerate.AllMoves
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.toUci
        Assert.AreEqual(6, mvs.Length)
        Assert.AreEqual("c7c6",mvhstr)
        let kmvs = bd|>MoveGenerate.KingMoves
        let kmvh = kmvs.Head
        let kmvhstr = kmvh|>MoveUtil.toUci
        Assert.AreEqual(1, kmvs.Length)
        Assert.AreEqual("e8e7",kmvhstr)
        let cmvs = bd|>MoveGenerate.CastleMoves
        Assert.AreEqual(0, cmvs.Length)
        let nmvs = bd|>MoveGenerate.KnightMoves
        let nmvh = nmvs.Head
        let nmvhstr = nmvh|>MoveUtil.toUci
        Assert.AreEqual(2, nmvs.Length)
        Assert.AreEqual("b8c6",nmvhstr)
        let bmvs = bd|>MoveGenerate.BishopMoves
        let bmvh = bmvs.Head
        let bmvhstr = bmvh|>MoveUtil.toUci
        Assert.AreEqual(1, bmvs.Length)
        Assert.AreEqual("c8d7",bmvhstr)
        let rmvs = bd|>MoveGenerate.RookMoves
        Assert.AreEqual(0, rmvs.Length)
        let qmvs = bd|>MoveGenerate.QueenMoves
        let qmvh = qmvs.Head
        let qmvhstr = qmvh|>MoveUtil.toUci
        Assert.AreEqual(1, qmvs.Length)
        Assert.AreEqual("d8d7",qmvhstr)
        let pmvs = bd|>MoveGenerate.PawnMoves
        let pmvh = pmvs.Head
        let pmvhstr = pmvh|>MoveUtil.toUci
        Assert.AreEqual(1, pmvs.Length)
        Assert.AreEqual("c7c6",pmvhstr)

    [<TestMethod>]
    member this.MoveGenerate_Moves3() =
        let bd = Board.FromStr "r1bqkbnr/ppp2ppp/2np4/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 4"
        let mvs = bd|>MoveGenerate.AllMoves
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.toUci
        Assert.AreEqual(32, mvs.Length)
        Assert.AreEqual("e1g1",mvhstr)
        let kmvs = bd|>MoveGenerate.KingMoves
        let kmvh = kmvs.Head
        let kmvhstr = kmvh|>MoveUtil.toUci
        Assert.AreEqual(2, kmvs.Length)
        Assert.AreEqual("e1f1",kmvhstr)
        let cmvs = bd|>MoveGenerate.CastleMoves
        let cmvh = cmvs.Head
        let cmvhstr = cmvh|>MoveUtil.toUci
        Assert.AreEqual(1, cmvs.Length)
        Assert.AreEqual("e1g1",cmvhstr)
        let nmvs = bd|>MoveGenerate.KnightMoves
        let nmvh = nmvs.Head
        let nmvhstr = nmvh|>MoveUtil.toUci
        Assert.AreEqual(7, nmvs.Length)
        Assert.AreEqual("b1c3",nmvhstr)
        let bmvs = bd|>MoveGenerate.BishopMoves
        let bmvh = bmvs.Head
        let bmvhstr = bmvh|>MoveUtil.toUci
        Assert.AreEqual(7, bmvs.Length)
        Assert.AreEqual("b5f1",bmvhstr)
        let rmvs = bd|>MoveGenerate.RookMoves
        let rmvh = rmvs.Head
        let rmvhstr = rmvh|>MoveUtil.toUci
        Assert.AreEqual(2, rmvs.Length)
        Assert.AreEqual("h1g1",rmvhstr)
        let qmvs = bd|>MoveGenerate.QueenMoves
        let qmvh = qmvs.Head
        let qmvhstr = qmvh|>MoveUtil.toUci
        Assert.AreEqual(1, qmvs.Length)
        Assert.AreEqual("d1e2",qmvhstr)
        let pmvs = bd|>MoveGenerate.PawnMoves
        let pmvh = pmvs.Head
        let pmvhstr = pmvh|>MoveUtil.toUci
        Assert.AreEqual(12, pmvs.Length)
        Assert.AreEqual("h2h4",pmvhstr)

    [<TestMethod>]
    member this.MoveGenerate_Moves4() =
        let bd = Board.FromStr "r1bqkb1r/ppp2ppp/2Bp1n2/4p3/4P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 0 5"
        let mvs = bd|>MoveGenerate.AllMoves
        let mvh = mvs.Head
        let mvhstr = mvh|>MoveUtil.toUci
        Assert.AreEqual(5, mvs.Length)
        Assert.AreEqual("b7c6",mvhstr)
        let kmvs = bd|>MoveGenerate.KingMoves
        let kmvh = kmvs.Head
        let kmvhstr = kmvh|>MoveUtil.toUci
        Assert.AreEqual(1, kmvs.Length)
        Assert.AreEqual("e8e7",kmvhstr)
        let cmvs = bd|>MoveGenerate.CastleMoves
        Assert.AreEqual(0, cmvs.Length)
        let nmvs = bd|>MoveGenerate.KnightMoves
        let nmvh = nmvs.Head
        let nmvhstr = nmvh|>MoveUtil.toUci
        Assert.AreEqual(1, nmvs.Length)
        Assert.AreEqual("f6d7",nmvhstr)
        let bmvs = bd|>MoveGenerate.BishopMoves
        let bmvh = bmvs.Head
        let bmvhstr = bmvh|>MoveUtil.toUci
        Assert.AreEqual(1, bmvs.Length)
        Assert.AreEqual("c8d7",bmvhstr)
        let rmvs = bd|>MoveGenerate.RookMoves
        Assert.AreEqual(0, rmvs.Length)
        let qmvs = bd|>MoveGenerate.QueenMoves
        let qmvh = qmvs.Head
        let qmvhstr = qmvh|>MoveUtil.toUci
        Assert.AreEqual(1, qmvs.Length)
        Assert.AreEqual("d8d7",qmvhstr)
        let pmvs = bd|>MoveGenerate.PawnMoves
        let pmvh = pmvs.Head
        let pmvhstr = pmvh|>MoveUtil.toUci
        Assert.AreEqual(1, pmvs.Length)
        Assert.AreEqual("b7c6",pmvhstr)

