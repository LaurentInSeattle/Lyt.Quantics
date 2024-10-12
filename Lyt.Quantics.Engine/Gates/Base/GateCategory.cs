namespace Lyt.Quantics.Engine.Gates.Base;

// Gate categories in the QRyd simulator 
// (Somewhat refined)
// 
// Categories are prefixed by alphebetical order in the UI toolbox
public enum GateCategory
{
    A_HadamardAndT, // H , T , T dagger

    B_Pauli, // X Y Z , 

    C_Phase, // Sqrt X (== SX) , Sqrt Z (== S)  S dagger

    D_BinaryControlled, // CX, Swap, CZ , FCX, CS

    E_Other, // I , 

    F_TernaryControlled,   // CCX,  CSwap , CCZ 

    X_Special, // Barrier (needed?) // Rx Ry Rz  // P , CP , CCP 
}
