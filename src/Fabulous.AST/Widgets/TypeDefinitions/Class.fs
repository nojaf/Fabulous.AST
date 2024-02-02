namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fantomas.FCS.Text
open Fabulous.AST.StackAllocatedCollections
open Fantomas.Core.SyntaxOak
open Fabulous.AST.StackAllocatedCollections.StackList
open Microsoft.FSharp.Collections

module Class =
    let Name = Attributes.defineWidget "Name"
    let Parameters = Attributes.defineScalar<SimplePatNode list> "Parameters"
    let Members = Attributes.defineWidgetCollection "Members"
    let MultipleAttributes = Attributes.defineScalar<string list> "MultipleAttributes"
    let TypeParams = Attributes.defineScalar<string list> "TypeParams"

    let WidgetKey =
        Widgets.register "TypeDefnRegularNode" (fun widget ->
            let name = Helpers.getNodeFromWidget<SingleTextNode> widget Name
            let parameters = Helpers.tryGetScalarValue widget Parameters
            let members = Helpers.tryGetNodesFromWidgetCollection<MemberDefn> widget Members
            let attributes = Helpers.tryGetScalarValue widget MultipleAttributes
            let typeParams = Helpers.tryGetScalarValue widget TypeParams

            let typeParams =
                match typeParams with
                | ValueSome values ->
                    TyparDeclsPostfixListNode(
                        SingleTextNode.lessThan,
                        [ for v in values do
                              // FIXME - Update
                              TyparDeclNode(None, SingleTextNode.Create v, [], Range.Zero) ],
                        [],
                        SingleTextNode.greaterThan,
                        Range.Zero
                    )
                    |> TyparDecls.PostfixList
                    |> Some
                | ValueNone -> None

            let multipleAttributes =
                match attributes with
                | ValueSome values -> MultipleAttributeListNode.Create values |> Some
                | ValueNone -> None

            let members =
                match members with
                | None -> []
                | Some members -> members

            let implicitConstructor =
                match parameters with
                | ValueNone -> None
                | ValueSome(parameters) when parameters.IsEmpty ->
                    Some(
                        ImplicitConstructorNode(
                            None,
                            None,
                            None,
                            SingleTextNode.leftParenthesis,
                            [],
                            SingleTextNode.rightParenthesis,
                            None,
                            Range.Zero
                        )
                    )
                | ValueSome(simplePatNodes) ->
                    let simplePats =
                        match simplePatNodes with
                        | [] -> []
                        | head :: tail ->
                            [ yield Choice1Of2 head
                              for p in tail do
                                  yield Choice2Of2 SingleTextNode.comma
                                  yield Choice1Of2 p ]

                    Some(
                        ImplicitConstructorNode(
                            None,
                            None,
                            None,
                            SingleTextNode.leftParenthesis,
                            simplePats,
                            SingleTextNode.rightParenthesis,
                            None,
                            Range.Zero
                        )
                    )

            TypeDefnRegularNode(
                TypeNameNode(
                    None,
                    multipleAttributes,
                    SingleTextNode.``type``,
                    Some(name),
                    IdentListNode([], Range.Zero),
                    typeParams,
                    [],
                    implicitConstructor,
                    Some(SingleTextNode.equals),
                    None,
                    Range.Zero
                ),
                members,
                Range.Zero
            ))

[<AutoOpen>]
module ClassBuilders =
    type Ast with
        static member inline private BaseClass(name: WidgetBuilder<#SingleTextNode>, typeParams: string list voption) =
            let scalars =
                match typeParams with
                | ValueNone -> StackList.empty()
                | ValueSome typeParams -> StackList.one(Class.TypeParams.WithValue(typeParams))

            CollectionBuilder<TypeDefnRegularNode, MemberDefn>(
                Class.WidgetKey,
                Class.Members,
                AttributesBundle(scalars, ValueSome [| Class.Name.WithValue(name.Compile()) |], ValueNone)
            )

        static member inline Class(name: WidgetBuilder<#SingleTextNode>) = Ast.BaseClass(name, ValueNone)

        static member inline Class(name: SingleTextNode) = Ast.Class(Ast.EscapeHatch(name))

        static member inline Class(name: string) =
            Ast.Class(SingleTextNode(name, Range.Zero))

        static member inline Interface(name: WidgetBuilder<#SingleTextNode>) = Ast.BaseClass(name, ValueNone)

        static member inline Interface(name: SingleTextNode) = Ast.Class(Ast.EscapeHatch(name))

        static member inline Interface(name: string) =
            Ast.Class(SingleTextNode(name, Range.Zero))

        static member inline GenericClass(name: WidgetBuilder<#SingleTextNode>, typeParams: string list) =
            Ast.BaseClass(name, ValueSome typeParams)

        static member inline GenericClass(name: SingleTextNode, typeParams: string list) =
            Ast.GenericClass(Ast.EscapeHatch(name), typeParams)

        static member inline GenericClass(name: string, typeParams: string list) =
            Ast.GenericClass(SingleTextNode(name, Range.Zero), typeParams)

        static member inline GenericInterface(name: WidgetBuilder<#SingleTextNode>, typeParams: string list) =
            Ast.BaseClass(name, ValueSome typeParams)

        static member inline GenericInterface(name: SingleTextNode, typeParams: string list) =
            Ast.GenericClass(Ast.EscapeHatch(name), typeParams)

        static member inline GenericInterface(name: string, typeParams: string list) =
            Ast.GenericClass(SingleTextNode(name, Range.Zero), typeParams)

[<Extension>]
type ClassModifiers =
    [<Extension>]
    static member inline attributes(this: WidgetBuilder<TypeDefnRegularNode>, attributes: string list) =
        this.AddScalar(Class.MultipleAttributes.WithValue(attributes))

    [<Extension>]
    static member inline implicitConstructorParameters
        (
            this: WidgetBuilder<TypeDefnRegularNode>,
            parameters: SimplePatNode list
        ) =
        this.AddScalar(Class.Parameters.WithValue(parameters))

[<Extension>]
type ClassYieldExtensions =
    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<'parent, ModuleDecl>,
            x: WidgetBuilder<TypeDefnRegularNode>
        ) : CollectionContent =
        let node = Tree.compile x
        let typeDefn = TypeDefn.Regular(node)
        let typeDefn = ModuleDecl.TypeDefn(typeDefn)

        let widget = Ast.EscapeHatch(typeDefn).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: MethodMemberNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.Member(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: WidgetBuilder<MethodMemberNode>
        ) : CollectionContent =
        let node = Tree.compile x
        ClassYieldExtensions.Yield(this, node)

    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: PropertyMemberNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.Member(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: WidgetBuilder<PropertyMemberNode>
        ) : CollectionContent =
        let node = Tree.compile x
        ClassYieldExtensions.Yield(this, node)

    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: MemberDefnAbstractSlotNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.AbstractSlot(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: WidgetBuilder<MemberDefnAbstractSlotNode>
        ) : CollectionContent =
        let node = Tree.compile x
        ClassYieldExtensions.Yield(this, node)

    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: InterfaceMemberNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.Interface(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<TypeDefnRegularNode, MemberDefn>,
            x: WidgetBuilder<InterfaceMemberNode>
        ) : CollectionContent =
        let node = Tree.compile x
        ClassYieldExtensions.Yield(this, node)
