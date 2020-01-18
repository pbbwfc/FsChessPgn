namespace FsChessPgn

open FsChess

module NagUtil = 
    
    let All = [0..6]|>List.map Ng
    let ToStr(nag:NAG) =
         match nag with
         | NAG.Good -> "!"
         | NAG.Poor -> "?"
         | NAG.VeryGood -> "!!"
         | NAG.VeryPoor -> "??"
         | NAG.Speculative -> "!?"
         | NAG.Questionable -> "?!"
         |_ -> ""

    let Desc(nag:NAG) =
         match nag with
         | NAG.Good -> "Good"
         | NAG.Poor -> "Poor"
         | NAG.VeryGood -> "Very Good"
         | NAG.VeryPoor -> "Very Poor"
         | NAG.Speculative -> "Speculative"
         | NAG.Questionable -> "Questionable"
         |_ -> "None"
