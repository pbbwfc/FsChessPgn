namespace KindleChess

open System
open System.Text
open System.Drawing
open System.Drawing.Imaging
open System.IO

[<AutoOpen>]
module Types =
    ///BookT - type holding data the defines a Kindle Book
    type BookT =
        { Title : string
          Creator : string
          Date : DateTime
          Desc : string
          Welcome : string
          IsW : bool
          Chapters : string list }
    
