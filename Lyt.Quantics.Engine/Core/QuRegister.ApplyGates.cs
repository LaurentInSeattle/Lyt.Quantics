// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;
using static MathUtilities;

/// <summary> Result of Combining QuBit's </summary>
public sealed partial class QuRegister
{
    private List<Tuple<int, int>> swaps = new(1024);
    private HashSet<ulong> processedSwaps = new(1024);

    /// <summary> This should perform just like applying a binary swap gate on qubits indices i and j </summary>
    /// <remarks> Assumes that i < j </remarks>
    public void Swap(KetMap ketMap, int i, int j)
    {
        KetMap reducedKetMap = ketMap.Reduce(i, j);
        
        // This is looping over the entire state vector and truly is where there's a need to go as fast as possible 
        void ProcessStateVector(int from, int to, bool withLocks)
        {
            for (int k1 = from; k1 < to; ++k1)
            {
                // for the state at k index: 
                // if bit #i is not equal to bit #j, we need to swap values 
                // The condition is equivalent to: 
                // if bit #i is set an bit #j is not set 
                // OR 
                // if bit #j is set an bit #i is not set 
                // 
                // WAS: bool areDifferent = ketMap.Get(k1, i) ^ ketMap.Get(k1, j);
                var fastMapAtK1 = ketMap.FastMapBits[k1];
                bool areDifferent = fastMapAtK1[i] ^ fastMapAtK1[j];
                if (areDifferent)
                {
                    // This index k1 needs to be swapped with another one, k2, so... Find k2 
                    int k2 = ketMap.SwapMatch(reducedKetMap, k1, i, j);
                    if (withLocks)
                    {
                        lock (this.swaps)
                        {
                            this.swaps.Add(new(k1, k2));
                        }
                    } 
                    else
                    {
                        this.swaps.Add(new(k1, k2));
                    }
                }
            }
        }

        this.swaps.Clear();
        this.processedSwaps.Clear();

        if (this.QuBitCount >= QuRegister.ThreadedRunAtQubits)
        {
            // Speed up the processing of the state vector using threads (aka tasks) 
            // 1 : Setup
            int count = 4; // Consider: Environment.ProcessorCount;  
            int all = this.State.Count;
            int half = all / 2;
            int quart = half / 2;
            int[] indices = [0, quart, half, half + quart, all];
            Task[] tasks = new Task[count];
            for (int taskIndex = 0; taskIndex < count; ++taskIndex)
            {
                int from = indices[taskIndex];
                int to = indices[1 + taskIndex];
                var task = new Task(() => ProcessStateVector(from, to, withLocks:true));
                tasks[taskIndex] = task;
            }

            // 2 : Start all tasks
            for (int taskIndex = 0; taskIndex < count; ++taskIndex)
            {
                tasks[taskIndex].Start();
            }

            // 3 : Wait for completion 
            Task.WaitAll(tasks);
        }
        else
        {
            ProcessStateVector(from: 0, to: this.State.Count, withLocks: false);
        }

        foreach (var swap in this.swaps)
        {
            int i1 = swap.Item1;
            int i2 = swap.Item2;
            ulong swapKey = (uint)(i2 << 32) + (uint)i1;
            if (this.processedSwaps.Contains(swapKey))
            {
                continue;
            }

            swapKey = (uint)(i1 << 32) + (uint)i2;
            this.processedSwaps.Add(swapKey);

#if VERBOSE
            Debug.WriteLine(i1 + " <-> " + i2);
#endif // VERBOSE            

            Complex state1 = this.State[i1];
            Complex state2 = this.State[i2];
            this.State[i1] = state2;
            this.State[i2] = state1;
        }
    }

    /// <summary> Apply the provided unary gate at provided position on this quregister </summary>
    public void ApplyUnaryGateAtPosition(Gate gate, KetMap ketMap, int position)
    {
        if (!gate.IsUnary)
        {
            throw new Exception("Gate should be unary.");
        }

        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        if ((position < 0) || (position >= quBitCount))
        {
            throw new ArgumentException("Invalid position");
        }

        if (position > 0)
        {
            this.Swap(ketMap, 0, position);
        }

        this.ApplyUnaryGateOnQuBitZero(gate);

        if (position > 0)
        {
            this.Swap(ketMap, 0, position);
        }
    }

