namespace FsChessPgn.Data

module MoveTextEntry =

    let Parse(s : string) =
        //TODO:need to handle number and contin
        let mn =
            if System.Char.IsNumber(s.[0]) then
                let bits = s.Split([|'.'|])
                bits.[0]|>int|>Some
            else None
        
        let ic = s.Contains("...") 

        let mv =
            let bits = s.Split([|' '|])
            let mvtxt = bits.[bits.Length-1].Trim()
            pMove.Parse(mvtxt)

        HalfMoveEntry(mn,ic,mv,None)
    
