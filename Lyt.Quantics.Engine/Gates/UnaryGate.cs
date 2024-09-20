namespace Lyt.Quantics.Engine.Gates;

public abstract class UnaryGate
{
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

    public abstract Complex[,] Matrix { get; }

    public abstract string Name { get; }
    public abstract string AlternateName { get; }
    public abstract string Caption { get; }
}
