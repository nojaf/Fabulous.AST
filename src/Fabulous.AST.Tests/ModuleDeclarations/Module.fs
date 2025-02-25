namespace Fabulous.AST.Tests.ModuleDeclarations

open Fabulous.AST.Tests
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak
open NUnit.Framework

open Fabulous.AST

open type Ast

module Module =
    [<Test>]
    let ``Produces a module with binding`` () =
        Module("Fabulous.AST") { Value("x", "3") }
        |> produces
            """
module Fabulous.AST

let x = 3
"""

    [<Test>]
    let ``Produces a module with unit`` () =
        Module("Fabulous.AST") { ConstantExpr(ConstantUnit()) }
        |> produces
            """
module Fabulous.AST

()
"""

    [<Test>]
    let ``Produces a module with IdentListNode`` () =
        Module("Fabulous.AST") { Value("x", "3") }
        |> produces
            """
module Fabulous.AST

let x = 3
"""

    [<Test>]
    let ``Produces a module with IdentListNode and BindingNode`` () =
        Module("Fabulous.AST") {
            BindingNode(
                None,
                None,
                MultipleTextsNode([ SingleTextNode("let", Range.Zero) ], Range.Zero),
                false,
                None,
                None,
                Choice1Of2(IdentListNode([ IdentifierOrDot.Ident(SingleTextNode("x", Range.Zero)) ], Range.Zero)),
                None,
                List.Empty,
                None,
                SingleTextNode("=", Range.Zero),
                Expr.Constant(Constant.FromText(SingleTextNode("12", Range.Zero))),
                Range.Zero
            )
        }
        |> produces
            """
module Fabulous.AST

let x = 12
"""

    [<Test>]
    let ``Produces a module with nested module`` () =
        Module("Fabulous.AST") {
            NestedModule("Foo") {
                BindingNode(
                    None,
                    None,
                    MultipleTextsNode([ SingleTextNode("let", Range.Zero) ], Range.Zero),
                    false,
                    None,
                    None,
                    Choice1Of2(IdentListNode([ IdentifierOrDot.Ident(SingleTextNode("x", Range.Zero)) ], Range.Zero)),
                    None,
                    List.Empty,
                    None,
                    SingleTextNode("=", Range.Zero),
                    Expr.Constant(Constant.FromText(SingleTextNode("12", Range.Zero))),
                    Range.Zero
                )
            }
        }

        |> produces
            """
module Fabulous.AST

module Foo =
    let x = 12
"""

    [<Test>]
    let ``Produces a module with multiple nested module`` () =
        Module("Fabulous.AST") {
            NestedModule("Foo") {
                BindingNode(
                    None,
                    None,
                    MultipleTextsNode([ SingleTextNode("let", Range.Zero) ], Range.Zero),
                    false,
                    None,
                    None,
                    Choice1Of2(IdentListNode([ IdentifierOrDot.Ident(SingleTextNode("x", Range.Zero)) ], Range.Zero)),
                    None,
                    List.Empty,
                    None,
                    SingleTextNode("=", Range.Zero),
                    Expr.Constant(Constant.FromText(SingleTextNode("12", Range.Zero))),
                    Range.Zero
                )
            }

            NestedModule("Bar") { Value("x", "12") }
        }

        |> produces
            """
module Fabulous.AST

module Foo =
    let x = 12

module Bar =
    let x = 12

"""
