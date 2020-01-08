namespace FsChessPgn

open FsChess

module NagUtil = 
    
    let ToStr(nag:NAG) =
         match nag with
         | NAG.Good -> "!"
         | NAG.Poor -> "?"
         | NAG.VeryGood -> "!!"
         | NAG.VeryPoor -> "??"
         | NAG.Speculative -> "!?"
         | NAG.Questionable -> "?!"
         |_ -> ""

