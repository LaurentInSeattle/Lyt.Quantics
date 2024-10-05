namespace Lyt.Quantics.Engine.Gates.Base;

// Gate categories in the QRyd simulator 
// (Somewhat refined)
// 
public enum GateCategory
{
    Red, // H , T , T dagger

    Blue, // CNOT, Swap, CZ 

    Green, // X Y Z , Sqrt X , Sqrt Z

    DarkGreen, // Sqrt X (== SX) , Sqrt Z (== S)

    Purple,  // CCX,  CSwap , CCZ 

    DarkRed , // Rx Ry Rz

    DarkPurple, // P , CP , CCP 

    Gray, // I , Barrier
}
