namespace FsChessPgn.Test

open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type FileTestCases () =

    let TestFolder = @"TestExamples/"

    [<TestMethod>]
    member this.EmptyFile() =
        let db = Games.ReadFromFile(TestFolder + "empty-file.pgn")
        Assert.AreEqual(db|>Seq.length,0)

    [<TestMethod>]
    member this.SimpleGame() =
        let db = Games.ReadFromFile(TestFolder + "simple-game.pgn")
        Assert.AreEqual(db|>Seq.length,1)

    [<TestMethod>]
    member this.TimeAnnotatedGames() =
        let db = Games.ReadFromFile(TestFolder + "time-annotated-games.pgn")
        Assert.AreEqual(db|>Seq.length,4)
