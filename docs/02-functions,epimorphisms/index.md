# 圏論学習ノート (C#)

## 関数 (Functions) と 射 (Morphisms)

参考動画: [Category Theory for Programmers 2.1: Functions, epimorphisms](https://www.youtube.com/watch?v=O2lZkr-aAqk&t=33s)

圏論における「射 (Morphism)」は、集合の圏 (Set) においては「関数 (Function)」として理解できます。
しかし、プログラミングにおける関数と、数学（圏論）における関数には重要な違いがあります。圏論として扱うためには、以下の性質を理解する必要があります。

### 1. 純粋関数 (Pure Functions)

数学的な関数は、入力と出力の関係のみを表します。

-   **同じ入力には常に同じ出力を返す (Deterministic)**
-   **副作用 (Side Effects) がない**: 外部の状態を変えたり、外部の状態に依存したりしない。

C# のような言語では、`static` メソッドであってもグローバルな状態にアクセスすれば純粋ではありません。

### 2. 全域関数 (Total Functions)

数学的な関数は、定義域 (Domain) の**すべての**要素に対して値が定義されていなければなりません。これを **全域関数 (Total Function)** と呼びます。

対して、一部の入力に対して値が定義されない（例外を投げる、無限ループするなど）関数を **部分関数 (Partial Function)** と呼びます。

#### 部分関数の例
```csharp
// 0 で割ると例外が発生するため、int 全体に対して定義されていない
int Divide(int a, int b) => a / b;
```

#### 全域関数への変換
部分関数を全域関数として扱うための一般的なアプローチは2つあります。

1.  **定義域を制限する**: ゼロを含まない型 `NonZeroInt` を定義し、それを引数にする。
2.  **値域を拡張する**: 戻り値を `int` ではなく、失敗の可能性を含む型（`int?` や `Option<int>`）にする。

---

## 実装デモ: FunctionsDemo

純粋関数・全域関数の概念を確認するための xUnit テストプロジェクトです。

### プロジェクト構成

-   `FunctionTests.cs`: 以下のテストを含みます。
    -   `Pure_vs_Impure`: 純粋関数と非純粋関数の挙動の違い。
    -   `Partial_vs_Total`: 部分関数（例外発生）と、それを全域関数化した例。

### コード例

#### 純粋関数 (Pure)
```csharp
public static int Add(int a, int b) => a + b;
```

#### 全域関数化 (Total)
```csharp
public static int? SafeDivide(int a, int b)
{
    if (b == 0) return null; // 値域を拡張して全域で定義する
    return a / b;
}
```

---

## WIP補足: 関係 (Relation) と関数の厳密な定義

- **関係 (Relation)**: 2 つの集合の要素同士の関係は、直積 (Cartesian product) の部分集合として定義できます。
    つまり、関係 R は $S_1\times S_2$ の部分集合で、要素はペア $(a,b)$ です。
- **直積 (Cartesian product)**: $S_1\times S_2=\{(a,b)\mid a\in S_1,\;b\in S_2\}$。
- **関数 (Function)**: 関係のうち、各入力 $a\in S_1$ に対して対応する出力が「高々1つ」だけ存在するものを関数と呼びます（これが関数の functional property）。
    - もしある $a$ に複数の $b\in S_2$ が対応するなら、それは関数ではありません（単なる関係です）。
- **全域関数 (Total function)**: さらに各 $a\in S_1$ にちょうど1つの像が存在する場合を全域関数と呼びます（定義域のすべてに定義される）。
    - これに対して、ある入力に像が存在しないことを許すと部分関数 (Partial function) になります。
- **Domain / Codomain / Image**:
    - Domain: 出発集合（例: $S_1$）。
    - Codomain: 目標集合（例: $S_2$）。
    - Image (像): 実際に現れる値の集合 $\mathrm{Im}(f)=\{f(a)\mid a\in S_1\}\subseteq S_2$。
- **逆写像と合成**:
    - $f:A\to B$ に対して $g:B\to A$ が逆写像であるためには、両方の合成が恒等写像になる必要があります:
        $g\circ f=\mathrm{id}_A$ かつ $f\circ g=\mathrm{id}_B$。
    - 片側だけ成り立つ場合は左逆（$g\circ f=\mathrm{id}_A$）や右逆（$f\circ g=\mathrm{id}_B$）と呼び、左逆があると injective、右逆があると surjective になります。
- **同型 (Isomorphism)**: 集合圏では、逆写像が存在する（双方向の合成が恒等写像になる）写像は双射（bijection）であり、同型と呼ばれます。

上の違い（関係 ⇄ 関数、部分関数 ⇄ 全域関数、逆写像の片側だけの性質）は圏論やプログラミングで扱う際に重要な直感と厳密性を与えます。
