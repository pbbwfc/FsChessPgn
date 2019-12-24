namespace FsChessPgn.Test

open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveUtilTest()=
    let brd1 = Board.Start
    let mv1 = "e4"|>MoveUtil.fromSAN brd1

    [<TestMethod>]
    member this.MoveUtil_Parse() =
        Assert.AreEqual(E2,mv1|>Move.From)
        Assert.AreEqual(E4,mv1|>Move.To)

    [<TestMethod>]
    member this.MoveUtil_Desc() =
        Assert.AreEqual("e2e4",mv1|>MoveUtil.Desc)

    [<TestMethod>]
    member this.MoveUtil_DescBd() =
        Assert.AreEqual("e4",brd1|>MoveUtil.DescBd mv1)

    [<TestMethod>]
    member this.MoveUtil_Descs() =
        Assert.AreEqual("1. e4 ",MoveUtil.Descs [mv1] brd1 true)

    [<TestMethod>]
    member this.MoveUtil_FindMv() =
        let mv = brd1|>MoveUtil.FindMv "e2e4"
        Assert.AreEqual(E2,mv.Value|>Move.From)
        Assert.AreEqual(E4,mv.Value|>Move.To)
