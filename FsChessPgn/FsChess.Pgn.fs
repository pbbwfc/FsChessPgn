namespace FsChess.Pgn

module Games =

    ///Get a list of Games from a file
    let ReadListFromFile = FsChessPgn.Games.ReadFromFile
    
    ///Get a list of index * Game from a file
    let ReadIndexListFromFile = FsChessPgn.Games.ReadIndexListFromFile

    ///Get a Sequence of Games from a file
    let ReadSeqFromFile = FsChessPgn.Games.ReadSeqFromFile

    ///Write a list of Games to a file
    let WriteFile = FsChessPgn.PgnWriter.WriteFile

    ///Finds the Games that containing the specified Board
    let FindBoard = FsChessPgn.Games.FindBoard
    
    ///Creates index on PGN for fast searches
    let CreateIndex = FsChessPgn.Games.CreateIndex

    ///Loads index for use in fast searches
    let GetIndex = FsChessPgn.Games.GetIndex

    //Does a fast search using the Index and the index list of Games
    let FastFindBoard = FsChessPgn.Games.FastFindBoard


module Stats =

    ///Get Statistics for the Board
    let Get = FsChessPgn.Stats.Get
