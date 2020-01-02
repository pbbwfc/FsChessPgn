﻿namespace FsChess.WinForms

open System
open System.Windows.Forms

module Main =
    [<STAThread>]
    Application.EnableVisualStyles()
    
    type FrmMain() as this =
        inherit Form(Text = "Show Board", Width = 500, Height = 500)

        let bd = new PnlBoard()

        do
            bd|>this.Controls.Add
    
    let frm = new FrmMain()
    
    Application.Run(frm)