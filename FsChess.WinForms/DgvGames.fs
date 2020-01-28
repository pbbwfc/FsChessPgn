namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess
open FsChess.Pgn

[<AutoOpen>]
module Library4 =
    type GmUI =
        {
            Num : int
            White : string
            W_Elo : string
            Black : string
            B_Elo : string
            Result : string
            Date : string
            Event : string
            Round : string
            Site : string
        }
    
    type DgvGames() as gms =
        inherit DataGridView(Width = 800, Height = 250, 
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.Black, MultiSelect = false,
                RowHeadersVisible=false)

        let mutable pgn = ""
        let mutable allgms:(int * Game) list = []
        let mutable indx = new System.Collections.Generic.Dictionary<Set<Square>,int list>()
        let mutable cbd = Board.Start
        let mutable filtgms:(int * Game * string) list = []
        let mutable crw = -1
        let mutable cgm = GameEMP
        let mutable gmchg = false
        let mutable gmsui = new System.ComponentModel.BindingList<GmUI>()
        let bs = new BindingSource()

        //events
        let filtEvt = new Event<_>()
        let selEvt = new Event<_>()

        let igm2gmui (igmmv:(int * Game * string)) =
            let i,gm,mv = igmmv
            {
                Num = i+1
                White = gm.WhitePlayer
                W_Elo = gm.WhiteElo
                Black = gm.BlackPlayer
                B_Elo = gm.BlackElo
                Result = gm.Result|>Result.ToUnicode
                Date = gm|>GameDate.ToStr
                Event = gm.Event
                Round = gm.Round
                Site = gm.Site
            }
        
        let dosave() =
            if crw=0 then
                ((0,cgm)::allgms.Tail)|>List.map snd|>Games.WriteFile pgn
            elif crw=allgms.Length-1 then
                (allgms.[..crw-1]@[crw,cgm])|>List.map snd|>Games.WriteFile pgn
            else
                (allgms.[..crw-1]@[crw,cgm]@allgms.[crw+1..])|>List.map snd|>Games.WriteFile pgn
        
        let doclick(e:DataGridViewCellEventArgs) =
            let rw = e.RowIndex
            //need to check if want to save
            if gmchg then
                let nm = cgm.WhitePlayer + " v. " + cgm.BlackPlayer
                let dr = MessageBox.Show("Do you want to save the game: " + nm + " ?","Save Game",MessageBoxButtons.YesNoCancel)
                if dr=DialogResult.Yes then
                    dosave()
                    let ci,cg,_ = filtgms.[rw]
                    crw <- ci
                    cgm <- cg
                    cgm|>selEvt.Trigger
                elif dr=DialogResult.No then
                    let ci,cg,_ = filtgms.[rw]
                    crw <- ci
                    cgm <- cg
                    cgm|>selEvt.Trigger
            else
                let ci,cg,_ = filtgms.[rw]
                crw <- ci
                cgm <- cg
                cgm|>selEvt.Trigger
            gms.CurrentCell <- gms.Rows.[rw].Cells.[0]
        
        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs

        do 
            setup()
            gms.CellClick.Add(doclick)

        ///Sets the PGN file to be used
        member gms.SetPgn(ipgn:string) =
            pgn <- ipgn
            allgms <- pgn|>Games.ReadIndexListFromFile
            pgn|>Games.CreateIndex
            indx <- pgn|>Games.GetIndex
            cbd <- Board.Start
            filtgms <- allgms|>Games.FastFindBoard cbd indx
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            filtgms|>filtEvt.Trigger
            gmchg <- false
            if not dispgms.IsEmpty then
                let ci,cg,_ = dispgms.Head
                crw <- ci
                cgm <- cg
                cgm|>selEvt.Trigger

        ///Saves the PGN file
        member _.SavePgn() = dosave()

        ///Saves the PGN file with a new name
        member _.SaveAsPgn(ipgn:string) = 
            //copy index
            System.IO.File.Copy(pgn + ".indx",ipgn + ".indx")
            pgn <- ipgn
            dosave()

        ///Sets the Board to be filtered on
        member gms.SetBoard(ibd:Brd) =
            cbd <- ibd
            filtgms <- allgms|>Games.FastFindBoard cbd indx
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
            gms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            filtgms|>filtEvt.Trigger

        ///Changes the contents of the Game that is selected
        member _.ChangeGame(igm:Game) =
            cgm <- igm
            gmchg <- true

        ///Provides the revised filtered list of Games
        member __.FiltChng = filtEvt.Publish
        
        ///Provides the selected Game
        member __.GmSel = selEvt.Publish
