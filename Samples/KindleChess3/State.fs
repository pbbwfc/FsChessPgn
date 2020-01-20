namespace KindleChess

open FsChess

type SharedState() =
    let mutable pos = Board.Start
    let mutable visw = true
    let mutable curb = Book.emp
    let mutable chi = -1
    //Events
    let pchngEvt = new Event<_>()
    let orntEvt = new Event<_>()
    let cchngEvt = new Event<_>()
    let chaddEvt = new Event<_>()
    let chrnmEvt = new Event<_>()
    let chchgEvt = new Event<_>()
    //publish
    member __.PosChng = pchngEvt.Publish
    member __.Ornt = orntEvt.Publish
    member __.CurChng = cchngEvt.Publish
    member __.ChAdd = chaddEvt.Publish
    member __.ChRnm = chrnmEvt.Publish
    member __.ChChg = chchgEvt.Publish
    
    //Members
    member __.Pos
        with get () = pos
        and set (value) =
            pos <- value
            pos |> pchngEvt.Trigger
    
    member __.Chi
        with get () = chi
        and set (value) =
            chi <- value
            let nm = curb.Chapters.[chi]
            let ch = curb|>Book.getChap chi
            (nm,ch) |> chchgEvt.Trigger
    
    member __.CurBook = curb
    
    member x.NewBook(nm, isw) =
        x.Pos <- Board.Start
        curb <- Book.cur (nm, isw)
        curb |> cchngEvt.Trigger
        visw <- isw
        visw |> orntEvt.Trigger
    
    member __.UpdateBook(cr, dt, dc, wel) =
        curb <- curb |> Book.upd (cr, dt, dc, wel)
    
    member __.AddChapter(nm) =
        if curb=Book.emp then
            System.Windows.Forms.MessageBox.Show("Please open book before adding a chapter!")|>ignore
        else
            let cur,ch = curb |> Book.addChap nm
            curb <- cur
            chi <- curb.Chapters.Length - 1
            (nm,ch) |> chaddEvt.Trigger

    member __.MoveUpChapter() =
        if chi<curb.Chapters.Length - 1 then
            let cur = curb |> Book.mvuChap chi
            curb <- cur
            chi <- chi + 1

    member __.MoveDownChapter() =
        if chi>0 then
            let cur = curb |> Book.mvdChap chi
            curb <- cur
            chi <- chi - 1
    
    member __.Books(isw) =
        if isw then Book.wbks()
        else Book.bbks()
    
    member x.OpenBook(nm, isw) =
        x.Pos <- Board.Start
        visw <- isw
        curb <- Book.load (nm, visw)
        curb |> cchngEvt.Trigger
        visw |> orntEvt.Trigger
        if curb.Chapters.Length > 0 then 
            chi <- 0
            let nm = curb.Chapters.[chi]
            let ch = curb|>Book.getChap chi
            (nm,ch) |> chchgEvt.Trigger
    
    member __.SaveBook(cho:Game option) = 
        Book.save (curb)|>ignore
        if cho.IsSome then cho.Value|>Book.saveChap chi curb 
    
    member __.SaveAsBook(nm) =
        curb <- Book.saveas (curb, nm)
        curb |> cchngEvt.Trigger
    
    member __.DelBook(nm, isw) = Book.delete (nm, isw)
    
    member __.ChOpen(i:int) =
        chi <- i
        let ch = curb|>Book.getChap chi
        let nm = curb.Chapters.[chi]
        (nm,ch) |> chchgEvt.Trigger

    member __.ChRename(nm) =
        curb <- curb |> Book.rnmChap chi nm
        nm |> chrnmEvt.Trigger
    
    member x.ChDelete(nm) =
        curb <- curb |> Book.delChap nm
        x.Pos <- Board.Start
        chi <- -1
        curb |> cchngEvt.Trigger
        if curb.Chapters.Length > 0 then 
            chi <- 0
            let nm = curb.Chapters.[chi]
            let ch = curb|>Book.getChap chi
            (nm,ch) |> chchgEvt.Trigger
   
    member __.GenHTML() = Book.genh (curb)
 
module State =
    let stt = new SharedState()
