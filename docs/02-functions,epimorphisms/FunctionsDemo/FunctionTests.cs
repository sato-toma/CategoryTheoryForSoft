using System;
using Xunit;

namespace FunctionsDemo;

public class FunctionTests
{
    // ==========================================
    // 1. Pure Functions vs Impure Functions
    // ==========================================

    // 純粋関数: 同じ入力に対して常に同じ出力を返す。副作用がない。
    public static int PureAdd(int a, int b) => a + b;

    // 非純粋関数: 外部状態(state)に依存するため、同じ入力でも結果が変わる。
    private static int _state = 0;
    public static int ImpureAdd(int a) => a + _state++;

    [Fact]
    public void PureFunction_AlwaysReturnsSameOutputForSameInput()
    {
        // 何度呼んでも結果は同じ (参照透過性)
        Assert.Equal(5, PureAdd(2, 3));
        Assert.Equal(5, PureAdd(2, 3));
        Assert.Equal(5, PureAdd(2, 3));
    }

    [Fact]
    public void ImpureFunction_ReturnsDifferentOutputForSameInput()
    {
        // 外部状態 _state が変わるため、結果が変わる
        _state = 0; // Reset state
        Assert.Equal(0, ImpureAdd(0)); // 0 + 0
        Assert.Equal(1, ImpureAdd(0)); // 0 + 1
        Assert.Equal(2, ImpureAdd(0)); // 0 + 2
    }

    // ==========================================
    // 2. Total Functions vs Partial Functions
    // ==========================================

    // 部分関数: 定義域の一部の値(b=0)に対して定義されていない(例外を投げる)。
    public static int PartialDivide(int a, int b) => a / b;

    // 全域関数: すべての入力に対して値を返す(nullを返すことで定義する)。
    // 値域を int から int? (Option<int>) に拡張している。
    public static int? TotalDivide(int a, int b)
    {
        if (b == 0) return null;
        return a / b;
    }

    [Fact]
    public void PartialFunction_ThrowsException_OnUndefinedInput()
    {
        // 定義域内の値
        Assert.Equal(5, PartialDivide(10, 2));

        // 定義されていない値 (0)
        Assert.Throws<DivideByZeroException>(() => PartialDivide(10, 0));
    }

    [Fact]
    public void TotalFunction_ReturnsValue_OnAllInputs()
    {
        // 通常のケース
        Assert.Equal(5, TotalDivide(10, 2));

        // 0 の場合も例外ではなく null (値なし) という「値」を返す
        Assert.Null(TotalDivide(10, 0));
    }
}
