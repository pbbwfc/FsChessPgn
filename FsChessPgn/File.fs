namespace FsChessPgn

open FsChess

module File = 
    
    let Parse(c : char):File = 
        let Filedesclookup = FILE_NAMES|>List.reduce(+)
        let idx = Filedesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid file")
        else idx
    
    let FileToString(file : File) = FILE_NAMES.[file]
    let IsInBounds(file : File) = file >= 0 && file <= 7
