namespace Fabulous.AST.Tests.Core

open System
open Fabulous.AST.StackAllocatedCollections
open NUnit.Framework


open type Fabulous.AST.Ast

module ArrayTests =

    [<Test>]
    let ``MutStackArray1.combineMut reuses array B if can fit all data`` () =
        let arrB = Array.zeroCreate 7

        let a = MutStackArray1.Many((2us, Array.zeroCreate 4))
        let b = MutStackArray1.Many((5us, arrB))
        let c = MutStackArray1.combineMut(&a, b)
        let cOpt = MutStackArray1.toArraySlice &c
        let struct (usedC, arrC) = cOpt.Value

        // We should have the same number of used items
        Assert.AreEqual(7us, usedC)

        // Reference should be equal to arrB since the array was reused
        Assert.True(Object.ReferenceEquals(arrC, arrB))
