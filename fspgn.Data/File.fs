namespace fspgn.Data

open System

module File = 
    let Filedesclookup = "abcdefgh"
    let AllFiles = [| File.FileA; File.FileB; File.FileC; File.FileD; File.FileE; File.FileF; File.FileG; File.FileH |]
    
    let Parse(c : char) = 
        let idx = Filedesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid file")
        else idx |> Fl
    
    let FileToString(file : File) = Filedesclookup.Substring(int (file), 1)
    let IsInBounds(file : File) = int (file) >= 0 && int (file) <= 7
    let ToPosition (rank : Rank) (file : File) = (int (rank) * 8 + int (file)) |> Pos
    
    let ToBitboard(file : File) = FileBits.[int(file)]
