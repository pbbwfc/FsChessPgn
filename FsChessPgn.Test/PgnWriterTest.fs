namespace FsChessPgn.Test

open FsChess
open System.IO
open FsChessPgn

open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Text

[<TestClass>]
type PgnWriterTest () =
    let TestGameString = @"[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]
[WhiteElo ""-""]
[BlackElo ""-""]


{some moves} 1-0

"
    let _testGame = {GameEMP with Event="Breslau";Site="Breslau";Year=Some(1879);WhitePlayer="Tarrasch, Siegbert";BlackPlayer ="Mendelsohn, J.";Result=GameResult.WhiteWins;MoveText=[CommentEntry("some moves");GameEndEntry(GameResult.WhiteWins)]}

    [<TestMethod>]
    member this.Write_should_write_game_correctly() =
        let stream = new MemoryStream()
        let db = [_testGame]
        PgnWriter.WriteStream(stream,db)
        let actual = Encoding.UTF8.GetString(stream.ToArray())

        Assert.AreEqual(TestGameString, actual)

    [<TestMethod>]
    member this.parser_should_read_written_game_correctly() =
        let stream = new MemoryStream()
        let db = [_testGame]
        PgnWriter.WriteStream(stream,db)
        let writtenResult = Encoding.UTF8.GetString(stream.ToArray())
        let actualDb = Games.ReadFromString(writtenResult)
        let actual = PgnWriter.WriteString(actualDb)

        Assert.AreEqual(TestGameString, actual)

