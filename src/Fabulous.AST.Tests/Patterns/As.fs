namespace Fabulous.AST.Tests.Patterns

open NUnit.Framework
open Fabulous.AST.Tests

open Fabulous.AST

open type Ast

module As =

    [<Test>]
    let ``let value with a As pattern`` () =
        AnonymousModule() { Value(AsPat(NamedPat("A"), NamedPat("B")), ConstantExpr(ConstantString "12")) }
        |> produces
            """

let A as B = 12
"""

    [<Test>]
    let ``let value with a As custom middle pattern`` () =
        AnonymousModule() { Value(AsPat(NamedPat("A"), "^", NamedPat("B")), ConstantExpr(ConstantString "12")) }
        |> produces
            """

let A ^ B = 12
"""
