
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

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

#endregion System 

#region Avalonia 

global using Avalonia;
global using Avalonia.Controls;
global using Avalonia.Controls.ApplicationLifetimes;
global using Avalonia.Controls.Shapes;
global using Avalonia.Data;
global using Avalonia.Data.Converters;
global using Avalonia.Data.Core.Plugins;
global using Avalonia.Input;
global using Avalonia.Input.Platform;
global using Avalonia.Interactivity;
global using Avalonia.Layout;
global using Avalonia.Markup.Xaml;
global using Avalonia.Markup.Xaml.Styling;
global using Avalonia.Media;
global using Avalonia.Media.Imaging;
global using Avalonia.Media.Immutable;
global using Avalonia.Platform;
global using Avalonia.Threading;

#endregion Avalonia 

#region Framework 

global using Lyt.Avalonia.Interfaces;
global using Lyt.Avalonia.Interfaces.UserInterface;

global using Lyt.Avalonia.Controls;
global using Lyt.Avalonia.Controls.Countdown;
global using Lyt.Avalonia.Controls.Glyphs;

global using Lyt.Avalonia.Mvvm;
global using Lyt.Avalonia.Mvvm.Animations;
global using Lyt.Avalonia.Mvvm.Behaviors;
global using Lyt.Avalonia.Mvvm.Core;
global using Lyt.Avalonia.Mvvm.Dialogs;
global using Lyt.Avalonia.Mvvm.Input;
global using Lyt.Avalonia.Mvvm.Logging;
global using Lyt.Avalonia.Mvvm.Toasting;
global using Lyt.Avalonia.Mvvm.Utilities;

global using Lyt.Framework.Interfaces.Binding;
global using Lyt.Framework.Interfaces.Dispatching;
global using Lyt.Framework.Interfaces.Logging;
global using Lyt.Framework.Interfaces.Messaging;
global using Lyt.Framework.Interfaces.Modeling;
global using Lyt.Framework.Interfaces.Profiling;
global using Lyt.Framework.Interfaces.Randomizing;

global using Lyt.Mvvm;
global using Lyt.Model;
global using Lyt.Persistence;
global using Lyt.Messaging;
global using Lyt.Search;
global using Lyt.Utilities.Extensions;
global using Lyt.Utilities.Profiling;
global using Lyt.Utilities.Randomizing;
global using Lyt.Validation;

#endregion Framework 

#region Engine 

global using Lyt.Quantics.Engine;
global using Lyt.Quantics.Engine.Core;
global using Lyt.Quantics.Engine.Gates;
global using Lyt.Quantics.Engine.Gates.Base;
global using Lyt.Quantics.Engine.Gates.Unary;
global using Lyt.Quantics.Engine.Gates.UnaryParametrized;
global using Lyt.Quantics.Engine.Gates.Binary;
global using Lyt.Quantics.Engine.Gates.Ternary;
global using Lyt.Quantics.Engine.Machine;
global using Lyt.Quantics.Engine.Utilities;

#endregion Engine 

global using FluentValidation;

#region Studio

global using Lyt.Quantics.Studio.Behaviors;
global using Lyt.Quantics.Studio.Behaviors.DragDrop;
global using Lyt.Quantics.Studio.Controls;
global using Lyt.Quantics.Studio.Controls.Histogram;
global using Lyt.Quantics.Studio.Messaging;
global using Lyt.Quantics.Studio.Model;
global using Lyt.Quantics.Studio.Model.Messaging;
global using Lyt.Quantics.Studio.Shell;
global using Lyt.Quantics.Studio.Utilities;

global using Lyt.Quantics.Studio.Workflow.Intro;

global using Lyt.Quantics.Studio.Workflow.Load;
global using Lyt.Quantics.Studio.Workflow.Load.Tiles;
global using Lyt.Quantics.Studio.Workflow.Load.Toolbars;

global using Lyt.Quantics.Studio.Workflow.Run;
global using Lyt.Quantics.Studio.Workflow.Run.Amplitudes;
global using Lyt.Quantics.Studio.Workflow.Run.Code;
global using Lyt.Quantics.Studio.Workflow.Run.Computer;
global using Lyt.Quantics.Studio.Workflow.Run.Dialogs;
global using Lyt.Quantics.Studio.Workflow.Run.Gates;
global using Lyt.Quantics.Studio.Workflow.Run.Gates.Special;
global using Lyt.Quantics.Studio.Workflow.Run.Toolbox;

#endregion Studio