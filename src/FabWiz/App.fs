namespace FabWiz

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FabWiz.Components

module App = 
    type Model =
        { TreeComponentModel: Tree.Model }

    type Msg =
        | TreeComponentMsg of Tree.Msg
    
    let init () = { TreeComponentModel = Tree.init () }, Cmd.none

    let update msg model =
        match msg with
        | TreeComponentMsg msg -> { model with TreeComponentModel = Tree.update msg model.TreeComponentModel }, Cmd.none

    let view (model: Model) dispatch =
        View.ContentPage(
          View.StackLayout(
            padding = Thickness 10.,
            children = [
                Tree.view model.TreeComponentModel (TreeComponentMsg >> dispatch)
            ]))

    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app
