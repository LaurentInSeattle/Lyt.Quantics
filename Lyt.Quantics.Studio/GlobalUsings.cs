
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
global using Lyt.Avalonia.Interfaces.Logger;
global using Lyt.Avalonia.Interfaces.Messenger;
global using Lyt.Avalonia.Interfaces.Model;
global using Lyt.Avalonia.Interfaces.Profiler;
global using Lyt.Avalonia.Interfaces.Random;
global using Lyt.Avalonia.Interfaces.UserInterface;

global using Lyt.Avalonia.Controls;
global using Lyt.Avalonia.Controls.Glyphs;
global using Lyt.Avalonia.Controls.Logging;
global using Lyt.Avalonia.Controls.Toasting;

global using Lyt.Avalonia.Mvvm;
global using Lyt.Avalonia.Mvvm.Animations;
global using Lyt.Avalonia.Mvvm.Behaviors;
global using Lyt.Avalonia.Mvvm.Core;
global using Lyt.Avalonia.Mvvm.Dialogs;
global using Lyt.Avalonia.Mvvm.Extensions;
global using Lyt.Avalonia.Mvvm.Interfaces.Animations;
global using Lyt.Avalonia.Mvvm.Messenger;
global using Lyt.Avalonia.Mvvm.Utilities;

global using Lyt.Avalonia.Localizer;
global using Lyt.Avalonia.Model;
global using Lyt.Avalonia.Persistence;

global using Lyt.Search;

#endregion Framework 

#region Engine 

global using Lyt.Quantics.Engine;
global using Lyt.Quantics.Engine.Core;
global using Lyt.Quantics.Engine.Gates;
global using Lyt.Quantics.Engine.Gates.Base;
global using Lyt.Quantics.Engine.Gates.Unary;
global using Lyt.Quantics.Engine.Gates.Binary;
global using Lyt.Quantics.Engine.Gates.Ternary;
global using Lyt.Quantics.Engine.Machine;
global using Lyt.Quantics.Engine.Utilities;

#endregion Engine 

#region Studio

global using Lyt.Quantics.Studio;
global using Lyt.Quantics.Studio.Behaviors;
global using Lyt.Quantics.Studio.Behaviors.DragDrop;
global using Lyt.Quantics.Studio.Controls;
global using Lyt.Quantics.Studio.Controls.Histogram;
global using Lyt.Quantics.Studio.Messaging;
global using Lyt.Quantics.Studio.Model;
global using Lyt.Quantics.Studio.Shell;
//global using Lyt.Quantics.Studio.Utilities;
global using Lyt.Quantics.Studio.Workflow.Intro;
global using Lyt.Quantics.Studio.Workflow.Load;
global using Lyt.Quantics.Studio.Workflow.Load.Tiles;
global using Lyt.Quantics.Studio.Workflow.Load.Toolbars;
global using Lyt.Quantics.Studio.Workflow.Run;
global using Lyt.Quantics.Studio.Workflow.Run.Code;
global using Lyt.Quantics.Studio.Workflow.Run.Computer;
global using Lyt.Quantics.Studio.Workflow.Run.Dialogs;
global using Lyt.Quantics.Studio.Workflow.Run.Gates;
global using Lyt.Quantics.Studio.Workflow.Run.Gates.Special;
global using Lyt.Quantics.Studio.Workflow.Run.Amplitudes;
global using Lyt.Quantics.Studio.Workflow.Save;

#endregion Studio