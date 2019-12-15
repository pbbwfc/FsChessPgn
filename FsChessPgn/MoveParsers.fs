[<AutoOpen>]
module FsChessPgn.PgnParsers.Move

open FParsec
open FsChessPgn.Data

type MoveInfo(piece, file, rank) =
    member val Piece = piece with get, set
    member val File  = file with get, set
    member val Rank  = rank with get, set

let getSquare(moveInfo : MoveInfo) =
    match moveInfo.File, moveInfo.Rank with
    | (x:File option, y:Rank option) when x.IsSome && y.IsSome -> (y.Value |> Rank.ToPosition x.Value)
    | _, _ -> OUTOFBOUNDS

let getMove(originInfo: MoveInfo option, targetInfo: MoveInfo, moveType: MoveType) =
    match originInfo, targetInfo with
    | None, _ -> pMove.Create (moveType,targetInfo.Piece,getSquare targetInfo,targetInfo.File,targetInfo.Piece)
    | Some(orig), _ -> pMove.CreateOrig (moveType,targetInfo.Piece,getSquare targetInfo,targetInfo.File,orig.Piece,getSquare orig,orig.File,orig.Rank)


//MOVE MECHANICS

// target square of a move
let pTarget =
    attempt(pPiece .>>. pFile .>>. pRank) // Qd5
    <|> (pFile .>>. pRank |>> fun (f, r) -> ((PieceType.Pawn, f),r)) //Pawn move, e.g. d5
    |>> fun ((piece, file), rank) ->  MoveInfo(Some(piece), Some(file), Some(rank))
    <!> "pTarget"

// origin squalre of move (usually for disambiguation)
let pOrigin =
    attempt(pPiece .>>. opt pFile .>>. opt pRank)
    <|> (pFile .>>. opt pRank |>> fun(f, r) -> ((PieceType.Pawn , Some f),r))
    |>> fun ((piece, file), rank) ->  MoveInfo(Some piece, file, rank)
    <!> "pOrigin"

let pBasicMove =
    attempt (pOrigin .>>. pTarget) |>> fun (origin, target) -> getMove(Some(origin), target, MoveType.Simple)
    <|> (pTarget |>> fun target -> getMove(None, target, MoveType.Simple))
    <!!> ("pBasicMove", 1)

// parsers for capturing
let pCapturingSign = (pchar 'x' <|> pchar ':') |>> fun x -> x.ToString()

let pInfixCaptureMove =   // e.g. QxBc5
    pOrigin .>> pCapturingSign .>>. pTarget
    |>> fun (orig, target) -> getMove(Some orig, target, MoveType.Capture)
    <!> "pInfixCaptureMove"

let pSimplifiedPawnCapture =  // e.g. dxe or de
    pFile .>> pCapturingSign .>>. pFile
    >>= fun(f1, f2) ->
        match f1 = f2 with //do not allow a6xa7
        | true -> pzero
        | false -> preturn (f1, f2)
    |>> fun (file1, file2) -> pMove.CreateOrig(MoveType.Capture,None,OUTOFBOUNDS,Some(file2),Some(PieceType.Pawn),OUTOFBOUNDS,Some(file1),None)
    <!> "pSimplifiedPawnCapture"

let pSuffixCaptureMove = // e.g. Qf4d4x or Qf4:
    pBasicMove .>> pCapturingSign
    |>> fun move -> {move with Mtype = MoveType.Capture}
    <!> "pSuffixCaptureMove"

let pBasicCapturingMove =
    attempt (attempt pInfixCaptureMove <|> pSuffixCaptureMove)
    <|> pSimplifiedPawnCapture
    <!> "pBasicCapturingMove"


// the two most common move types: move and capture
let pCapturingMove =
    pBasicCapturingMove .>>. opt (strCI "e.p." ) //TODO: e.p. should only be allowed for pawns
    |>> fun (move, enpassant) ->
            match enpassant with
            | None -> move
            | _ -> {move with Mtype=MoveType.CaptureEnPassant}
    <!!> ("pCapturingMove", 1)


// special moves: pawn promotion and castle (king-side, queen-side)
// TODO: this parser allows to much, e.g. Qxd5(R).
//       It should be asserted, that the moved piece is a pawn.
//       If rank is set, then only rank 8 is allowed
let pPawnPromotion =
    (attempt  pBasicCapturingMove <|> pBasicMove)
    .>>. ((str "=" >>. pPiece) <|> (str "(" >>. pPiece .>> str ")"))
    |>> fun (move, piece) -> {move with PromotedPiece = Some(piece)}
    <!> "pPawnPromotion"

