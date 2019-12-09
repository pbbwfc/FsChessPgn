namespace fspgn.Data

module Player = 
    let AllPlayers = [| Player.White; Player.Black |]
    let PlayerOther(player : Player) = (int (player) ^^^ 1) |> Plyr
    
    let MyNorth(player : Player) = 
        if player = Player.White then Direction.DirN
        else Direction.DirS
    
    let MyRanks = 
        [| [| Rank.Rank8; Rank.Rank7; Rank.Rank6; Rank.Rank5; Rank.Rank4; Rank.Rank3; Rank.Rank2; Rank.Rank1 |]
           [| Rank.Rank1; Rank.Rank2; Rank.Rank3; Rank.Rank4; Rank.Rank5; Rank.Rank6; Rank.Rank7; Rank.Rank8 |] |]
    
    let MyRank (rank : Rank) (player : Player) = MyRanks.[int (player)].[int (rank)]
