namespace Lyt.Quantics.Engine.Gates.Base;

using MathNet.Numerics.LinearAlgebra;

public abstract class Gate
{
    // See: https://en.wikipedia.org/wiki/List_of_quantum_logic_gates 

    // TODO: Core
    // Introduce gate parameters 
    // S Dagger gate 
    // Custom Phase Gate 
    // See: https://en.wikipedia.org/wiki/Quantum_logic_gate#Controlled_gates 
    //      Controlled Phase 
    //      Controlled Controlled Phase 
    //      Controlled Swap 
    //      Controlled Controlled Z

    // TODO: UI 
    // Assign Colors to gates 
    // Gate Icons (SVG) : https://commons.wikimedia.org/w/index.php?title=Special:ListFiles&user=Geek3

    public abstract Matrix<Complex> Matrix { get; }

    public int Length => this.Matrix.RowCount;

    public abstract string Name { get; }

    public abstract string AlternateName { get; }
    
    public abstract string CaptionKey { get; }
}
