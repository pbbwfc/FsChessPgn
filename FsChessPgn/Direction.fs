namespace FsChessPgn.Data

module Direction =

    let AllDirectionsKnight = 
        [| Direction.DirNNE; Direction.DirEEN; Direction.DirEES; Direction.DirSSE; Direction.DirSSW; Direction.DirWWS; 
           Direction.DirWWN; Direction.DirNNW |]
    let AllDirectionsRook = [| Direction.DirN; Direction.DirE; Direction.DirS; Direction.DirW |]
    let AllDirectionsBishop = [| Direction.DirNE; Direction.DirSE; Direction.DirSW; Direction.DirNW |]
    let AllDirectionsQueen = 
        [| Direction.DirN; Direction.DirE; Direction.DirS; Direction.DirW; Direction.DirNE; Direction.DirSE; 
           Direction.DirSW; Direction.DirNW |]
    let AllDirections = 
        [| Direction.DirN; Direction.DirE; Direction.DirS; Direction.DirW; Direction.DirNE; Direction.DirSE; 
           Direction.DirSW; Direction.DirNW; Direction.DirNNE; Direction.DirEEN; Direction.DirEES; Direction.DirSSE; 
           Direction.DirSSW; Direction.DirWWS; Direction.DirWWN; Direction.DirNNW |]

    let IsDirectionRook(dir : Direction) = 
        match dir with
        | Direction.DirN | Direction.DirE | Direction.DirS | Direction.DirW -> true
        | _ -> false

    let IsDirectionBishop(dir : Direction) = 
        match dir with
        | Direction.DirNW | Direction.DirNE | Direction.DirSW | Direction.DirSE -> true
        | _ -> false

    let IsDirectionKnight(dir : Direction) = 
        match dir with
        | Direction.DirNNE | Direction.DirEEN | Direction.DirEES | Direction.DirSSE 
        | Direction.DirSSW | Direction.DirWWS | Direction.DirWWN | Direction.DirNNW -> 
            true
        | _ -> false

    let Opposite(dir : Direction) = -int (dir) |> Dirn
