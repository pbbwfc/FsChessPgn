namespace FsChessPgn.Test

open System.IO
open FsChessPgn.NET

open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Text

[<TestClass>]
type ParserTest () =

    let NormalGame = @"
[Event ""Breslau""]
[Site ""Breslau""]
[Date ""1879.??.??""]
[Round ""?""]
[White ""Tarrasch, Siegbert""]
[Black ""Mendelsohn, J.""]
[Result ""1-0""]
[WhiteElo """"]
[BlackElo """"]
[ECO ""C49""]

1.e4 e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"

    let PrepareStream(content:string) = new MemoryStream(Encoding.UTF8.GetBytes(content))
    let PrepareFile(fileName:string,content:string)=
        let file = new StreamWriter(fileName)
        file.WriteLine(content)
        file.Close()

    [<TestMethod>]
    member this.ReadFromStream_should_return_empty_Database_if_data_is_empty() =
        let stream = PrepareStream("")
        let db = PgnReader.ReadFromStream(stream)
        Assert.AreEqual(db|>Seq.length,0)

    [<TestMethod>]
    member this.ReadFromStream_should_return_read_game_from_stream() =
        let stream = PrepareStream(NormalGame)
        let db = PgnReader.ReadFromStream(stream)
        Assert.AreEqual(db|>Seq.length,1)

    [<TestMethod>]
    member this.ReadFromFile_should_return_read_game_from_file() =
        PrepareFile("one-game.pgn", NormalGame)
        let db = PgnReader.ReadFromFile("one-game.pgn")
        Assert.AreEqual(db|>Seq.length,1)

    [<TestMethod>]
    member this.ReadFromString_should_read_game_from_string() =
        let db = PgnReader.ReadFromString(NormalGame)
        Assert.AreEqual(db|>Seq.length,1)
