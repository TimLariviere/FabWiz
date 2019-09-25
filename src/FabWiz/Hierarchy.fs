namespace FabWiz
open Fabulous.CodeGen.AssemblyReader.Models
open Fabulous.CodeGen.Models

module Hierarchy =
    type BindState =
        | Bound
        | Unbound
    
    type Node =
        { TypeName: string
          BindState: BindState
          Properties: string list
          Descendants: Node list }
        
    let rec private createNodeForType (assemblies: AssemblyType array) (bindings: Bindings) typeName =
        let properties =
            assemblies
            |> Array.find (fun asm -> asm.Name = typeName)
            |> (fun asm -> asm.Properties)
            |> Array.map (fun p -> p.Name)
            |> Array.sort
            |> List.ofArray
        
        let descendants =
            assemblies
            |> Array.filter (fun asm -> asm.InheritanceHierarchy.Length > 0 && asm.InheritanceHierarchy.[0] = typeName)
            |> Array.map (fun asm -> asm.Name)
            |> Array.sort
            |> List.ofArray
            
        let isBound = bindings.Types |> Array.exists (fun t -> t.Type = typeName)
            
        { TypeName = typeName
          BindState = if isBound then Bound else Unbound
          Properties = properties
          Descendants = descendants |> List.map (createNodeForType assemblies bindings) }
        
    let createHierarchy (assemblies: AssemblyType array) (bindings: Bindings) =
        assemblies
        |> Array.find (fun asm -> asm.InheritanceHierarchy.Length = 0)
        |> (fun asm -> asm.Name)
        |> createNodeForType assemblies bindings

