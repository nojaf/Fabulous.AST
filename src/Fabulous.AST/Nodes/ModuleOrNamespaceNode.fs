namespace Fabulous.AST

open System.Runtime.CompilerServices
open FSharp.Compiler.Text
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak

type IFabModuleOrNamespace = inherit IFabNodeBase

module ModuleOrNamespace =
    
    let ModuleDecls = Attributes.defineWidgetCollection "ModuleDecls"
    
    let WidgetKey = Widgets.register(fun (widget: Widget) ->
        let struct (numberOfElements, moduleDecls) = Helpers.getWidgetCollectionValue widget ModuleDecls
        let moduleDecls =
            moduleDecls
            |> Array.take(int numberOfElements)
            |> Array.map(Helpers.createValueForWidget)
            |> List.ofArray
        ModuleOrNamespaceNode(None, moduleDecls, Range.Zero)
    )
    
[<AutoOpen>]
module ModuleOrNamespaceBuilders =
    type Fabulous.AST.Node with
        static member inline ModuleOrNamespace() =
            CollectionBuilder<IFabModuleOrNamespace, IFabModuleOrNamespace>(ModuleOrNamespace.WidgetKey, ModuleOrNamespace.ModuleDecls) 
            

[<Extension>]
type ModuleOrNamespaceYieldExtensions =
    [<Extension>]
    static member inline Yield(_: CollectionBuilder<#IFabModuleOrNamespace, IFabModuleOrNamespace>, x: WidgetBuilder<#IFabModuleOrNamespace>) : Content =
        { Widgets = MutStackArray1.One(x.Compile()) }
