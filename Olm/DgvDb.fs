namespace Olm

open System.Drawing
open System.Windows.Forms
open FsChess
open FsChess.Pgn

[<AutoOpen>]
module Library4 =
    
    type DgvDb() as gms =
        inherit DataGridView(Width = 800, Height = 250, 
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.Black, MultiSelect = false,
                RowHeadersVisible=false)

        let mutable fch = ""
        let mutable cp = new ChessPack()
        let mutable allgms:(int * Game) list = []
        let mutable indx = new System.Collections.Generic.Dictionary<Set<Square>,int list>()
        let mutable cbd = Board.Start
        let mutable filtgms:Hdr[] = [||]
        let mutable crw = -1
        let mutable cgm = GameEMP
        let mutable gmchg = false
        let mutable gmsui = new System.ComponentModel.BindingList<Hdr>()
        let bs = new BindingSource()

        //events
        let filtEvt = new Event<_>()
        let selEvt = new Event<_>()
        let pgnEvt = new Event<_>()

        //let igm2gmui (igmmv:(int * Game * string)) =
        //    let i,gm,mv = igmmv
        //    {
        //        Num = i+1
        //        White = gm.WhitePlayer
        //        W_Elo = gm.WhiteElo
        //        Black = gm.BlackPlayer
        //        B_Elo = gm.BlackElo
        //        Result = gm.Result|>Result.ToUnicode
        //        Date = gm|>GameDate.ToStr
        //        Event = gm.Event
        //        Round = gm.Round
        //        Site = gm.Site
        //    }
        
        let dosave() =
            if crw=0 then
                ((0,cgm)::allgms.Tail)|>List.map snd|>Games.WriteFile fch
            elif crw=allgms.Length-1 then
                (allgms.[..crw-1]@[crw,cgm])|>List.map snd|>Games.WriteFile fch
            else
                (allgms.[..crw-1]@[crw,cgm]@allgms.[crw+1..])|>List.map snd|>Games.WriteFile fch
        
        let doclick(e:DataGridViewCellEventArgs) =
            let rw = e.RowIndex
            //need to check if want to save
            //if gmchg then
            //    let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
            //    let dr = MessageBox.Show("Do you want to save the game: " + nm + " ?","Save Game",MessageBoxButtons.YesNoCancel)
            //    if dr=DialogResult.Yes then
            //        dosave()
            //        let ci,cg,_ = filtgms.[rw]
            //        crw <- ci
            //        cgm <- cg
            //        cgm|>selEvt.Trigger
            //    elif dr=DialogResult.No then
            //        let ci,cg,_ = filtgms.[rw]
            //        crw <- ci
            //        cgm <- cg
            //        cgm|>selEvt.Trigger
            //else
            //    let ci,cg,_ = filtgms.[rw]
            //    crw <- ci
            //    cgm <- cg
            //    cgm|>selEvt.Trigger
            gms.CurrentCell <- gms.Rows.[rw].Cells.[0]
        
        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs

        do 
            setup()
            gms.CellClick.Add(doclick)

        ///Sets the FCH file to be used
        member gms.SetFch(ifch:string) =
            fch <- ifch
            cp <- fch|>Pack.Load
            cbd <- Board.Start
            filtgms <- cp.Hdrs
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>Array.iter(fun hdr -> gmsui.Add(hdr))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            filtgms|>filtEvt.Trigger
            gmchg <- false
            if dispgms.Length>0 then
                let hdr = dispgms.[0]
                crw <- hdr.Num
                cgm <- Game.Set(cp,crw)
                cgm|>selEvt.Trigger
                cbd|>pgnEvt.Trigger

        ///Saves the PGN file
        member _.SavePgn() = dosave()

        ///Saves the PGN file with a new name
        member _.SaveAsPgn(ipgn:string) = 
            //copy index
            System.IO.File.Copy(fch + ".indx",ipgn + ".indx")
            fch <- ipgn
            dosave()

        ///Sets the Board to be filtered on
        member gms.SetBoard(ibd:Brd) =
            cbd <- ibd
            //filtgms <- allgms|>Games.FastFindBoard cbd indx
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            //dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            filtgms|>filtEvt.Trigger

        ///Changes the contents of the Game that is selected
        member _.ChangeGame(igm:Game) =
            cgm <- igm
            gmchg <- true

        ///Changes the header of the Game that is selected
        member _.ChangeGameHdr(igm:Game) =
            cgm <- igm
            gmchg <- true
            let rw = gms.SelectedCells.[0].RowIndex
            let chdr = gmsui.[rw]
            chdr.White<-cgm.WhitePlayer
            chdr.W_Elo<-cgm.WhiteElo
            chdr.Black<-cgm.BlackPlayer
            chdr.B_Elo<-cgm.BlackElo
            chdr.Result<-cgm.Result|>Result.ToUnicode
            chdr.Date<-cgm|>GameDate.ToStr
            chdr.Event<-cgm.Event
            chdr.Round<-cgm.Round
            chdr.Site<-cgm.Site
            gmsui.[rw] <- chdr

        ///Creates a new Game
        member _.NewGame() =
            //need to check if want to save
            if gmchg then
                let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
                let dr = MessageBox.Show("Do you want to save the game: " + nm + " ?","Save Game",MessageBoxButtons.YesNo)
                if dr=DialogResult.Yes then
                    dosave()
            cbd <- Board.Start
            cgm <- GameEMP
            crw <- allgms.Length
            allgms <- allgms@[crw,cgm]
            //filtgms <- allgms|>Games.FastFindBoard cbd indx
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            //dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            filtgms|>filtEvt.Trigger
            gmchg <- false
            cgm|>selEvt.Trigger

        ///Deletes selected Game
        member gms.DeleteGame() =
            let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
            let dr = MessageBox.Show("Do you really want to permanently delete the game: " + nm + " ?","Delete Game",MessageBoxButtons.YesNo)
            if dr=DialogResult.Yes then
                let orw = gms.SelectedCells.[0].RowIndex
                //save without gams
                if crw=0 then
                    (allgms.Tail)|>List.map snd|>Games.WriteFile fch
                elif crw=allgms.Length-1 then
                    (allgms.[..crw-1])|>List.map snd|>Games.WriteFile fch
                else
                    (allgms.[..crw-1]@allgms.[crw+1..])|>List.map snd|>Games.WriteFile fch
                //reload saves fch
                allgms <- fch|>Games.ReadIndexListFromFile
                fch|>Games.CreateIndex
                indx <- fch|>Games.GetIndex
                cbd <- Board.Start
                //filtgms <- allgms|>Games.FastFindBoard cbd indx
                gmsui.Clear()
                let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
                //dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
                //gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                //filtgms|>filtEvt.Trigger
                //gmchg <- false
                ////select row
                let rw = if orw=0 then 0 else orw-1
                //let ci,cg,_ = filtgms.[rw]
                //crw <- ci
                //cgm <- cg
                //cgm|>selEvt.Trigger
                gms.CurrentCell <- gms.Rows.[rw].Cells.[0]

        ///Export filtered games
        member gms.ExportFilter(filtfil:string) =
            filtgms
            //|>List.map(fun (_,gm,_) -> gm)
            //|>Games.WriteFile filtfil
        
        ///Provides the revised filtered list of Games
        member __.FiltChng = filtEvt.Publish
        
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish

        ///Provides the initial Board when the PGN file selected changes
        member __.PgnChng = pgnEvt.Publish
