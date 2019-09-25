// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace FabWiz

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System.IO
open Fabulous.CodeGen.AssemblyReader.Models
open Fabulous.CodeGen.Models
open FabWiz.Hierarchy
open Xamarin.Forms

module App = 
    type Model = 
      { Hierarchy : Node option }

    type Msg = 
        | TestCodeGen
        | Loaded of Node
        
    let loadAssemblyReaderOutput () =
        let readerTypes =
            File.ReadAllText "/Git/GitHub/Fabulous/1-assembly-types.json"
            |> Fabulous.CodeGen.Json.deserialize<AssemblyType array>
            
        let bindings =
            File.ReadAllText "/Git/GitHub/Fabulous/Fabulous.XamarinForms/src/Fabulous.XamarinForms/Xamarin.Forms.Core.json"
            |> Fabulous.CodeGen.Json.deserialize<Bindings>
        
        Hierarchy.createHierarchy readerTypes bindings
        |> Loaded
        |> Cmd.ofMsg

    let initModel = { Hierarchy = None }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | TestCodeGen -> model, loadAssemblyReaderOutput()
        | Loaded root -> { model with Hierarchy = Some root }, Cmd.none
        
    let rec hierarchyView (indent: int) (node: Node) =
        let marginLeft = Thickness((float indent) * 20., 0., 0., 0.)
        
        [
            yield View.Label(
                margin = marginLeft,
                text = sprintf "%i - %s" indent (node.TypeName.Replace("Xamarin.Forms.", "")),
                textColor = match node.BindState with Bound -> Color.Green | Unbound -> Color.LightGray)
            
            if node.Properties.Length > 0 then
                for p in node.Properties do
                    yield View.Label(
                        margin = Thickness(marginLeft.Left + 20., 0., 0., 0.),
                        text = sprintf "Prop - %s" p)
            
            if node.Properties.Length > 0 && node.Descendants.Length > 0 then
                yield View.Label(
                    margin = Thickness(marginLeft.Left + 20., 0., 0., 0.),
                    text = "--------")
            
            if node.Descendants.Length > 0 then
                for descendant in node.Descendants do
                    for n in (hierarchyView (indent + 1) descendant) do
                        yield n
        ]

    let view (model: Model) dispatch =
        View.ContentPage(
          content = View.StackLayout(
            padding = 20.0,
            children = [ 
                yield View.Button(text = "Test CodeGen", command = fun () ->  dispatch TestCodeGen)
                
                match model.Hierarchy with
                | None -> yield View.Label("No hierarchy loaded")
                | Some root ->
                    let nodes = (hierarchyView 0 root)
                    
                    yield View.ScrollView(
                        verticalOptions = LayoutOptions.FillAndExpand,
                        content = View.StackLayout(children = nodes))
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