let pCasteKingSide =
    str "O-O" <|> str "O - O" <|> str "0-0"  <|> str "0 - 0"
    |>> fun _ -> pMove.CreateCastle(MoveType.CastleKingSide)
    <!> "pCastleKingSide"

let pCasteQueenSide =
    str "O-O-O" <|> str "O - O - O" <|> str "0-0-0"  <|> str "0 - 0 - 0"
    |>> fun _ -> pMove.CreateCastle(MoveType.CastleQueenSide)
    <!> "pCasteQueenSide"

let pCastle = pCasteQueenSide <|> pCasteKingSide

// indicators
let pCheckIndicator = str "++" <|> str "††" <|> str "dbl ch" <|> str "+" <|> str "†" <|> str "ch" <!> "pCheckIndicator"
let pCheckMateIndicator = str "#" <|> str "‡" <!> "pCheckMateIndicator"
let pIndicator = pCheckIndicator <|> pCheckMateIndicator
let pAnnotation =
    str "????" <|> str "???"
    <|> str "!!!!" <|> str "!!!" <|> str "?!?" <|> str "!?!" <|> str "??" <|> str "?!"
    <|> str "!!" <|> str "!?" <|> str "?" <|> str "!"
    <|> str "=/∞" <|> str "=/+" <|> str "="
    <|> str "+/=" <|> str "+/-" <|> str "+-" <|> str "-/+" <|> str "-+"
    <|> str "∞" <|> str "○" <|> str "↑↑" <|> str "↑" <|> str "⇄" <|> str "∇" <|> str "Δ"
    <|> str "TN" <|> str "N"
    |>> fun annotation ->
            match annotation with
            | "!!!" | "!!!!" -> MoveAnnotation.MindBlowing
            | "!!" -> MoveAnnotation.Brilliant
            | "!" -> MoveAnnotation.Good
            | "!?" -> MoveAnnotation.Interesting
            | "?!" -> MoveAnnotation.Dubious
            | "?" -> MoveAnnotation.Mistake
            | "??" -> MoveAnnotation.Blunder
            | "???" | "????" -> MoveAnnotation.Abysmal
            | "!?!" | "?!?" -> MoveAnnotation.FascinatingButUnsound
            | "∞" -> MoveAnnotation.Unclear
            | "=/∞" -> MoveAnnotation.WithCompensation
            | "=" -> MoveAnnotation.EvenPosition
            | "+/=" -> MoveAnnotation.SlightAdvantageWhite
            | "=/+" -> MoveAnnotation.SlightAdvantageBlack
            | "+/-" -> MoveAnnotation.AdvantageWhite
            | "-/+" -> MoveAnnotation.AdvantageBlack
            | "+-" -> MoveAnnotation.DecisiveAdvantageWhite
            | "-+" -> MoveAnnotation.DecisiveAdvantageBlack
            | "○" -> MoveAnnotation.Space
            | "↑" -> MoveAnnotation.Initiative
            | "↑↑" -> MoveAnnotation.Development
            | "⇄" -> MoveAnnotation.Counterplay
            | "∇" -> MoveAnnotation.Countering
            | "Δ" -> MoveAnnotation.Idea
            | "TN" | "N" -> MoveAnnotation.TheoreticalNovelty
            | _ -> MoveAnnotation.UnknownAnnotation
    <!> "pAnnotation"
    <?> "Move annotation (e.g. ! or ??)"

let pAdditionalInfo =
    (attempt(pIndicator .>>. pAnnotation) |>> fun (i, a) -> Some(i), Some(a))
    <|> (attempt(pAnnotation) |>> fun (a) -> None, Some(a))
    <|> (pIndicator|>> fun (i) -> Some(i), None)

let pMove =
    attempt pPawnPromotion <|>
    attempt pCapturingMove <|>
    attempt pBasicMove <|>
    pCastle
    .>>. opt(pAdditionalInfo)
    |>> fun (move, addInfo) ->
            let indicator, annotation =
                match addInfo with
                | None -> None, None
                | Some(x) -> x
            let nmove = {move with Annotation=annotation}
            match indicator with
            | None -> nmove
            | Some(i) ->
                match i with
                | "+"  | "†"  | "ch" -> {nmove with IsCheck=true}
                | "++" | "††" | "dbl ch" -> {nmove with IsCheck=true;IsDoubleCheck=true}
                | "#"  | "‡" -> {nmove with IsCheckMate=true}
                | _ -> move


    <!!> ("pMove", 2)
    <?> "Move (e.g. Qc4 or e2e4 or 0-0-0 etc.)"

let appyPMove (p: string)= run pMove p
