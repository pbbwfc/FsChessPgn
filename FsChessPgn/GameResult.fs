namespace FsChessPgn

module GameResult = 
    
    let Parse(s : string) = 
         if s="1-0" then GameResult.WhiteWins
         elif s="0-1" then GameResult.BlackWins
         elif s="1/2-1/2" then GameResult.Draw
         else GameResult.Open
