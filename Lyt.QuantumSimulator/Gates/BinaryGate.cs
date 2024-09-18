namespace Lyt.QuantumSimulator.Gates;

public abstract class BinaryGate
{
    public void Apply(Qubit q1, Qubit q2)
    {
        Qubit.Combine(q1, q2);
        q1.State.ApplyBinaryGate(this, q1.Id, q2.Id);
    }

    public Complex[,] GetMatrix(int bitLen, int bit1Pos, int bit2Pos)
    {
        var matrix = this.GetMatrix();
        var table = AlgebraUtility.LookupTable(matrix);

        int mLen = 1 << bitLen;
        matrix = new Complex[mLen, mLen];

        for (int i = 0; i < mLen; i++)
        {
            int x =
                (BinaryUtility.IsBitSet(i, bit1Pos) ? 2 : 0) +
                (BinaryUtility.IsBitSet(i, bit2Pos) ? 1 : 0);

            foreach (var y in table[x])
            {
                int  j = BinaryUtility.SetBitValue( i, bit1Pos, BinaryUtility.IsBitSet(y.Key, 1) );
                j = BinaryUtility.SetBitValue( j, bit2Pos, BinaryUtility.IsBitSet(y.Key, 0));

                matrix[i, j] = y.Value;
                matrix[j, i] = y.Value;
            }
        }

        return matrix;
    }

    protected abstract Complex[,] GetMatrix();
}
