using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DataRaceDemo
{
    /// <summary>
    /// 可変な状態 (Mutable State) を持つカウンタークラス。
    /// このクラスのインスタンスは、複数のスレッドから安全にアクセスできません。
    /// </summary>
    public class MutableCounter
    {
        public int Value { get; private set; }

        public MutableCounter()
        {
            Value = 0;
        }

        /// <summary>
        /// カウンターの値を1増やします。
        /// この操作はアトミックではないため、データ競合を引き起こします。
        /// 内部的には「1. 現在の値の読み取り」「2. 読んだ値に1を足す」「3. 結果を書き込む」という
        /// 3ステップで実行され、このステップの間に別のスレッドが割り込む可能性があります。
        /// </summary>
        public void Increment()
        {
            Value++;
        }
    }

    /// <summary>
    /// オブジェクト指向における「共有された参照」と「可変な状態」が
    /// データ競合を引き起こすことを示すテストクラス。
    /// </summary>
    public class SharedMutableStateDataRaceTest
    {
        private readonly ITestOutputHelper _output;

        public SharedMutableStateDataRaceTest(ITestOutputHelper output)
        {
            // テストの実行結果を出力するためのヘルパー
            _output = output;
        }

        [Fact]
        public async Task IncrementingSharedMutableState_CausesDataRace()
        {
            // --- 準備 (Arrange) ---

            // 「共有された参照 (Shared Pointers)」:
            // 'counter' インスタンスはヒープ上に1つだけ存在し、その参照が複数のタスクに共有されます。
            var counter = new MutableCounter();

            const int incrementsPerTask = 100000;
            const int taskCount = 10;
            const int expected = incrementsPerTask * taskCount; // 期待される最終的な値 (1,000,000)

            // --- 実行 (Act) ---

            // 複数のタスク（スレッド）を生成し、それぞれが同じカウンターインスタンスの
            // 「可変な状態 (Mutable State)」を同時に変更しようとします。
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < incrementsPerTask; j++)
                    {
                        counter.Increment();
                    }
                });
            }

            // すべてのタスクが完了するのを待つ
            await Task.WhenAll(tasks);

            // --- 検証 (Assert) ---
            var actual = counter.Value;

            _output.WriteLine($"期待値 (Expected): {expected:N0}");
            _output.WriteLine($"実際の結果 (Actual):   {actual:N0}");
            _output.WriteLine($"失われたカウント: {expected - actual:N0}");

            // データ競合により、Value++ の「読み取り・加算・書き込み」の間に
            // 他のスレッドが割り込むため、インクリメントの一部が失われます。
            // そのため、実際の結果は期待値よりも小さくなります。
            // このアサーションは、期待値と実際の結果が等しくないことを検証しており、
            // データ競合が発生すればテストは「成功」します。
            Assert.NotEqual(expected, actual);
        }
    }
}
