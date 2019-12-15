namespace FsChessPgn.Test

open FsChessPgn.Data
open FsChessPgn.PgnParsers.PgnTags
open FsChessPgn.PgnParsers.Tag
open FsChessPgn.Test.TestBase

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TagParserTests() =
    // TODO: Expect exception, so wait with converting this
    // [<TestMethod>]
    // member this.pTag_should_fail_if_expressions_starts_with_non_bracket() =
    //     tryParse pTag "test"

    [<TestMethod>]
    member this.pTag_should_accept_random_spaces_before_and_after_brackets() =
        tryParse pTag "          [Event \"Foo\"]"
        tryParse pTag "  \t \n  [Event \"Foo\"]"
        tryParse pTag "  \t \n  [Event \"Foo\"]"
        tryParse pTag "[  \t \tEvent \"Foo\"\t \n ]  \t \n   \t \n"
        tryParse pTag "  \t \n  [  \t \n   \t \n Event \"Foo\"  \t \n   \t \n ]  \t\n\t \n   \t\t\n\n   \t \n\n "

    [<TestMethod>]
    member this.pTag_should_accept_random_spaces_between_tag_name_and_value() =
        tryParse pTag "[Event\t \n   \t \"Foo\"]"
        tryParse pTag "[Event \"Foo\"]"
        tryParse pTag "[Event \t \n   \t\"Foo\"]"
        tryParse pTag "[  \t \tEvent         \"Foo\"\t \n ]  \t \n   \t \n"
        tryParse pTag "  \t \n  [  \t \n   \t \n Event \t \n   \t \n  \"Foo\"  \t \n   \t \n ]  \t\n\t \n   \t\t\n\n   \t \n\n "

    [<TestMethod>]
    ///<see href="see http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm#c8.1.1" />
    member this.pTag_should_allow_tag_names_from_SevenTagRoster() =
        tryParse pTag "[Date \"2013.05.18\"]"
        tryParse pTag "[Round \"5\"]"
        tryParse pTag "[Result \"*\"]"

        let basicTagNames = ["Event"; "Site"; "White"; "Black";]
        let parseTag x = tryParse pTag ("["+ x + " \"Foo\"]")
        List.map parseTag basicTagNames |> ignore
        ()

    [<TestMethod>]
    ///<see href="http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm#c9" />
    member this.pTag_should_allow_suplemental_tag_names() =
        let allowedTagNames =
            ["WhiteTitle"; "BlackTitle"; "WhiteElo"; "BlackElo"; "WhiteUSCF"; "BlackUSCF"; "WhiteNA"; "BlackNA"; "WhiteType"; "BlackType";
            "EventDate"; "EventSponsor"; "Section"; "Stage"; "Board";
            "Opening"; "Variation"; "SubVariation";
            "ECO"; "NIC"; "Time"; "UTCTime"; "UTCDate";
            "TimeControl";
            "SetUp";
            "Termination";
            "Annotator"; "Mode"; "PlyCount"]
        let parseTag x = tryParse pTag ("["+ x + " \"Foo\"]")
        List.map parseTag allowedTagNames |> ignore
        ()

    [<TestMethod>]
    member this.pTag_should_accept_tags_which_are_prefixes_of_others() =
        tryParse pTag "[WhiteSomethingFoo \"\"]"


    [<TestMethod>]
    member this.pTag_should_create_a_PgnDateTag_object_from_a_valid_tag() =
        let tag= parse pTag "[Date \"2013.05.15\"]"
        Assert.IsInstanceOfType(tag,typeof<PgnDateTag>) |> ignore
        Assert.AreEqual("Date", tag.Name)
        Assert.AreEqual(2013, (tag :?> PgnDateTag).Year.Value)
        Assert.AreEqual(5, (tag :?> PgnDateTag).Month.Value)
        Assert.AreEqual(15, (tag :?> PgnDateTag).Day.Value)

    [<TestMethod>]
    member this.pTag_should_accept_only_the_year_as_date() =
        let tag= parse pTag "[Date \"2013\"]"
        Assert.IsInstanceOfType(tag,typeof<PgnDateTag>) |> ignore
        Assert.AreEqual("Date", tag.Name)
        Assert.AreEqual(2013, (tag :?> PgnDateTag).Year.Value)
        Assert.IsFalse((tag :?> PgnDateTag).Month.IsSome)
        Assert.IsFalse((tag :?> PgnDateTag).Day.IsSome)

    [<TestMethod>]
    member this.pTag_should_create_a_PgnRoundTag_object_from_a_valid_tag() =
        let tag= parse pTag "[Round \"13\"]"
        Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
        Assert.AreEqual("Round", tag.Name)
        Assert.AreEqual("13", tag.Value)

    [<TestMethod>]
    member this.pTag_should_create_PgnRoundTag_object_from_two_tags_in_sequence() =
        let tag = parse pTag @"[Round ""?""][White ""Tarrasch, Siegbert""]"

        Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
        Assert.AreEqual("Round", tag.Name)
        Assert.AreEqual("?", tag.Value)

    [<TestMethod>]
    member this.pTag_should_accept_non_numeric_rounds() =
        let tag= parse pTag "[Round \"4.1\"]"
        Assert.IsInstanceOfType(tag,typeof<PgnTag>) |> ignore
        Assert.AreEqual("Round", tag.Name)
        Assert.AreEqual("4.1", tag.Value)

    [<TestMethod>]
    member this.pTag_should_create_a_PgnResultTag_object_from_a_valid_tag() =
        let tag= parse pTag "[Result \"1-0\"]"
        Assert.IsInstanceOfType(tag,typeof<PgnResultTag>) |> ignore
        Assert.AreEqual("Result", tag.Name)
        Assert.AreEqual(GameResult.WhiteWins, (tag :?> PgnResultTag).Result)

    [<TestMethod>]
    member this.pTag_should_create_a_FenTag_object_from_a_valid_tag() =
        let tag= parse pTag "[FEN \"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\"]"
        Assert.IsInstanceOfType(tag,typeof<FenTag>) |> ignore

        let setup= (tag :?> FenTag).Setup

        Assert.AreEqual(Piece.BRook, setup.Pieceat.[Sq(FileA,Rank8)|>int])
        Assert.AreEqual(Piece.WKnight, setup.Pieceat.[Sq(FileB,Rank1)|>int])
        Assert.AreEqual(Piece.BBishop, setup.Pieceat.[Sq(FileC,Rank8)|>int])
        Assert.AreEqual(Piece.EMPTY, setup.Pieceat.[Sq(FileC,Rank3)|>int])
        Assert.AreEqual(Piece.WKing, setup.Pieceat.[Sq(FileE,Rank1)|>int])

        Assert.AreEqual(Player.White, setup.Whosturn)

        Assert.AreEqual(true, setup.CastleWS)
        Assert.AreEqual(true, setup.CastleWL)
        Assert.AreEqual(true, setup.CastleBS)
        Assert.AreEqual(true, setup.CastleBL)

        Assert.AreEqual(OUTOFBOUNDS, setup.Enpassant)

        Assert.AreEqual(0, setup.Fiftymove)
        Assert.AreEqual(1, setup.Fullmove)


    [<TestMethod>]
    member this.pTag_should_create_a_FenTag_object_from_another_valid_tag() =
        let tag= parse pTag "[FEN \"rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR b Kq c6 1 2\"]"
        Assert.IsInstanceOfType(tag,typeof<FenTag>) |> ignore

        let setup= (tag :?> FenTag).Setup

        Assert.AreEqual(Piece.BRook, setup.Pieceat.[Sq(FileA,Rank8)|>int])
        Assert.AreEqual(Piece.BPawn, setup.Pieceat.[Sq(FileB,Rank7)|>int])
        Assert.AreEqual(Piece.BPawn, setup.Pieceat.[Sq(FileC,Rank5)|>int])
        Assert.AreEqual(Piece.WPawn, setup.Pieceat.[Sq(FileE,Rank4)|>int])
        Assert.AreEqual(Piece.EMPTY, setup.Pieceat.[Sq(FileE,Rank2)|>int])
        Assert.AreEqual(Piece.WKing, setup.Pieceat.[Sq(FileE,Rank1)|>int])

        Assert.AreEqual(Player.Black, setup.Whosturn)

        Assert.AreEqual(true, setup.CastleWS)
        Assert.AreEqual(false, setup.CastleWL)
        Assert.AreEqual(false, setup.CastleBS)
        Assert.AreEqual(true, setup.CastleBL)

        Assert.AreEqual(C6, setup.Enpassant)

        Assert.AreEqual(1, setup.Fiftymove)
        Assert.AreEqual(2, setup.Fullmove)
