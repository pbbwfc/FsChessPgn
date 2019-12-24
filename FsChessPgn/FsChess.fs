namespace FsChess

module Board =

    ///Create a new Board given a FEN string
    let FromStr = FsChessPgn.Board.FromStr
    
    ///The starting Board at the beginning of a game
    let Start = FsChessPgn.Board.Start

    ///Gets all legal moves for this Board
    let AllMoves = FsChessPgn.MoveGenerate.AllMoves

    ///Make a SAN Move such as Nf3 for this Board and return the new Board
    let PushSAN = FsChessPgn.MoveUtil.ApplySAN

    ///Is the current position on the Board checkmate?
    let IsCheckMate = FsChessPgn.MoveGenerate.IsMate 

    ///Creates a PNG image file with the specified name, flipped if specified for the given Board 
    let ToPng = FsChessPgn.Png.BoardToPng
