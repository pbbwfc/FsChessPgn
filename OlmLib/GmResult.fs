namespace Olm

module GmResult = 
    
    let Parse(s : string) = 
         if s="1-0" then GmResult.WhiteWins
         elif s="0-1" then GmResult.BlackWins
         elif s="1/2-1/2" then GmResult.Draw
         else GmResult.Open
    
    let ToStr(result:GmResult) =
        match result with
        |GmResult.WhiteWins -> "1-0" 
        |GmResult.BlackWins -> "0-1" 
        |GmResult.Draw -> "1/2-1/2" 
        |_ -> "*" 

    let ToUnicode(result:GmResult) =
        match result with
        |GmResult.WhiteWins -> "1-0" 
        |GmResult.BlackWins -> "0-1" 
        |GmResult.Draw -> "½-½" 
        |_ -> "*" 
