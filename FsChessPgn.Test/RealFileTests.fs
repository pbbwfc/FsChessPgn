namespace FsChessPgn.Test

open FsChessPgn.NET

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type RealFileTests () =
    let TestSet = "TestExamples/RealGames/"

    [<TestMethod>]
    member this.ChessInformantSample() =
        let db = Games.ReadFromFile(TestSet+"chess-informant-sample.pgn")

        Assert.AreEqual(db|>Seq.length,5)

    [<TestMethod>]
    member this.DemoGames() =
        let db = Games.ReadFromFile(TestSet+"demoGames.pgn")

        Assert.AreEqual(db|>Seq.length,2)

    [<TestMethod>]
    member this.Lon09R5() =
        let db = Games.ReadFromFile(TestSet+"Lon09R5.pgn")

        Assert.AreEqual(db|>Seq.length,4)

    [<TestMethod>]
    member this.Tilb98R2() =
        let db = Games.ReadFromFile(TestSet+"Tilb98R2.pgn")

        Assert.AreEqual(db|>Seq.length,6)

