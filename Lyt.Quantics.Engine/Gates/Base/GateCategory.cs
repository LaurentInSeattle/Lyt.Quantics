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

    D_Rotation, // Rx Ry Rz 

    E_BinaryControlled, // CX, Swap, CZ , FCX, CS

    F_Other, // I , 

    G_TernaryControlled,   // CCX,  CSwap , CCZ 

    H_Phase,   // Phase , Later CP CPP ? 

    X_Special, // Barrier (needed?) // P , CP , CCP 
}
