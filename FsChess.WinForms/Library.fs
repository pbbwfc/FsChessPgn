namespace FsChess.WinForms

open System.Drawing
open System.Windows.Forms
open FsChess

[<AutoOpen>]
module Library =
    
    let private img nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Images." + nm)
        Image.FromStream(file)

    let private cur nm =
        let thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        //let nms = thisExe.GetManifestResourceNames()
        let file = thisExe.GetManifestResourceStream("FsChess.WinForms.Cursors." + nm)
        new Cursor(file)
 

    type PnlBoard() as bd =
        inherit Panel(Width = 400, Height = 450)

        let mutable board = Board.Start
        let mutable sqTo = -1
        let mutable cCur = Cursors.Default
        let bdpnl = new Panel(Dock = DockStyle.Top, Height = 400)
        let sqpnl = new Panel(Width = 420, Height = 420, Left = 29, Top = 13)
        
        let edges =
            [ new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 24, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 24, Top = 8)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 8, 
                        Height = 350, Left = 366, Top = 6)
              
              new Panel(BackgroundImage = img "Back.jpg", Width = 342, 
                        Height = 8, Left = 32, Top = 350) ]
        
        let sqs : PictureBox [] = Array.zeroCreate 64
        let flbls : Label [] = Array.zeroCreate 8
        let rlbls : Label [] = Array.zeroCreate 8

        /// get cursor given char
        let getcur c =
            match c with
            | "P" -> cur "WhitePawn.cur"
            | "B" -> cur "WhiteBishop.cur"
            | "N" -> cur "WhiteKnight.cur"
            | "R" -> cur "WhiteRook.cur"
            | "K" -> cur "WhiteKing.cur"
            | "Q" -> cur "WhiteQueen.cur"
            | "p" -> cur "BlackPawn.cur"
            | "b" -> cur "BlackBishop.cur"
            | "n" -> cur "BlackKnight.cur"
            | "r" -> cur "BlackRook.cur"
            | "k" -> cur "BlackKing.cur"
            | "q" -> cur "BlackQueen.cur"
            | _ -> failwith "invalid piece"
 
        /// get image given char
        let getim c =
            match c with
            | "P" -> img "WhitePawn.png"
            | "B" -> img "WhiteBishop.png"
            | "N" -> img "WhiteKnight.png"
            | "R" -> img "WhiteRook.png"
            | "K" -> img "WhiteKing.png"
            | "Q" -> img "WhiteQueen.png"
            | "p" -> img "BlackPawn.png"
            | "b" -> img "BlackBishop.png"
            | "n" -> img "BlackKnight.png"
            | "r" -> img "BlackRook.png"
            | "k" -> img "BlackKing.png"
            | "q" -> img "BlackQueen.png"
            | _ -> failwith "invalid piece"

        /// Action for GiveFeedback
        let giveFeedback (e : GiveFeedbackEventArgs) =
            e.UseDefaultCursors <- false
            sqpnl.Cursor <- cCur

        /// Action for Drag Over
        let dragOver (e : DragEventArgs) = e.Effect <- DragDropEffects.Move

        /// Action for Drag Drop
        let dragDrop (p : PictureBox, e) =
            sqTo <- System.Convert.ToInt32(p.Tag)
            sqpnl.Cursor <- Cursors.Default

        /// Action for Mouse Down
        let mouseDown (p : PictureBox, e : MouseEventArgs) =
            if e.Button = MouseButtons.Left then 
                let sqFrom = System.Convert.ToInt32(p.Tag)
                //stt.GetPossSqs(sqFrom)
                let oimg = p.Image
                p.Image <- null
                p.Refresh()
                let c = board.PieceAt.[sqFrom]|>Piece.ToStr
                cCur <- getcur c
                sqpnl.Cursor <- cCur
                //if stt.PsSqs.Length > 0 
                //   && (p.DoDragDrop(oimg, DragDropEffects.Move) = DragDropEffects.Move) then 
                //    stt.Move(sqFrom, sqTo)
                //else p.Image <- oimg
                //sqpnl.Cursor <- Cursors.Default

        /// creates file label
        let flbl i lbli =
            let lbl = new Label()
            lbl.Text <- FILE_NAMES.[i]
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 21
            lbl.Width <- 42
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- i * 42 + 30
            lbl.Top <- 8 * 42 + 24
            flbls.[i] <- lbl

        /// creates rank label
        let rlbl i lbli =
            let lbl = new Label()
            lbl.Text <- (i + 1).ToString()
            lbl.Font <- new Font("Arial", 12.0F, FontStyle.Bold, 
                                 GraphicsUnit.Point, byte (0))
            lbl.ForeColor <- Color.Green
            lbl.Height <- 42
            lbl.Width <- 21
            lbl.TextAlign <- ContentAlignment.MiddleCenter
            lbl.Left <- 0
            lbl.Top <- 7 * 42 - i * 42 + 16
            rlbls.[i] <- lbl
       
        ///set board colours and position of squares
        let setsq i sqi =
            let r = i / 8
            let f = i % 8
            let sq =
                new PictureBox(Height = 42, Width = 42, 
                               SizeMode = PictureBoxSizeMode.CenterImage)
            sq.BackColor <- if (f + r) % 2 = 1 then Color.Green
                            else Color.PaleGreen
            sq.Left <- f * 42 + 1
            sq.Top <- r * 42 + 1
            sq.Tag <- i
            //events
            sq.MouseDown.Add(fun e -> mouseDown (sq, e))
            sq.DragDrop.Add(fun e -> dragDrop (sq, e))
            sq.AllowDrop <- true
            sq.DragOver.Add(dragOver)
            sq.GiveFeedback.Add(giveFeedback)
            sqs.[i] <- sq
        
        ///set pieces on squares
        let setpcsmvs (p : Brd) =
            let setpcsmvs() =
                p.PieceAt
                |>List.map Piece.ToStr
                |> List.iteri (fun i c -> sqs.[i].Image <- if c = " " then null else getim c)
                //mvstb.Text <- stt.GetMvsStr()
            if (bd.InvokeRequired) then 
                try 
                    bd.Invoke(MethodInvoker(setpcsmvs)) |> ignore
                with _ -> ()
            else setpcsmvs()
        
        
        
        
        
        
        do 
            //mvstb |> mvspnl.Controls.Add
            //mvspnl |> bd.Controls.Add
            sqs |> Array.iteri setsq
            sqs |> Array.iter sqpnl.Controls.Add
            board |> setpcsmvs
            edges |> List.iter bdpnl.Controls.Add
            flbls |> Array.iteri flbl
            flbls |> Array.iter bdpnl.Controls.Add
            rlbls |> Array.iteri rlbl
            rlbls |> Array.iter bdpnl.Controls.Add
            sqpnl |> bdpnl.Controls.Add
            bdpnl |> bd.Controls.Add

