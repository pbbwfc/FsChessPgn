﻿namespace fspgn.Test

open System
open System.IO
open fspgn.Data
open fspgn.Data.PgnTextTypes

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
        {pGameEMP with Event="Breslau";Site="Breslau";Year=Some(1879);WhitePlayer="Tarrasch, Siegbert";BlackPlayer="Mendelsohn, J.";Result=GameResult.WhiteWins;MoveText=[CommentEntry("some moves");GameEndEntry(GameResult.WhiteWins)]}

    [<TestMethod>]
    member this.Format_should_accept_TextWriter() =
        let writer = new StringWriter()
        Formatter.Format(_testGame, writer)
        let act = writer.ToString()
        Assert.AreEqual(TestGameString, act)
