namespace FsChessPgn.Test

open System.IO
open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type FormatterTest()=

    let TestGameString =
        @"[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]

{some moves} 1-0"

    let _testGame = 
        {GameEMP with Event="Breslau";Site="Breslau";Year=Some(1879);WhitePlayer="Tarrasch, Siegbert";BlackPlayer="Mendelsohn, J.";Result=GameResult.WhiteWins;MoveText=[CommentEntry("some moves");GameEndEntry(GameResult.WhiteWins)]}

    [<TestMethod>]
    member this.Format_should_accept_TextWriter() =
        let writer = new StringWriter()
        Formatter.Format(_testGame, writer)
        let act = writer.ToString()
        Assert.AreEqual(TestGameString, act)

    [<TestMethod>]
    member this.Format_should_format_correctly() =
        let act = Formatter.FormatStr(_testGame)
        Assert.AreEqual(TestGameString, act)
