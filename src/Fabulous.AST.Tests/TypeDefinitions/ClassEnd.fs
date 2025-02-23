namespace Fabulous.AST.Tests.TypeDefinitions

open Fabulous.AST.Tests
open NUnit.Framework

open Fabulous.AST
open type Ast

module ClassEnd =
    [<Test>]
    let ``Produces a class end`` () =
        AnonymousModule() { ClassEnd("MyClass") }
        |> produces
            """
type MyClass = class end
            """

    [<Test>]
    let ``Produces a class end with constructor`` () =
        AnonymousModule() { ClassEnd("MyClass", true) }
        |> produces
            """
type MyClass () = class end
            """

    [<Test>]
    let ``Produces a class end with constructor and attributes`` () =
        AnonymousModule() {
            ClassEnd("MyClass", true)
                .attributes(
                    AttributeNodes() {
                        AttributeNode("Sealed")
                        AttributeNode("AbstractClass")
                    }
                )
        }
        |> produces
            """
[<Sealed; AbstractClass>]
type MyClass () = class end
            """

    [<Test>]
    let ``Produces a class end with constructor params`` () =
        AnonymousModule() {
            ClassEnd("MyClass", ImplicitConstructor() { SimplePat("name", CommonType.String, false) })
                .attributes(
                    AttributeNodes() {
                        AttributeNode("Sealed")
                        AttributeNode("AbstractClass")
                    }
                )
        }
        |> produces
            """
[<Sealed; AbstractClass>]
type MyClass (name: string) = class end
            """

    [<Test>]
    let ``Produces a class end with constructor params and type args`` () =
        AnonymousModule() {
            ClassEnd("MyClass", [ "'a" ], ImplicitConstructor() { SimplePat("name", CommonType.String, false) })
                .attributes(
                    AttributeNodes() {
                        AttributeNode("Sealed")
                        AttributeNode("AbstractClass")
                    }
                )
        }
        |> produces
            """
[<Sealed; AbstractClass>]
type MyClass <'a>(name: string) = class end
            """

    [<Test>]
    let ``Produces a class end with type params`` () =
        AnonymousModule() { ClassEnd("MyClass", [ "'a"; "'b" ]) }
        |> produces
            """
type MyClass <'a, 'b> = class end
            """

    [<Test>]
    let ``Produces a class end with constructor and  type params`` () =
        AnonymousModule() { ClassEnd("MyClass", [ "'a"; "'b" ], true) }
        |> produces
            """
type MyClass <'a, 'b>() = class end
            """
