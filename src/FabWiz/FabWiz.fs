// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace FabWiz

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System.IO
open Newtonsoft.Json
open Fabulous.CodeGen.AssemblyReader.Models

module App = 
    type Model = 
      { Count : int }

    type Msg = 
        | TestCodeGen
        | Done
        
    let loadAssemblyReaderOutput () =
        let x =
            File.ReadAllText "/Git/GitHub/Fabulous/1-assembly-types.json"
            |> JsonConvert.DeserializeObject<AssemblyType array>
        
        Cmd.none

    let initModel = { Count = 0 }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | TestCodeGen -> model, loadAssemblyReaderOutput()

    let view (model: Model) dispatch =
        View.ContentPage(
          content = View.StackLayout(padding = 20.0, verticalOptions = LayoutOptions.Center,
            children = [ 
                View.Button(text = "Test CodeGen", command = fun () ->  dispatch TestCodeGen)
            ]))

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app
