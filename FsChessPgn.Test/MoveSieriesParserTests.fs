#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data
open FsChessPgn.Data.PgnTextTypes
open FsChessPgn.PgnParsers.Move
open FsChessPgn.PgnParsers.MoveSeries
open FsChessPgn.Test.TestBase

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveSeriesParserTest() =
    [<TestMethod>]
    member this.pMoveNumberIndicator_should_accept_simple_move_number() =
        tryParse pMoveNumberIndicator "1."
        tryParse pMoveNumberIndicator "104."

    [<TestMethod>]
    member this.pMoveNumberIndicator_should_accept_continued_move_number() =
        tryParse pMoveNumberIndicator "2..."
        tryParse pMoveNumberIndicator "54....."
        tryParse pMoveNumberIndicator "7…"

    [<TestMethod>]
    member this.pMoveNumberIndicator_should_accept_spaces_between_number_and_periods() =
        tryParse pMoveNumberIndicator "2  \t..."
        tryParse pMoveNumberIndicator "1    ."
        tryParse pMoveNumberIndicator "54\n....."
        tryParse pMoveNumberIndicator "7 …"

    [<TestMethod>]
    member this.pMoveSeriesEntry_should_accept_a_split_move_white() =
        let (HalfMoveEntry (mn,_,mv)) = (parse pMoveSeriesEntry "1. e2e4")
        let moveWhite = parse pMove "e2e4"

        Assert.AreEqual(moveWhite, mv)
        Assert.AreEqual(1, mn.Value)

    [<TestMethod>]
    member this.pMoveSeriesEntry_should_accept_a_single_move_by_white() =
        let (HalfMoveEntry (mn,ic,mv)) = (parse pMoveSeriesEntry "13.Nxd4")
        let move = parse pMove "Nxd4"

        Assert.AreEqual(move, mv)
        Assert.AreEqual(13, mn.Value)
        Assert.AreEqual(false, ic)

    [<TestMethod>]
    member this.pMoveSeriesEntry_should_accept_a_continued_move_by_black() =
        let (HalfMoveEntry (mn,ic,mv)) = (parse pMoveSeriesEntry "13... Ba6")
        let move = parse pMove "Ba6"

        Assert.AreEqual(move, mv)
        Assert.AreEqual(13, mn.Value)
        Assert.AreEqual(true, ic)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_a_moveSeries() =
        let moveSeries = parse pMoveSeries "1. e4 c5 2. Nf3 d6 3. Bb5+ Bd7"

        Assert.AreEqual(6, moveSeries.Length)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_a_moveSeries_with_split_moves() =
        let moveSeries = parse pMoveSeries "1. e4 c5 2. Nf3 \n 2... d6 3. Bb5+ Bd7"

        Assert.AreEqual(6, moveSeries.Length)

    [<TestMethod>]
    member this.pMoveEntry_should_accept_comments_in_braces() =
        let (CommentEntry str) = (parse pMoveSeriesEntry "{this is a comment}") 

        Assert.AreEqual("this is a comment", str)

    [<TestMethod>]
    member this.pMoveEntry_should_accept_comments_semicolon_comment() =
        let moveSeries = parse pMoveSeries "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
        3... a6"

        Assert.AreEqual(7, moveSeries.Length)

        let (CommentEntry str) = (moveSeries.Item(5)) 
        Assert.AreEqual("This opening is called the Ruy Lopez.", str)

    [<TestMethod>]
    member this.pMoveEntry_should_accept_multiple_line_comments() =
        let moveSeries = parse pMoveSeries "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
        ;Another comment
        3... a6"

        Assert.AreEqual(8, moveSeries.Length)

        let (CommentEntry str1) = (moveSeries.Item(5))
        let (CommentEntry str2) = (moveSeries.Item(6))
        Assert.AreEqual("This opening is called the Ruy Lopez.", str1)
        Assert.AreEqual("Another comment", str2)


    [<TestMethod>]
    member this.pEndOfGame_should_accept_Draw() =
        let (GameEndEntry gr) = (parse pEndOfGame "1/2 - 1/2")
        Assert.AreEqual(GameResult.Draw, gr)

    [<TestMethod>]
    member this.pEndOfGame_should_accept_utf8_draw() =
        let (GameEndEntry gr) = (parse pEndOfGame "½-½")
        Assert.AreEqual(GameResult.Draw, gr)

    [<TestMethod>]
    member this.pEndOfGame_should_accept_WhiteWin() =
        let (GameEndEntry gr) = (parse pEndOfGame "1-0")
        Assert.AreEqual(GameResult.WhiteWins, gr)

    [<TestMethod>]
    member this.pEndOfGame_should_accept_BlackWin() =
        let (GameEndEntry gr) = (parse pEndOfGame "0-1")
        Assert.AreEqual(GameResult.BlackWins, gr)

    [<TestMethod>]
    member this.pEndOfGame_should_accept_EndOpen() =
        let (GameEndEntry gr) = (parse pEndOfGame "*")
        Assert.AreEqual(GameResult.Open, gr)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_comment_after_end_game() =
        let entries = parse pMoveSeries "1-0 {impressive game!}"

        let (CommentEntry str) = (entries.Item(1))
        Assert.AreEqual("impressive game!", str)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_comment_before_moves() =
        let entries = parse pMoveSeries "{This game is gonna be awesome! Watch this} \n 1. e4 e5 2. Nf3"
        let (CommentEntry str) = entries.[0]
        Assert.AreEqual("This game is gonna be awesome! Watch this", str)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_comment_between_moves() =
        let entries = parse pMoveSeries "1. e4 {[%emt 0.0]} c5 {[%emt 0.0]} 2. Nc3"

        Assert.AreEqual(5, entries.Length)
        let (CommentEntry str) = entries.[1]
        Assert.AreEqual("[%emt 0.0]", str)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_NAGs() =
        let entries = parse pMoveSeries "1. e4 c5 $6 "

        Assert.AreEqual(3, entries.Length)
        let (NAGEntry cd) = entries.[2]
        Assert.AreEqual(6, cd)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_RAVs() =
        let entries = parse pMoveSeries "6. d5 $6 (6. Bd3 cxd4 7. exd4 d5 { - B14 }) 6... exd5"

        Assert.AreEqual(4, entries.Length)
        let (RAVEntry ml) = entries.[2]
        Assert.AreEqual(5, ml.Length)
        let (HalfMoveEntry (_, _, mv0)) = ml.[0] 
        let (HalfMoveEntry (_, _, mv1)) = ml.[2]
        let (CommentEntry str) = ml.[4]
        Assert.AreEqual(parse pMove "Bd3", mv0)
        Assert.AreEqual(parse pMove "exd4", mv1)
        Assert.AreEqual(" - B14 ", str)

    [<TestMethod>]
    member this.pMoveSeries_should_accept_nested_RAVs() =
        let entries = parse pMoveSeries "6. d5 (6. Bd3 cxd4 7. exd4 d5 (7... Qa4)) 6... exd5"

        Assert.AreEqual(3, entries.Length)
        let (RAVEntry ml) = entries.[1]
        Assert.AreEqual(5, ml.Length)

        let (RAVEntry ml2) = ml.[4]
        Assert.AreEqual(1, ml2.Length)
        let (HalfMoveEntry (_,_,mv)) = ml2.[0]
        Assert.AreEqual(parse pMove "Qa4", mv)
        