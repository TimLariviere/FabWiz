namespace FabWiz.Components

open Fabulous.XamarinForms
open System
open Xamarin.Forms
    
type Node =
    { Id: Guid
      Name: string
      IsExpanded: bool
      Children: Node list }

module Nodes =
    let private createNode (node: Node) indent showToggleButton toggleExpandFunc =
        let indentSize = 10
        
        let toggleExpand = fun () -> toggleExpandFunc node.Id
        
        View.Grid(
            padding = Thickness (float (indent * indentSize), 0., 0., 0.),
            coldefs = [ Absolute 25.; Star ],
            children = [
                if showToggleButton then
                    yield View.Button(
                        width = 25.,
                        height = 25.,
                        text = (if node.IsExpanded then "-" else "+"),
                        command = toggleExpand
                    )
                    
                yield View.Label(node.Name)
                              .GridColumn(1)
            ]
        )
        
    let rec createNodeWithChildren indent (node: Node) toggleExpandFunc =
        let hasChildren = node.Children.Length > 0
        [
            yield createNode node indent hasChildren toggleExpandFunc
            
            if node.IsExpanded && hasChildren then
                for child in node.Children do
                    for childNode in (createNodeWithChildren (indent + 1) child toggleExpandFunc) do
                        yield childNode
        ]
            
    let create (nodes: Node list) toggleExpandFunc =
        [
            for node in nodes do
                for n in (createNodeWithChildren 0 node toggleExpandFunc) do
                    yield n
        ]