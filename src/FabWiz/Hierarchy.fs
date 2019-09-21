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
          Descendants: Node list }
        
    let rec private createNodeForType (assemblies: AssemblyType array) (bindings: Bindings) typeName =
        let descendants =
            assemblies
            |> Array.filter (fun asm -> asm.InheritanceHierarchy.Length > 0 && asm.InheritanceHierarchy.[0] = typeName)
            |> Array.map (fun asm -> asm.Name)
            |> Array.sort
            |> List.ofArray
            
        let isBound = bindings.Types |> Array.exists (fun t -> t.Type = typeName)
            
        { TypeName = typeName
          BindState = if isBound then Bound else Unbound
          Descendants = descendants |> List.map (createNodeForType assemblies bindings) }
        
    let createHierarchy (assemblies: AssemblyType array) (bindings: Bindings) =
        assemblies
        |> Array.find (fun asm -> asm.InheritanceHierarchy.Length = 0)
        |> (fun asm -> asm.Name)
        |> createNodeForType assemblies bindings

