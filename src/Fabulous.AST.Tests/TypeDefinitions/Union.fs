namespace Fabulous.AST.Tests.TypeDefinitions

open Fabulous.AST.Tests
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak
open NUnit.Framework

open Fabulous.AST

open type Fabulous.AST.Ast

module Union =

    [<Test>]
    let ``Produces an union`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union with interface member`` () =
        AnonymousModule() {

            Interface("IMyInterface") {
                let parameters = [ CommonType.Unit ]
                AbstractCurriedMethodMember("GetValue", parameters, CommonType.String)
            }

            (Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            })
                .members() {
                InterfaceMember("IMyInterface") {
                    Member("x.GetValue", UnitPat()) { ConstantExpr(ConstantString "\"\"") }
                }
            }

        }

        |> produces
            """
type IMyInterface =
    abstract member GetValue: unit -> string

type Colors =
    | Red
    | Green
    | Blue
    | Yellow

    interface IMyInterface with
        member x.GetValue() = ""

"""

    [<Test>]
    let ``Produces an union with fields`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionParamsCase("Red") {
                    Field("a", CommonType.String)
                    Field("b", "int")
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors =
    | Red of a: string * b: int
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union with SingleTextNode`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }
        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union using Widget and escape hatch`` () =
        AnonymousModule() {
            Union("Colors") {
                UnionCase("Red")
                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
                EscapeHatch(UnionCaseNode(None, None, None, SingleTextNode("Black", Range.Zero), [], Range.Zero))
            }
        }
        |> produces
            """

type Colors =
    | Red
    | Green
    | Blue
    | Yellow
    | Black

"""

    [<Test>]
    let ``Produces an union with attribute`` () =
        AnonymousModule() {
            (Union("Colors") { UnionCase("Red") })
                .attributes(AttributeNode "Test")
        }
        |> produces
            """

[<Test>]
type Colors = | Red
"""

    [<Test>]
    let ``Produces an union case with attributes`` () =
        AnonymousModule() {
            (Union("Colors") {
                UnionCase("Red")
                    .attributes(
                        AttributeNodes() {
                            AttributeNode("Obsolete")
                            AttributeNode("Test")

                        }
                    )
            })
                .attributes(AttributeNode "Test")
        }
        |> produces
            """

[<Test>]
type Colors = | [<Obsolete; Test>] Red
"""

module GenericUnion =

    [<Test>]
    let ``Produces an union with TypeParams`` () =
        AnonymousModule() {
            GenericUnion("Colors", [ "'other" ]) {
                UnionParamsCase("Red") {
                    Field("a", CommonType.String)
                    Field("b", CommonType.mkLongIdent("'other"))
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            }
        }

        |> produces
            """

type Colors<'other> =
    | Red of a: string * b: 'other
    | Green
    | Blue
    | Yellow

"""

    [<Test>]
    let ``Produces an union with TypeParams and interface member`` () =
        AnonymousModule() {
            Interface("IMyInterface") {
                AbstractCurriedMethodMember("GetValue", [ CommonType.Unit ], CommonType.String)
            }

            (GenericUnion("Colors", [ "'other" ]) {
                UnionParamsCase("Red") {
                    Field("a", CommonType.String)
                    Field("b", CommonType.mkLongIdent("'other"))
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            })
                .members() {
                InterfaceMember("IMyInterface") {
                    Member("x.GetValue", UnitPat()) { ConstantExpr(ConstantString "\"\"") }
                }
            }
        }

        |> produces
            """
type IMyInterface =
    abstract member GetValue: unit -> string

type Colors<'other> =
    | Red of a: string * b: 'other
    | Green
    | Blue
    | Yellow

    interface IMyInterface with
        member x.GetValue() = ""

"""

    [<Test>]
    let ``Produces an struct union with TypeParams`` () =
        AnonymousModule() {
            (GenericUnion("Colors", [ "'other" ]) {
                UnionParamsCase("Red") {
                    Field("a", CommonType.String)
                    Field("b", CommonType.mkLongIdent("'other"))
                }

                UnionCase("Green")
                UnionCase("Blue")
                UnionCase("Yellow")
            })
                .attributes(AttributeNode "Struct")

        }

        |> produces
            """
[<Struct>]
type Colors<'other> =
    | Red of a: string * b: 'other
    | Green
    | Blue
    | Yellow

"""
