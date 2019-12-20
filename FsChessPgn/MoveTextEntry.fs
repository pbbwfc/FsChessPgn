namespace FsChessPgn.Data

module MoveTextEntry =

    let Parse(s : string) =
        //TODO:need to handle number and contin
        HalfMoveEntry(None,false,pMove.Parse(s),None)
    
