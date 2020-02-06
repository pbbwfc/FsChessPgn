namespace DbConvert

open System
open System.Windows.Forms
open FsChessDb

module Form =

    type FrmMain() as this =
        inherit Form(Text = "Db Convert", Height = 500, Width = 400, FormBorderStyle = FormBorderStyle.FixedToolWindow,StartPosition=FormStartPosition.CenterScreen)
    
        let vc =
            new FlowLayoutPanel(FlowDirection = FlowDirection.TopDown, 
                                Height = 300, Width = 380,Dock=DockStyle.Fill)
        let hc1 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 40, Width = 380)
        let pgntb = new TextBox(Text="Please select PGN file", Width=320, Height=35)
        let pgnbtn = new Button(Text = "...",Width=30)
        let hc2 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 40, Width =380)
        let fchtb = new TextBox(Text="Please select FCH file", Width=320, Height=35)
        let fchbtn = new Button(Text = "...",Width=30)
    
        let logtb = new TextBox(Width=380, Height=320, Multiline=true, BorderStyle=BorderStyle.FixedSingle)
        
        let hc3 =
            new FlowLayoutPanel(FlowDirection = FlowDirection.RightToLeft, 
                                Height = 30, Width = 380)
        let okbtn = new Button(Text = "Convert")
        let cnbtn = new Button(Text = "Exit")

        let selpgn(e) =
            let dlg = new OpenFileDialog(Filter = "pgn files (*.pgn)|*.pgn")
            if dlg.ShowDialog() = DialogResult.OK then
                pgntb.Text <- dlg.FileName

        let selfch(e) =
            let dlg = new SaveFileDialog(Filter = "fch files (*.fch)|*.fch")
            if dlg.ShowDialog() = DialogResult.OK then
                fchtb.Text <- dlg.FileName
            
        let convert(e) =
            let nl = System.Environment.NewLine
            let log txt =
                logtb.Text <- logtb.Text + nl + DateTime.Now.ToLongTimeString() + "  " + txt
                Application.DoEvents()
            Convert.PgnToFch(pgntb.Text,fchtb.Text,log)


        do 
            hc1.Controls.Add(pgnbtn)
            hc1.Controls.Add(pgntb)
            vc.Controls.Add(hc1)
            hc2.Controls.Add(fchbtn)
            hc2.Controls.Add(fchtb)
            vc.Controls.Add(hc2)
            vc.Controls.Add(logtb)
            hc3.Controls.Add(cnbtn)
            hc3.Controls.Add(okbtn)
            vc.Controls.Add(hc3)
            this.Controls.Add(vc)
            //events
            cnbtn.Click.Add(fun _ -> this.Close())
            pgnbtn.Click.Add(selpgn)
            fchbtn.Click.Add(selfch)
            okbtn.Click.Add(convert)