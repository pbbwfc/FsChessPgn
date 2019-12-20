#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type MoveSeriesRegParserTest() =
    [<TestMethod>]
    member this.parse_simple_piece_move() =
        let s = "Qf5"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(None,mn)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Queen, mv.Piece.Value)
        Assert.AreEqual(F5, mv.TargetSquare)
        Assert.AreEqual(s,mv|>PgnWrite.MoveStr)

    [<TestMethod>]
    member this.parse_simple_white_move() =
        let s = "12. Qf5"
        let gml = RegParse.ReadFromString s
        Assert.AreEqual(1, gml.Length)
        let gm = gml.Head
        let mt = gm.MoveText
        Assert.AreEqual(1, mt.Length)
        let (HalfMoveEntry (mn,ic,mv,_)) = mt.Head
        Assert.AreEqual(12,mn.Value)
        Assert.AreEqual(false,ic)
        Assert.AreEqual(MoveType.Simple,mv.Mtype)
        Assert.AreEqual(PieceType.Queen, mv.Piece.Value)
        Assert.AreEqual(F5, mv.TargetSquare)
        Assert.AreEqual(s,mv|>PgnWrite.MoveStr)



    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_a_moveSeries() =
    //    let moveSeries = parse pMoveSeries "1. e4 c5 2. Nf3 d6 3. Bb5+ Bd7"

    //    Assert.AreEqual(6, moveSeries.Length)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_a_moveSeries_with_split_moves() =
    //    let moveSeries = parse pMoveSeries "1. e4 c5 2. Nf3 \n 2... d6 3. Bb5+ Bd7"

    //    Assert.AreEqual(6, moveSeries.Length)

    //[<TestMethod>]
    //member this.pMoveEntry_should_accept_comments_in_braces() =
    //    let (CommentEntry str) = (parse pMoveSeriesEntry "{this is a comment}") 

    //    Assert.AreEqual("this is a comment", str)

    //[<TestMethod>]
    //member this.pMoveEntry_should_accept_comments_semicolon_comment() =
    //    let moveSeries = parse pMoveSeries "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
    //    3... a6"

    //    Assert.AreEqual(7, moveSeries.Length)

    //    let (CommentEntry str) = (moveSeries.Item(5)) 
    //    Assert.AreEqual("This opening is called the Ruy Lopez.", str)

    //[<TestMethod>]
    //member this.pMoveEntry_should_accept_multiple_line_comments() =
    //    let moveSeries = parse pMoveSeries "1. e4 e5 2. Nf3 Nc6 3. Bb5 ;This opening is called the Ruy Lopez.
    //    ;Another comment
    //    3... a6"

    //    Assert.AreEqual(8, moveSeries.Length)

    //    let (CommentEntry str1) = (moveSeries.Item(5))
    //    let (CommentEntry str2) = (moveSeries.Item(6))
    //    Assert.AreEqual("This opening is called the Ruy Lopez.", str1)
    //    Assert.AreEqual("Another comment", str2)


    //[<TestMethod>]
    //member this.pEndOfGame_should_accept_Draw() =
    //    let (GameEndEntry gr) = (parse pEndOfGame "1/2 - 1/2")
    //    Assert.AreEqual(GameResult.Draw, gr)

    //[<TestMethod>]
    //member this.pEndOfGame_should_accept_utf8_draw() =
    //    let (GameEndEntry gr) = (parse pEndOfGame "½-½")
    //    Assert.AreEqual(GameResult.Draw, gr)

    //[<TestMethod>]
    //member this.pEndOfGame_should_accept_WhiteWin() =
    //    let (GameEndEntry gr) = (parse pEndOfGame "1-0")
    //    Assert.AreEqual(GameResult.WhiteWins, gr)

    //[<TestMethod>]
    //member this.pEndOfGame_should_accept_BlackWin() =
    //    let (GameEndEntry gr) = (parse pEndOfGame "0-1")
    //    Assert.AreEqual(GameResult.BlackWins, gr)

    //[<TestMethod>]
    //member this.pEndOfGame_should_accept_EndOpen() =
    //    let (GameEndEntry gr) = (parse pEndOfGame "*")
    //    Assert.AreEqual(GameResult.Open, gr)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_comment_after_end_game() =
    //    let entries = parse pMoveSeries "1-0 {impressive game!}"

    //    let (CommentEntry str) = (entries.Item(1))
    //    Assert.AreEqual("impressive game!", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_comment_before_moves() =
    //    let entries = parse pMoveSeries "{This game is gonna be awesome! Watch this} \n 1. e4 e5 2. Nf3"
    //    let (CommentEntry str) = entries.[0]
    //    Assert.AreEqual("This game is gonna be awesome! Watch this", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_comment_between_moves() =
    //    let entries = parse pMoveSeries "1. e4 {[%emt 0.0]} c5 {[%emt 0.0]} 2. Nc3"

    //    Assert.AreEqual(5, entries.Length)
    //    let (CommentEntry str) = entries.[1]
    //    Assert.AreEqual("[%emt 0.0]", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_NAGs() =
    //    let entries = parse pMoveSeries "1. e4 c5 $6 "

    //    Assert.AreEqual(3, entries.Length)
    //    let (NAGEntry cd) = entries.[2]
    //    Assert.AreEqual(6, cd)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_RAVs() =
    //    let entries = parse pMoveSeries "6. d5 $6 (6. Bd3 cxd4 7. exd4 d5 { - B14 }) 6... exd5"

    //    Assert.AreEqual(4, entries.Length)
    //    let (RAVEntry ml) = entries.[2]
    //    Assert.AreEqual(5, ml.Length)
    //    let (HalfMoveEntry (_, _, mv0,None)) = ml.[0] 
    //    let (HalfMoveEntry (_, _, mv1,None)) = ml.[2]
    //    let (CommentEntry str) = ml.[4]
    //    Assert.AreEqual(parse pMove "Bd3", mv0)
    //    Assert.AreEqual(parse pMove "exd4", mv1)
    //    Assert.AreEqual(" - B14 ", str)

    //[<TestMethod>]
    //member this.pMoveSeries_should_accept_nested_RAVs() =
    //    let entries = parse pMoveSeries "6. d5 (6. Bd3 cxd4 7. exd4 d5 (7... Qa4)) 6... exd5"

    //    Assert.AreEqual(3, entries.Length)
    //    let (RAVEntry ml) = entries.[1]
    //    Assert.AreEqual(5, ml.Length)

    //    let (RAVEntry ml2) = ml.[4]
    //    Assert.AreEqual(1, ml2.Length)
    //    let (HalfMoveEntry (_,_,mv,_)) = ml2.[0]
    //    Assert.AreEqual(parse pMove "Qa4", mv)
        