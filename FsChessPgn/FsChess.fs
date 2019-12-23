namespace FsChess

module Board =

    ///The starting Board at the beginning of a game
    let Start = FsChessPgn.Data.Board.Start

    ///Gets all legal moves for this Board(bd)
    let AllMoves = FsChessPgn.Data.MoveGenerate.AllMoves

    ///Make a SAN Move(move) such as Nf3 for this Board(bd) and return the new Board
    let PushSAN = FsChessPgn.Data.MoveUtil.ApplySAN

    ///Is the current position on the Board(bd) checkmate?
    let IsCheckMate = FsChessPgn.Data.MoveGenerate.IsMate 
