namespace FabWiz.Components

open Fabulous.XamarinForms
open System

module Tree =
    type Model =
        { Assemblies: Node list }
        
    type Msg =
        | ToggleExpandNode of Guid
        
    type CmdMsg = CmdMsgNone
    type ExternalMsg = ExternalMsgNone
    
    let init () =
        { Assemblies = [
            { Id = Guid.NewGuid(); Name = "Xamarin.Forms.Core"; IsExpanded = false; Children = [
                { Id = Guid.NewGuid(); Name = "BindableObject"; IsExpanded = false; Children = [
                    { Id = Guid.NewGuid(); Name = "NavigableElement"; IsExpanded = false; Children = [
                        { Id = Guid.NewGuid(); Name = "Button"; IsExpanded = false; Children = [
                            { Id = Guid.NewGuid(); Name = "Properties"; IsExpanded = false; Children = [
                                { Id = Guid.NewGuid(); Name = "Text"; IsExpanded = false; Children = [] }
                                { Id = Guid.NewGuid(); Name = "Command"; IsExpanded = false; Children = [] }
                                { Id = Guid.NewGuid(); Name = "FontSize"; IsExpanded = false; Children = [] }
                            ] }
                            { Id = Guid.NewGuid(); Name = "Events"; IsExpanded = false; Children = [
                                { Id = Guid.NewGuid(); Name = "Clicked"; IsExpanded = false; Children = [] }
                                { Id = Guid.NewGuid(); Name = "Resized"; IsExpanded = false; Children = [] }
                            ] }
                        ] }
                        { Id = Guid.NewGuid(); Name = "Label"; IsExpanded = false; Children = [
                            { Id = Guid.NewGuid(); Name = "Properties"; IsExpanded = false; Children = [
                                { Id = Guid.NewGuid(); Name = "Text"; IsExpanded = false; Children = [] }
                                { Id = Guid.NewGuid(); Name = "FontSize"; IsExpanded = false; Children = [] }
                            ] }
                            { Id = Guid.NewGuid(); Name = "Events"; IsExpanded = false; Children = [] }
                        ] }
                    ] }
                ] }
            ] }
            { Id = Guid.NewGuid(); Name = "Fabulous.XamarinForms"; IsExpanded = false; Children = [
                { Id = Guid.NewGuid(); Name = "CustomContentPage"; IsExpanded = false; Children = [
                    { Id = Guid.NewGuid(); Name = "Properties"; IsExpanded = false; Children = [
                        { Id = Guid.NewGuid(); Name = "Text"; IsExpanded = false; Children = [] }
                    ] }
                    { Id = Guid.NewGuid(); Name = "Events"; IsExpanded = false; Children = [
                        { Id = Guid.NewGuid(); Name = "Clicked"; IsExpanded = false; Children = [] }
                        { Id = Guid.NewGuid(); Name = "Layout"; IsExpanded = false; Children = [] }
                    ] }
                ] }
            ] }
        ] }
        
    let rec updateNodeIsExpandedRec (remainingNodes: Node list) (expandedNodeId: Guid) (updatedNodes: Node list) =
        match remainingNodes with
        | [] -> false, updatedNodes
        | node::rest ->
            if node.Id = expandedNodeId then
                let updatedNodes = { node with IsExpanded = not node.IsExpanded } :: updatedNodes
                true, List.rev rest @ updatedNodes
            else
                if node.Children.Length = 0 then
                    updateNodeIsExpandedRec rest expandedNodeId (node :: updatedNodes)
                else
                    match updateNodeIsExpandedRec node.Children expandedNodeId [] with
                    | false, _ -> updateNodeIsExpandedRec rest expandedNodeId (node :: updatedNodes)
                    | true, childNodes ->
                        let updatedNodes = { node with Children = List.rev childNodes } :: updatedNodes
                        true, List.rev rest @ updatedNodes
                    
    let updateNodeIsExpanded nodes expandedNode =
        let _, updatedNodes = updateNodeIsExpandedRec nodes expandedNode []
        List.rev updatedNodes
        
    let update msg model =
        match msg with
        | ToggleExpandNode nodeId -> { model with Assemblies = updateNodeIsExpanded model.Assemblies nodeId }
        
    let view model dispatch =
        let toggleExpand = fun nodeId -> dispatch (ToggleExpandNode nodeId)
        
        View.ScrollView(
            View.StackLayout(
                Nodes.create model.Assemblies toggleExpand
            )
        )
