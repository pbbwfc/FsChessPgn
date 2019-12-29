//This does not work (yet?)
//Fails to use 'System.IO.StringWriter' from assembly 'System.Runtime.Extensions
//Error is for example Readme7.fsx
//Hopefully it will go way!
//Referencing the list of fs files works!

#I @"D:\GitHub\FsChessPgn\FsChessPgn\bin\Debug\netcoreapp3.1"
#r "FsChessPgn.dll"
open FsChess
do
    fsi.AddPrinter<Move>(Pretty.Move)
    fsi.AddPrinter<Square>(Pretty.Square)
    fsi.AddPrinter<Brd>(Pretty.Board)
    fsi.AddPrinter<Game>(Pretty.Game)

