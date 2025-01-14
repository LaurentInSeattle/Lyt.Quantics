namespace Lyt.Quantics.Engine.Gates.Base;

// Gate categories similar to those in the QRyd simulator (Somewhat refined)
// 
// Categories are prefixed by numerical ical order in the UI toolbox

public enum GateCategory : int 
{
    HadamardAndT, // H , T , T dagger

    Pauli, // X Y Z , 

    Phase, // Sqrt X (== SX) , Sqrt Z (== S)  S dagger

    Rotation, // Rx Ry Rz 

    PhaseParametrized,   // Phase , Later CP CPP ? 

    BinaryControlled, // CX, Swap, CZ , FCX, CS

    Other, // I , 

    TernaryControlled,   // CCX,  CSwap , CCZ 

    Special, // Barrier (needed?) // P , CP , CCP 
}
