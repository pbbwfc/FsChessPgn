namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess
open FsChess.Pgn

[<AutoOpen>]
module Library4 =
    type GmUI =
        {
            Number : int
            White : string
            Black : string
            Result : string
            Date : string
            Event : string
            Round : string
            Site : string
        }
    
    type DgvGames() as gms =
        inherit DataGridView(Width = 800, Height = 400, 
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.Black, MultiSelect = false)

        let mutable pgn = ""
        let mutable allgms:(int * Game) list = []
        let mutable indx = new System.Collections.Generic.Dictionary<Set<Square>,int list>()
        let mutable cbd = Board.Start
        let mutable filtgms:(int * Game * string) list = []
        let mutable crw = -1
        let mutable gmsui = new System.ComponentModel.BindingList<GmUI>()
        let bs = new BindingSource()

        let igm2gmui (igmmv:(int * Game * string)) =
            let i,gm,mv = igmmv
            {
                Number = i
                White = gm.WhitePlayer
                Black = gm.BlackPlayer
                Result = gm.Result|>Result.ToUnicode
                Date = gm|>GameDate.ToStr
                Event = gm.Event
                Round = gm.Round
                Site = gm.Site
            }
            
        
        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs

        do 
            setup()

        member gms.SetPgn(ipgn:string) =
            pgn <- ipgn
            allgms <- pgn|>Games.ReadIndexListFromFile
            pgn|>Games.CreateIndex
            indx <- pgn|>Games.GetIndex
            Board.Start|>gms.SetBoard

        member gms.SetBoard(ibd:Brd) =
            cbd <- ibd
            filtgms <- allgms|>Games.FastFindBoard cbd indx
            gmsui.Clear()
            let dispgms = if filtgms.Length>201 then filtgms.[..200] else filtgms 
            dispgms|>List.map igm2gmui|>List.iter(fun gmui -> gmsui.Add(gmui))
