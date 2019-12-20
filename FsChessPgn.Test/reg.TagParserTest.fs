namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TagRegParserTests() =

    [<TestMethod>]
    member this.pTag_should_accept_tags_which_are_prefixes_of_others() =
        let gml = RegParse.ReadFromString "[WhiteSomethingFoo \"\"]"
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        Assert.AreEqual(2013, gm.Year.Value)
        Assert.AreEqual(5, gm.Month.Value)
        Assert.AreEqual(15, gm.Day.Value)

    [<TestMethod>]
    member this.pTag_should_create_a_PgnDateTag_object_from_a_valid_tag() =
        let gml = RegParse.ReadFromString "[Date \"2013.05.15\"]"
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        Assert.AreEqual(2013, gm.Year.Value)
        Assert.AreEqual(5, gm.Month.Value)
        Assert.AreEqual(15, gm.Day.Value)

    //[<TestMethod>]
    //member this.pTag_should_accept_only_the_year_as_date() =
    //    let tag= parse pTag "[Date \"2013\"]"
    //    Assert.IsInstanceOfType(tag,typeof<PgnDateTag>) |> ignore
    //    Assert.AreEqual("Date", tag.Name)
    //    Assert.AreEqual(2013, (tag :?> PgnDateTag).Year.Value)
    //    Assert.IsFalse((tag :?> PgnDateTag).Month.IsSome)
    //    Assert.IsFalse((tag :?> PgnDateTag).Day.IsSome)

    //[<TestMethod>]
    //member this.pTag_should_create_a_PgnRoundTag_object_from_a_valid_tag() =
    //    let tag= parse pTag "[Round \"13\"]"
    //    Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
    //    Assert.AreEqual("Round", tag.Name)
    //    Assert.AreEqual("13", tag.Value)

    //[<TestMethod>]
    //member this.pTag_should_create_PgnRoundTag_object_from_two_tags_in_sequence() =
    //    let tag = parse pTag @"[Round ""?""][White ""Tarrasch, Siegbert""]"

    //    Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
    //    Assert.AreEqual("Round", tag.Name)
    //    Assert.AreEqual("?", tag.Value)

    //[<TestMethod>]
    //member this.pTag_should_accept_non_numeric_rounds() =
    //    let tag= parse pTag "[Round \"4.1\"]"
    //    Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
    //    Assert.AreEqual("Round", tag.Name)
    //    Assert.AreEqual("4.1", tag.Value)

    //[<TestMethod>]
    //member this.pTag_should_create_a_PgnResultTag_object_from_a_valid_tag() =
    //    let tag= parse pTag "[Result \"1-0\"]"
    //    Assert.IsInstanceOfType(tag,typeof<PgnResultTag>) |> ignore
    //    Assert.AreEqual("Result", tag.Name)
    //    Assert.AreEqual(GameResult.WhiteWins, (tag :?> PgnResultTag).Result)

    //[<TestMethod>]
    //member this.pTag_should_create_a_FenTag_object_from_a_valid_tag() =
    //    let tag= parse pTag "[FEN \"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\"]"
    //    Assert.IsInstanceOfType(tag,typeof<FenTag>) |> ignore

    //    let setup= (tag :?> FenTag).Setup

    //    Assert.AreEqual(Piece.BRook, setup.Pieceat.[Sq(FileA,Rank8)|>int])
    //    Assert.AreEqual(Piece.WKnight, setup.Pieceat.[Sq(FileB,Rank1)|>int])
    //    Assert.AreEqual(Piece.BBishop, setup.Pieceat.[Sq(FileC,Rank8)|>int])
    //    Assert.AreEqual(Piece.EMPTY, setup.Pieceat.[Sq(FileC,Rank3)|>int])
    //    Assert.AreEqual(Piece.WKing, setup.Pieceat.[Sq(FileE,Rank1)|>int])

    //    Assert.AreEqual(Player.White, setup.Whosturn)

    //    Assert.AreEqual(true, setup.CastleWS)
    //    Assert.AreEqual(true, setup.CastleWL)
    //    Assert.AreEqual(true, setup.CastleBS)
    //    Assert.AreEqual(true, setup.CastleBL)

    //    Assert.AreEqual(OUTOFBOUNDS, setup.Enpassant)

    //    Assert.AreEqual(0, setup.Fiftymove)
    //    Assert.AreEqual(1, setup.Fullmove)


    //[<TestMethod>]
    //member this.pTag_should_create_a_FenTag_object_from_another_valid_tag() =
    //    let tag= parse pTag "[FEN \"rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR b Kq c6 1 2\"]"
    //    Assert.IsInstanceOfType(tag,typeof<FenTag>) |> ignore

    //    let setup= (tag :?> FenTag).Setup

    //    Assert.AreEqual(Piece.BRook, setup.Pieceat.[Sq(FileA,Rank8)|>int])
    //    Assert.AreEqual(Piece.BPawn, setup.Pieceat.[Sq(FileB,Rank7)|>int])
    //    Assert.AreEqual(Piece.BPawn, setup.Pieceat.[Sq(FileC,Rank5)|>int])
    //    Assert.AreEqual(Piece.WPawn, setup.Pieceat.[Sq(FileE,Rank4)|>int])
    //    Assert.AreEqual(Piece.EMPTY, setup.Pieceat.[Sq(FileE,Rank2)|>int])
    //    Assert.AreEqual(Piece.WKing, setup.Pieceat.[Sq(FileE,Rank1)|>int])

    //    Assert.AreEqual(Player.Black, setup.Whosturn)

    //    Assert.AreEqual(true, setup.CastleWS)
    //    Assert.AreEqual(false, setup.CastleWL)
    //    Assert.AreEqual(false, setup.CastleBS)
    //    Assert.AreEqual(true, setup.CastleBL)

    //    Assert.AreEqual(C6, setup.Enpassant)

    //    Assert.AreEqual(1, setup.Fiftymove)
    //    Assert.AreEqual(2, setup.Fullmove)
