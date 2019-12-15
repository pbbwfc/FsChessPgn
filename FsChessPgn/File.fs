namespace FsChessPgn.Data

module File = 
    
    let Parse(c : char):File = 
        let Filedesclookup = FILE_NAMES|>Array.reduce(+)
        let idx = Filedesclookup.IndexOf(c.ToString().ToLower())
        if idx < 0 then failwith (c.ToString() + " is not a valid file")
        else idx
    
    let FileToString(file : File) = FILE_NAMES.[int(file)]
    let IsInBounds(file : File) = int (file) >= 0 && int (file) <= 7
