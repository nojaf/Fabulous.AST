namespace Fabulous.AST.Tests.TypeDefinitions

open Fabulous.AST.Tests
open NUnit.Framework

open Fabulous.AST

open type Fabulous.AST.Ast
open type Fantomas.Core.SyntaxOak.Type


module UnitsOfMeasure =

    [<Test>]
    let ``Produces type Unit of measure`` () =
        AnonymousModule() {
            Measure("cm").xmlDocs([ "Cm, centimeters." ])

            Measure("ml", MeasurePower("cm", "3"))
                .xmlDocs([ "Ml, milliliters." ])

            Measure("g").xmlDocs([ "Mass, grams." ])
            Measure("kg").xmlDocs([ "Mass, kilograms." ])
            Measure("lb").xmlDocs([ "Weight, pounds." ])
            Measure("m").xmlDocs([ "Distance, meters." ])
            Measure("cm").xmlDocs([ "Distance, cm" ])
            Measure("inch").xmlDocs([ "Distance, inches." ])
            Measure("ft").xmlDocs([ "Distance, feet" ])
            Measure("s").xmlDocs([ "Time, seconds." ])

            Measure("N", Tuple("kg", "m", MeasurePower([ "s" ], "2")))
                .xmlDocs([ "Force, Newtons." ])

            Measure("bar").xmlDocs([ "Pressure, bar." ])

            Measure("Pa", Tuple("N", MeasurePower([ "m" ], "2")))
                .xmlDocs([ "Pressure, Pascals" ])

            Measure("ml").xmlDocs([ "Volume, milliliters." ])

            Measure("L").xmlDocs([ "Volume, liters." ])

            Measure("Ml", "cm^3")
        }

        |> produces
            """

/// Cm, centimeters.
[<Measure>]
type cm

/// Ml, milliliters.
[<Measure>]
type ml = cm^3

/// Mass, grams.
[<Measure>]
type g

/// Mass, kilograms.
[<Measure>]
type kg

/// Weight, pounds.
[<Measure>]
type lb

/// Distance, meters.
[<Measure>]
type m

/// Distance, cm
[<Measure>]
type cm

/// Distance, inches.
[<Measure>]
type inch

/// Distance, feet
[<Measure>]
type ft

/// Time, seconds.
[<Measure>]
type s

/// Force, Newtons.
[<Measure>]
type N = kg m / s^2

/// Pressure, bar.
[<Measure>]
type bar

/// Pressure, Pascals
[<Measure>]
type Pa = N / m^2

/// Volume, milliliters.
[<Measure>]
type ml

/// Volume, liters.
[<Measure>]
type L

[<Measure>]
type Ml = cm^3

"""
