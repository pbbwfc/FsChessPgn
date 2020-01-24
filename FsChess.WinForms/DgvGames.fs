namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess

[<AutoOpen>]
module Library4 =
    type GmUI =
        {
            Number : int
            White : string
            Black : string
        }
    
    type DgvGames() as gms =
        inherit DataGridView(Width = 400, Height = 400, 
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
        let mutable filtgms:(int * Game * string) list = []
        let mutable crw = -1
        let mutable gmsui = new System.ComponentModel.BindingList<GmUI>()
        let bs = new BindingSource()

        let setup() =
            bs.DataSource <- gmsui
            gms.DataSource <- bs

        do 
            setup()

        member gms.SetGames(igms:(int * Game) list) =
            allgms <- igms