    /// <summary> Same as above for position zero </summary>
    /// <remarks> Made public for unit tests</remarks>
    public void ApplyUnaryGateOnQuBitZero(Gate gate)
    {
        if (!gate.IsUnary)
        {
            throw new Exception("Gate should be unary.");
        }

        var matrix = gate.Matrix;
        Vector<Complex> subState = Vector<Complex>.Build.Dense(2);
        int half = this.State.Count / 2;
        for (int k = 0; k < half; ++k)
        {
            subState.At(0, this.State[k]);
            subState.At(1, this.State[k + half]);
            subState = matrix.Multiply(subState);
            this.state[k] = subState.At(0);
            this.state[k + half] = subState.At(1);
        }
    }

    /// <summary> Apply the provided binary controlled gate at the provided positions on this quregister </summary>
    public void ApplyBinaryControlledGateAtPositions(
        ControlledGate gate, KetMap ketMap, int positionControl, int positionTarget)
    {
        if (!gate.IsBinary)
        {
            throw new Exception("Gate should be binary and controlled.");
        }

        var baseGate = gate.BaseGate;
        if (!baseGate.IsUnary)
        {
            throw new Exception("Base gate should be unary.");
        }

        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        if ((positionControl < 0) || (positionControl >= quBitCount))
        {
            throw new ArgumentException("Invalid control position");
        }

        if ((positionTarget < 0) || (positionTarget >= quBitCount))
        {
            throw new ArgumentException("Invalid target position");
        }

        if (positionTarget == positionControl)
        {
            throw new ArgumentException("Invalid positions");
        }

        bool needToSwapControl = positionControl != 0;
        bool needToSwapTarget = positionTarget != 1;
        bool needToSwap = needToSwapControl || needToSwapTarget;
        if (needToSwap)
        {
            if (needToSwapControl)
            {
                this.Swap(ketMap, 0, positionControl);
            }

            if (needToSwapTarget)
            {
                this.Swap(ketMap, 1, positionTarget);
            }
        }

        this.ApplyBinaryControlledGateOnQuBitZeroOne(gate);

        if (needToSwap)
        {
            // Must swap in reverse order 
            if (needToSwapTarget)
            {
                this.Swap(ketMap, 1, positionTarget);
            }

            if (needToSwapControl)
            {
                this.Swap(ketMap, 0, positionControl);
            }
        }
    }

    /// <summary> Apply the provided binary controlled gate at positions zero and one on this quregister </summary>
    /// <remarks> Made public for unit tests</remarks>
    public void ApplyBinaryControlledGateOnQuBitZeroOne(ControlledGate gate)
    {
        if (!gate.IsBinary)
        {
            throw new Exception("Gate should be binary and controlled.");
        }

        var baseGate = gate.BaseGate;
        if (!baseGate.IsUnary)
        {
            throw new Exception("Base gate should be unary.");
        }

        var tuple = this.Split();
        var top = tuple.Item1;
        var bot = tuple.Item2;
        bot.ApplyUnaryGateOnQuBitZero(baseGate);
        this.Merge(top, bot);
    }

    /// <summary> Apply the provided ternary controlled gate at positions zero one and two on this quregister </summary>
    /// <remarks> Made public for unit tests</remarks>
    public void ApplyTernaryControlledGateOnQuBitZeroOneTwo(ControlledGate gate)
    {
        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        if (quBitCount < 3)
        {
            throw new Exception("Invalid qubit count.");
        }

        if (!gate.IsTernary)
        {
            throw new Exception("Gate should be ternary and controlled.");
        }

        var baseGate = gate.BaseGate;
        if (!baseGate.IsBinary)
        {
            throw new Exception("Base gate should be binary.");
        }

        // Split the state vector and apply the base gate on the lower half, then merge it back 
        var tuple = this.Split();
        var top = tuple.Item1;
        var bot = tuple.Item2;
        if (baseGate is ControlledGate controlledBaseGate)
        {
            bot.ApplyBinaryControlledGateOnQuBitZeroOne(controlledBaseGate);
        }
        else if (baseGate is SwapGate)
        {
            var halfKetMap = new KetMap(bot.QuBitCount);
            bot.Swap(halfKetMap, 0, 1);
        }
        else
        {
            throw new Exception("Base gate should be either SWAP or controlled.");
        }

        this.Merge(top, bot);
    }

