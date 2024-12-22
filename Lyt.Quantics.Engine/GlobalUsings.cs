
#region System + MSFT 

global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Input;

#endregion System 

// This one conflicts with MathNet but is still needed for basic Complex operations 
global using System.Numerics;

global using MathNet.Numerics;
global using MathNet.Numerics.Statistics;
global using MathNet.Numerics.LinearAlgebra;
global using MathNet.Numerics.LinearAlgebra.Complex;

global using Lyt.Quantics.Engine.Core;
global using Lyt.Quantics.Engine.Gates.Base;
global using Lyt.Quantics.Engine.Gates.Unary;
global using Lyt.Quantics.Engine.Gates.Binary;
global using Lyt.Quantics.Engine.Gates.Ternary;
global using Lyt.Quantics.Engine.Machine;
global using Lyt.Quantics.Engine.Matrices;
global using Lyt.Quantics.Engine.Utilities;
