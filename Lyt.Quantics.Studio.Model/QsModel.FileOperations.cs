﻿namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;
using static FileManagerModel;

public sealed partial class QsModel : ModelBase
{
    public bool CreateBlank(out string message)
    {
        try
        {
            // Initialize a 'blank' computer , starting with two (empty) qubits 
            this.QuComputer = new("Untitled", "New quantum computer project.");
            bool status = this.QuComputer.AddQubit(0, out message);
            if (status)
            {
                status = this.QuComputer.AddQubit(1, out message);
                if (status)
                {
                    this.IsDirty = false;
                    this.Messenger.Publish(MakeModelLoaded());
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create New: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool CreateFromResource(string computerName, out string message)
    {
        try
        {
            message = string.Empty;
            var builtInComputers = QsModel.BuiltInComputers;
            var sourceComputer = builtInComputers[computerName];
            var computer = sourceComputer.DeepClone();
            this.QuComputer = computer;
            bool status = this.QuComputer.Validate(out message);
            if (status)
            {
                this.IsDirty = false;
                this.Messenger.Publish(MakeModelLoaded());
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create From Resource: Exception thrown: \n" + ex.Message;
            return false;
        }
    }

    public bool CreateFromDocument(QuComputer? computer, out string message)
    {
        try
        {
            if (computer is null)
            {
                throw new Exception("Failed to provide a QuComputer object");
            }

            bool isValid = computer.Validate(out message);
            if (!isValid)
            {
                throw new Exception(message);
            }

            bool isBuilt = computer.Build(out message);
            if (!isBuilt)
            {
                throw new Exception(message);
            }

            this.QuComputer = computer;
            this.IsDirty = false;
            this.Messenger.Publish(MakeModelLoaded());
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create from Document: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool ValidateComputerMetadata(
        string name, string description, bool withOverwrite, out string message)
    {
        if (!QsModel.ValidateStringInput(name, "Computer name", 4, 64, out message))
        {
            return false;
        }

        if (!QsModel.ValidateStringInput(description, "Computer description", 4, 2048, out message))
        {
            return false;
        }

        // Make sure that 'name' can be used as the data file 
        string pathName = FileManagerModel.ValidPathName(name, out bool changed);
        if (changed)
        {
            Debug.WriteLine("Save Path Adjusted: ");
        }

        Debug.WriteLine("Save Path: " + pathName);

        if (!withOverwrite)
        {
            // Check for duplicates 
            if (this.fileManager.Exists(Area.User, Kind.Json, pathName))
            {
                message = "There is already a computer file with that name";
                return false;
            }
        }

        return true;
    }

    public bool SaveComputerMetadata(string name, string description, bool withOverwrite, out string message)
    {
        if (!this.ValidateComputerMetadata(name, description, withOverwrite, out message))
        {
            return false;
        }

        this.QuComputer.Name = name;
        this.QuComputer.Description = description;
        this.QuComputer.LastModified = DateTime.Now;
        this.IsDirty = true;

        return true;
    }

    public string? SaveComputerToFile(bool withOverwrite, out string message)
    {
        try
        {
            message = string.Empty;
            string name = this.QuComputer.Name;

            // Make sure that 'name' can be used as the data file 
            string pathName = FileManagerModel.ValidPathName(name, out bool changed);
            if (changed)
            {
                Debug.WriteLine("Save Path Adjusted: ");
            }

            Debug.WriteLine("Save Path: " + pathName);

            if (!withOverwrite)
            {
                // Check for duplicate files 
                if (this.fileManager.Exists(Area.User, Kind.Json, pathName))
                {
                    message = "There is already a computer file with that name";
                    return null;
                }
            }

            this.fileManager.Save<QuComputer>(Area.User, Kind.Json, pathName, this.QuComputer);

            // Update or Add to Projects 
            var clone = this.QuComputer.DeepClone();
            if (!this.Projects.TryAdd(name, clone))
            {
                this.Projects[name] = clone;
            }

            this.IsDirty = false;
            return pathName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Save Computer To File: Exception thrown: " + ex.Message;
            return null;
        }
    }

    public bool DeleteDocument(QuComputer quComputer, out string message)
    {
        message = string.Empty;
        try
        {
            string name = quComputer.Name;

            // Make sure that 'name' can be used as the data file 
            string pathName = FileManagerModel.ValidPathName(name, out bool changed);
            if (changed)
            {
                Debug.WriteLine("Save Path Adjusted: ");
            }

            Debug.WriteLine("Save Path: " + pathName);

            if (this.fileManager.Exists(Area.User, Kind.Json, pathName))
            {
                // Delete File 
                this.fileManager.Delete(Area.User, Kind.Json, pathName);

                // Remove from Projects 
                this.Projects.Remove(name);

                return true;
            }

            message = "No computer file with that name was found.";
            return false;
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex.ToString());
            message = "Exception thrown: " + ex.Message;
            return false;
        }
    }

    private static bool ValidateStringInput(string input, string label, int min, int max, out string message)
    {
        message = string.Empty;
        input = input.Trim();
        if (string.IsNullOrEmpty(input))
        {
            message = label + " cannot be left empty.";
            return false;
        }

        if (input.Length < min)
        {
            message = label + " is too short. (Min " + min.ToString() + " chars)";
            return false;
        }

        if (input.Length > max)
        {
            message = label + " is too long. (Max " + max.ToString() + " chars)";
            return false;
        }

        return true;
    }
}