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
        let mutable cbd = Board.Start
        let mutable filtgms:(Hdr*Move)[] = [||]
        let mutable crw = -1
        let mutable chdr = new Hdr()
        let mutable cmvstr = ""
        let mutable cmvs = [||]
        let mutable cgm = GameEMP
        let mutable gmchg = false
        let mutable gmsui = new System.ComponentModel.BindingList<Hdr>()
        let bs = new BindingSource()

        //events
        let filtEvt = new Event<_>()
        let selEvt = new Event<_>()
        let pgnEvt = new Event<_>()

        let dosave() =
            //TODO need to update index
            cp.Hdrs.[crw] <- chdr
            cp.MvsStrs.[crw] <- cmvstr
            cp.Mvss.[crw] <- cmvs
            fch|>cp.Save
        
        let doclick(e:DataGridViewCellEventArgs) =
            let rw = e.RowIndex
            //need to check if want to save
            if gmchg then
                let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
                let dr = MessageBox.Show("Do you want to save the game: " + nm + " ?","Save Game",MessageBoxButtons.YesNoCancel)
                if dr=DialogResult.Yes then
                    dosave()
                    let hdr,mv = filtgms.[rw]
                    crw <- hdr.Num
                    chdr <- hdr
                    cmvstr <- cp.MvsStrs.[crw]
                    cmvs <- cp.Mvss.[crw]
                    cgm <- Game.Set(cp,crw)
                    cgm|>selEvt.Trigger
                elif dr=DialogResult.No then
                    let hdr,mv = filtgms.[rw]
                    crw <- hdr.Num
                    chdr <- hdr
                    cmvstr <- cp.MvsStrs.[crw]
                    cmvs <- cp.Mvss.[crw]
                    cgm <- Game.Set(cp,crw)
                    cgm|>selEvt.Trigger
            else
                let hdr,mv = filtgms.[rw]
                crw <- hdr.Num
                chdr <- hdr
                cmvstr <- cp.MvsStrs.[crw]
                cmvs <- cp.Mvss.[crw]
                cgm <- Game.Set(cp,crw)
                cgm|>selEvt.Trigger
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
            filtgms <- cp.Hdrs|>Array.mapi(fun i h -> (h,if cp.Mvss.[i].Length=0 then FsChess.Types.MoveEmpty else cp.Mvss.[i].[0]))
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>Array.iter(fun (hdr,mv) -> gmsui.Add(hdr))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            (filtgms,cbd,cp)|>filtEvt.Trigger
            gmchg <- false
            if dispgms.Length>0 then
                let hdr,mv = dispgms.[0]
                crw <- hdr.Num
                chdr <- hdr
                cmvstr <- cp.MvsStrs.[crw]
                cmvs <- cp.Mvss.[crw]
                cgm <- Game.Set(cp,crw)
                cgm|>selEvt.Trigger
                cbd|>pgnEvt.Trigger

        ///Saves the PGN file
        member _.SaveFch() = dosave()

        ///Saves the PGN file with a new name
        member _.SaveAsPgn(ipgn:string) = 
            //copy index
            System.IO.File.Copy(fch + ".indx",ipgn + ".indx")
            fch <- ipgn
            dosave()

        ///Sets the Board to be filtered on
        member gms.SetBoard(ibd:Brd) =
            cbd <- ibd
            filtgms <- Find.Board cbd cp
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>Array.iter(fun (hdr,mv) -> gmsui.Add(hdr))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            (filtgms,cbd,cp)|>filtEvt.Trigger

        ///Changes the contents of the Game that is selected
        member _.ChangeGame(igm:Game) =
            cgm <- igm
            gmchg <- true

        ///Changes the header of the Game that is selected
        member _.ChangeGameHdr(igm:Game) =
            cgm <- igm
            gmchg <- true
            let rw = gms.SelectedCells.[0].RowIndex
            chdr <- gmsui.[rw]
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
            crw <- cp.Hdrs.Length
            filtgms <- cp.Hdrs|>Array.mapi(fun i h -> (h,if cp.Mvss.[i].Length=0 then FsChess.Types.MoveEmpty else cp.Mvss.[i].[0]))
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>Array.iter(fun (hdr,mv) -> gmsui.Add(hdr))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            (filtgms,cbd,cp)|>filtEvt.Trigger
            chdr <- new Hdr()
            cmvstr <- ""
            cmvs <- [||]
            gmchg <- false
            cgm|>selEvt.Trigger

        ///Deletes selected Game
        member gms.DeleteGame() =
            let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
            let dr = MessageBox.Show("Do you really want to permanently delete the game: " + nm + " ?","Delete Game",MessageBoxButtons.YesNo)
            if dr=DialogResult.Yes then
                let orw = gms.SelectedCells.[0].RowIndex
                //save without gams
                //TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //reload saves fch
                cbd <- Board.Start
                filtgms <- cp.Hdrs|>Array.mapi(fun i h -> (h,if cp.Mvss.[i].Length=0 then FsChess.Types.MoveEmpty else cp.Mvss.[i].[0]))
                gmsui.Clear()
                let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
                dispgms|>Array.iter(fun (hdr,mv) -> gmsui.Add(hdr))
                gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                (filtgms,cbd,cp)|>filtEvt.Trigger
                gmchg <- false
                //select row
                let rw = if orw=0 then 0 else orw-1
                let hdr,mv = dispgms.[rw]
                crw <- hdr.Num
                chdr <- hdr
                cmvstr <- cp.MvsStrs.[crw]
                cmvs <- cp.Mvss.[crw]
                cgm <- Game.Set(cp,crw)
                cgm|>selEvt.Trigger
                gms.CurrentCell <- gms.Rows.[rw].Cells.[0]

        ///Export filtered games
        member gms.ExportFilter(filtfil:string) =
            let fcp = new ChessPack()
            let filtnms = filtgms|>Array.map fst|>Array.map(fun h -> h.Num)  
            fcp.Hdrs <- filtgms|>Array.map fst
            fcp.MvsStrs <- filtnms|>Array.map(fun i -> cp.MvsStrs.[i])
            Convert.ToPgn(filtfil,fcp,fun s -> ())
        
        ///Provides the revised filtered list of Games
        member __.FiltChng = filtEvt.Publish
        
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish

        ///Provides the initial Board when the PGN file selected changes
        member __.PgnChng = pgnEvt.Publish
