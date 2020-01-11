namespace KindleChess

open FsChess

type SharedState() =
    let mutable pos = Board.Start
    let mutable mvs = []
    let mutable visw = true
    let mutable curb = Book.emp
    let mutable chi = -1
    let mutable tel = [] //table entries for selected branch
    let mutable mvl = [] //moves for selected branch
    let mutable mct = -1 //move num selected in selected branch
    let mutable issub = false //IsSub for selected branch
    //Events
    let pchngEvt = new Event<_>()
    let mchngEvt = new Event<_>()
    let orntEvt = new Event<_>()
    let cchngEvt = new Event<_>()
    let chaddEvt = new Event<_>()
    let chrnmEvt = new Event<_>()
    let chchgEvt = new Event<_>()
    let chmvextEvt = new Event<_>()
    let chmvsplEvt = new Event<_>()
    let mvlchgEvt = new Event<_>()
    let mctchgEvt = new Event<_>()
    let cmplxchgEvt = new Event<_>()
    let nagchgEvt = new Event<_>()
    //publish
    member __.PosChng = pchngEvt.Publish
    member __.MvsChng = mchngEvt.Publish
    member __.Ornt = orntEvt.Publish
    member __.CurChng = cchngEvt.Publish
    member __.ChAdd = chaddEvt.Publish
    member __.ChRnm = chrnmEvt.Publish
    member __.ChChg = chchgEvt.Publish
    member __.ChMvExt = chmvextEvt.Publish
    member __.ChMvSpl = chmvsplEvt.Publish
    member __.MvlChg = mvlchgEvt.Publish
    member __.MctChg = mctchgEvt.Publish
    member __.CmplxChg = cmplxchgEvt.Publish
    member __.NagChg = nagchgEvt.Publish
    
    //Members
    member __.Pos
        with get () = pos
        and set (value) =
            pos <- value
            pos |> pchngEvt.Trigger
    
    member __.Mvs
        with get () = mvs
        and set (value) =
            mvs <- value
            mvs |> mchngEvt.Trigger
    
    member __.Chi
        with get () = chi
        and set (value) =
            chi <- value
            let nm = curb.Chapters.[chi]
            let ch = curb|>Book.getChap chi
            (nm,ch) |> chchgEvt.Trigger
    
    member __.Tel
        with get () = tel
        and set (value) = tel <- value
    
    member __.Mvl
        with get () = mvl
        and set (value) = mvl <- value
    
    member __.Mct
        with get () = mct
        and set (value) = mct <- value
    
    member __.IsSub
        with get () = issub
        and set (value) = issub <- value
    
    //member x.NextMv() =
    //    if mct < mvl.Length then 
    //        x.Pos |> Pos.DoMv mvl.[mct]
    //        x.Mvs <- x.Mvs @ [ mvl.[mct] ]
    //        mct <- mct + 1
    //        pos |> pchngEvt.Trigger
    //        chi |> mctchgEvt.Trigger
    
    //member x.EndMv() =
    //    let rec domvs() =
    //        if mct < mvl.Length then 
    //            x.Pos |> Pos.DoMv mvl.[mct]
    //            x.Mvs <- x.Mvs @ [ mvl.[mct] ]
    //            mct <- mct + 1
    //            domvs()
    //    if mct < mvl.Length then 
    //        domvs()
    //        pos |> pchngEvt.Trigger
    //        chi |> mctchgEvt.Trigger
    
    //member x.PrevMv() =
    //    if mct > 0 then 
    //        mct <- mct - 1
    //        x.Mvs <- x.Mvs.[0..x.Mvs.Length - 2]
    //        x.Pos <- Pos.FromMoves x.Mvs
    //        chi |> mctchgEvt.Trigger
    
    //member x.HomeMv() =
    //    if mct > 0 then 
    //        x.Mvs <- x.Mvs.[0..x.Mvs.Length - mct - 1]
    //        mct <- 0
    //        x.Pos <- Pos.FromMoves x.Mvs
    //        chi |> mctchgEvt.Trigger
    
    member __.CurBook = curb
    
    member x.NewBook(nm, isw) =
        x.Pos <- Board.Start
        x.Mvs <- []
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
            tel <- []
            mvl <- []
            mct <- 0
            issub <- false

    member __.MoveUpChapter() =
        if chi<curb.Chapters.Length - 1 then
            let cur = curb |> Book.mvuChap chi
            curb <- cur
            chi <- chi + 1
            tel <- []
            mvl <- []
            mct <- 0
            issub <- false

    member __.MoveDownChapter() =
        if chi>0 then
            let cur = curb |> Book.mvdChap chi
            curb <- cur
            chi <- chi - 1
            tel <- []
            mvl <- []
            mct <- 0
            issub <- false
    
 
    //member __.EditGameDet(nm,intro) =
    //    curb <- curb |> Book.editGmDt chi nm intro
    //    (chi, nm) |> chrnmEvt.Trigger

    member __.Books(isw) =
        if isw then Book.wbks()
        else Book.bbks()
    
    member x.OpenBook(nm, isw) =
        x.Pos <- Board.Start
        x.Mvs <- []
        visw <- isw
        curb <- Book.load (nm, visw)
        curb |> cchngEvt.Trigger
        visw |> orntEvt.Trigger
        if curb.Chapters.Length > 0 then 
            //vid <- curb.Chapters.[chi].Lines.Vid
            //issub <- curb.Chapters.[chi].Lines.IsSub
            //tel <- curb.Chapters.[chi].Lines.Mvs
            //mvl <- tel |> List.map (fun te -> te.Mv)
            chi <- 0
            mct <- 0
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
        x.Mvs <- []
        chi <- -1
        curb |> cchngEvt.Trigger
        if curb.Chapters.Length > 0 then 
            chi <- 0
            mct <- 0
            let nm = curb.Chapters.[chi]
            let ch = curb|>Book.getChap chi
            (nm,ch) |> chchgEvt.Trigger
   
    //member __.GetDsc() = curb |> Book.getdsc vid mct chi
    //member __.UpdDsc(pm) = curb <- curb |> Book.upddsc vid mct chi pm
    //member __.GenHTML() = Book.genh (curb)
    
    //member __.DelLine() =
    //    curb <- curb |> Book.delLine chi vid
    //    vid <- curb.Chapters.[chi].Lines.Vid
    //    tel <- []
    //    mvl <- []
    //    mct <- 0
    //    issub <- false
    //    pos <- Pos.Start()
    //    mvs <- []
    //    pos |> pchngEvt.Trigger
    //    (chi,curb.Chapters.[chi]) |> cmplxchgEvt.Trigger
    
    //member __.SetSub() =
    //    if not issub then 
    //        curb <- curb |> Book.setsub vid chi
    //        (chi,curb.Chapters.[chi]) |> cmplxchgEvt.Trigger
    
    //member __.SetMain() =
    //    if issub then 
    //        curb <- curb |> Book.setmain vid chi
    //        (chi,curb.Chapters.[chi]) |> cmplxchgEvt.Trigger
    
    //member __.MoveUp() =
    //    curb <- curb |> Book.moveup vid chi
    //    (chi,curb.Chapters.[chi]) |> cmplxchgEvt.Trigger
    
    //member __.MoveDown() =
    //    curb <- curb |> Book.movedown vid chi
    //    (chi,curb.Chapters.[chi]) |> cmplxchgEvt.Trigger
    
    //member __.SetNag(nag) =
    //    let ncurb, ntel = curb |> Book.setnag nag mct vid chi
    //    curb <- ncurb
    //    tel <- ntel
    //    (chi, tel) |> nagchgEvt.Trigger

module State =
    let stt = new SharedState()
