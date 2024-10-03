namespace Lyt.Quantics.Engine.Gates.Base;

using MathNet.Numerics.LinearAlgebra;

public abstract class Gate
{
    // TODO: Core
    // T Dagger gate 
    // S Dagger gate 

    // TODO: UI 
    // Assign Colors to gates 
    // Gate Icons (SVG) : https://commons.wikimedia.org/w/index.php?title=Special:ListFiles&user=Geek3


    //public Complex[,] GetMatrix(int bitLen, int bitPos)
    //{
    //    var matrix = this.GetMatrix();

    //    if (bitLen > 1)
    //    {
    //        var table = AlgebraUtility.LookupTable(matrix);
    //        int mLen = 1 << bitLen;
    //        matrix = new Complex[mLen, mLen];

    //        for (int i = 0; i < mLen; i++)
    //        {
    //            int x = BinaryUtility.IsBitSet(i, bitPos) ? 1 : 0;
    //            foreach (var y in table[x])
    //            {
    //                int j = BinaryUtility.SetBitValue(i, bitPos, BinaryUtility.IsBitSet(y.Key, 0));

    //                matrix[i, j] = y.Value;
    //                matrix[j, i] = y.Value;
    //            }
    //        }
    //    }

    //    return matrix;
    //}

    public abstract Matrix<Complex> Matrix { get; }

    public int Length => this.Matrix.RowCount;

    public abstract string Name { get; }

    public abstract string AlternateName { get; }
    
    public abstract string CaptionKey { get; }
}