    /// <summary> Apply the provided ternary controlled gate at the provided positions on this quregister </summary>
    public void ApplyTernaryControlledGateAtPositions(
        ControlledGate gate, KetMap ketMap, int positionControl1, int positionControl2, int positionTarget)
    {
        if (!gate.IsTernary)
        {
            throw new Exception("Gate should be ternary.");
        }

        var baseGate = gate.BaseGate;
        if (!baseGate.IsBinary)
        {
            throw new Exception("Base gate should be binary.");
        }

        if ((baseGate is not ControlledGate) && (baseGate is not SwapGate))
        {
            throw new Exception("Base gate should be either SWAP or controlled.");
        }

        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        if ((positionControl1 < 0) || (positionControl1 >= quBitCount))
        {
            throw new ArgumentException("Invalid control position");
        }

        if ((positionControl2 < 0) || (positionControl2 >= quBitCount))
        {
            throw new ArgumentException("Invalid control position");
        }

        if ((positionTarget < 0) || (positionTarget >= quBitCount))
        {
            throw new ArgumentException("Invalid target position");
        }

        if ((positionTarget == positionControl1) ||
            (positionTarget == positionControl2) ||
            (positionControl1 == positionControl2))
        {
            throw new ArgumentException("Invalid positions");
        }

        int minControl = Math.Min(positionControl1, positionControl2);
        int maxControl = Math.Max(positionControl1, positionControl2);
        bool needToSwapMin = minControl != 0;
        bool needToSwapMax = maxControl != 1;
        bool needToSwapTarget = positionTarget != 2;
        bool needToSwap = needToSwapMin || needToSwapMax || needToSwapTarget;
        if (needToSwap)
        {
            if (needToSwapMin)
            {
                this.Swap(ketMap, minControl, 0);
            }

            if (needToSwapMax)
            {
                this.Swap(ketMap, maxControl, 1);
            }

            if (needToSwapTarget)
            {
                this.Swap(ketMap, positionTarget, 2);
            }
        }

        this.ApplyTernaryControlledGateOnQuBitZeroOneTwo(gate);

        if (needToSwap)
        {
            // Important: Swap back in reverse order 
            if (needToSwapTarget)
            {
                this.Swap(ketMap, positionTarget, 2);
            }

            if (needToSwapMax)
            {
                this.Swap(ketMap, maxControl, 1);
            }

            if (needToSwapMin)
            {
                this.Swap(ketMap, minControl, 0);
            }
        }
    }

    /// <summary> Split this register into two halves. </summary>
    /// <returns> A tuple containing two registers, top and bottom halves. </returns>
    private Tuple<QuRegister, QuRegister> Split()
    {
        int halfCount = this.state.Count / 2;
        Vector<Complex> topStateVector = Vector<Complex>.Build.Dense(halfCount);
        Vector<Complex> botStateVector = Vector<Complex>.Build.Dense(halfCount);
        for (int i = 0; i < halfCount; ++i)
        {
            topStateVector.At(i, this.state[i]);
            botStateVector.At(i, this.state[i + halfCount]);
        }

        return new Tuple<QuRegister, QuRegister>(new(topStateVector), new(botStateVector));
    }

    /// <summary> Update this register by merging the two provided halves. </summary>
    private void Merge(QuRegister top, QuRegister bot)
    {
        int halfCount = this.state.Count / 2;
        for (int i = 0; i < halfCount; ++i)
        {
            this.state[i] = top.state.At(i);
            this.state[i + halfCount] = bot.state.At(i);
        }
    }
}
