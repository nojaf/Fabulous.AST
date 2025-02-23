namespace Fabulous.AST.Tests.Expressions

open NUnit.Framework
open Fabulous.AST.Tests

open Fabulous.AST

open type Ast

module InfixApp =

    [<Test>]
    let ``let value with a InfixApp expression`` () =
        AnonymousModule() { InfixAppExpr(ConstantExpr(ConstantString "a"), "|>", ConstantExpr(ConstantString "b")) }
        |> produces
            """

a |> b
"""
