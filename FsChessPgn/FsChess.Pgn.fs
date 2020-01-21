namespace FsChess.Pgn

module Games =

    ///Get a list of Games from a file
    let ReadListFromFile = FsChessPgn.Games.ReadFromFile
    
    ///Get a Sequence of Games from a file
    let ReadSeqFromFile = FsChessPgn.Games.ReadSeqFromFile

    ///Write a list of Games to a file
    let WriteFile = FsChessPgn.PgnWriter.WriteFile

    ///Finds the Games that containg the specified Board
    let FindBoard = FsChessPgn.Games.FindBoard

    let CreateIndex = FsChessPgn.Games.CreateIndex