// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;

/// <summary> Result of Combining QuBit's </summary>
public sealed partial class QuRegister
{
    public void GeneralSwap(int i, int j)
    {
        if (i == j)
        {
            throw new Exception("Invalid swap indices. ! ( i == j )");
        }

        if (i > 0)
        {
            this.Swap(0, i);
        }

        if (j > 1)
        {
            this.Swap(1, j);
        }

        this.Swap(0, 1);

        if (j > 1)
        {
            this.Swap(1, j);
        }

        if (i > 0)
        {
            this.Swap(0, i);
        }

    }

    public void Swap(int i, int j)
    {
        if (i == j)
        {
            throw new Exception("Invalid swap indices. ! ( i == j )");
        }

        if (this.QuBitCount == 2)
        {
            Complex state1 = this.State[1];
            Complex state2 = this.State[2];
            this.State[1] = state2;
            this.State[2] = state1;
        }
        else
        {
            if ( i > j )
            {
                (j, i) = (i, j);
            }

            var preloadedSwaps = SwapData.Swaps(this.QuBitCount, i, j);

            void ProcessStateVector(int from, int to)
            {
                for (int k = from; k < to; ++k)
                {
                    var swap = preloadedSwaps[k];
                    int i1 = swap.Index1;
                    int i2 = swap.Index2;
                    Complex state1 = this.State[i1];
                    Complex state2 = this.State[i2];
                    this.State[i1] = state2;
                    this.State[i2] = state1;
                }
            }

            int swapCount = preloadedSwaps.Count; 
            if (this.QuBitCount >= QuRegister.ThreadedRunAtQubits)
            {
                // Speed up the processing of the state vector using threads (aka tasks) 
                // 1 : Setup
                int taskCount = 4; // Consider: Calculated from Environment.ProcessorCount;
                                    
                int all = swapCount;
                int half = all / 2;
                int quart = half / 2;
                int[] indices = [0, quart, half, half + quart, all];
                var tasks = new Task[taskCount];
                for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
                {
                    int from = indices[taskIndex];
                    int to = indices[1 + taskIndex];
                    var task = new Task(() => ProcessStateVector(from, to));
                    tasks[taskIndex] = task;
                }

                // 2 : Start all tasks
                for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
                {
                    tasks[taskIndex].Start();
                }

                // 3 : Wait for completion 
                Task.WaitAll(tasks);
            }
            else
            {
                ProcessStateVector(from: 0, to: swapCount);
            }
        }
    }

    /// <summary> Apply the provided unary gate at provided position on this quregister </summary>
    public void ApplyUnaryGateAtPosition(Gate gate, int position)
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
            this.Swap(0, position);
        }

        this.ApplyUnaryGateOnQuBitZero(gate);

        if (position > 0)
        {
            this.Swap(0, position);
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
        ControlledGate gate, int positionControl, int positionTarget)
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
        bool cancelledSwapTarget = false;
        bool needToSwap = needToSwapControl || needToSwapTarget;
        if (needToSwap)
        {
            if (needToSwapControl)
            {
                this.Swap(0, positionControl);
            }

            if (needToSwapTarget)
            {
                // Make sure we do not swap twice with cancelling the previous swap 
                if ((positionTarget == 0) && (positionControl == 1))
                {
                    cancelledSwapTarget = true;
                }
                else
                {
                    this.Swap(1, positionTarget);
                } 
            }
        }

        this.ApplyBinaryControlledGateOnQuBitZeroOne(gate);

        if (needToSwap)
        {
            // Must swap in reverse order 
            if (needToSwapTarget && ! cancelledSwapTarget)
            {
                this.Swap(1, positionTarget);
            }

            if (needToSwapControl)
            {
                this.Swap(0, positionControl);
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
            bot.Swap(0, 1);
        }
        else
        {
            throw new Exception("Base gate should be either SWAP or controlled.");
        }

        this.Merge(top, bot);
    }

    /// <summary> Apply the provided ternary controlled gate at the provided positions on this quregister </summary>
    public void ApplyTernaryControlledGateAtPositions(
        ControlledGate gate, int positionControl1, int positionControl2, int positionTarget)
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
                this.Swap(0, minControl);
            }

            if (needToSwapMax)
            {
                this.Swap(1, maxControl);
            }

            if (needToSwapTarget)
            {
                this.Swap(2, positionTarget);
            }
        }

        this.ApplyTernaryControlledGateOnQuBitZeroOneTwo(gate);

        if (needToSwap)
        {
            // Important: Swap back in reverse order 
            if (needToSwapTarget)
            {
                this.Swap(2, positionTarget);
            }

            if (needToSwapMax)
            {
                this.Swap(1, maxControl);
            }

            if (needToSwapMin)
            {
                this.Swap(0, minControl);
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
