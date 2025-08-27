namespace Lyt.Quantics.Studio.Model;

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
            bool status = this.QuComputer.AddQubitAtEnd(out message);
            if (status)
            {
                status = this.QuComputer.AddQubitAtEnd(out message);
                if (status)
                {
                    this.FinalizeModelCreation();
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
                this.FinalizeModelCreation();
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

            // Save the opening time so that we can later filter by MRU 
            this.QuComputer.LastOpened = DateTime.Now;
            this.SaveComputerToFile(withOverwrite: true, out message);

            this.FinalizeModelCreation();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create from Document: Exception thrown: " + ex.Message;
            return false;
        }
    }

    private void FinalizeModelCreation ()
    {
        SwapData.OnQuBitCountChanged(this.QuComputer.QuBitsCount);
        _ = this.QuComputer.Reset(out string _);
        this.InitializeMeasureStates();
        this.IsDirty = false;
        MakeModelLoaded().Publish();
    }

    public bool ValidateComputerMetadata(
        FileInformation fileInformation, bool withOverwrite, out string message)
    {
        message = string.Empty;
        // Make sure that 'name' can be used as the data file 
        string pathName = FileManagerModel.ValidPathName(fileInformation.Name, out bool changed);
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

    public bool SaveComputerMetadata(FileInformation fileInformation, bool withOverwrite, out string message)
    {
        if (!this.ValidateComputerMetadata(fileInformation, withOverwrite, out message))
        {
            return false;
        }

        this.QuComputer.Name = fileInformation.Name;
        this.QuComputer.Description = fileInformation.Description;
        this.QuComputer.LastModified = DateTime.Now;

        // We changed a date witout saving to disk, then we are dirty
        this.IsDirty = true;

        return true;
    }

    public string? SaveComputerToFile(bool withOverwrite, out string message)
    {
        try
        {
            message = string.Empty;
            string name = this.QuComputer.Name;

            // Make sure again that 'name' can be used as the data file 
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
            string fileName = string.Concat(pathName, FileManagerModel.JsonExtension);
            if (!this.Projects.TryAdd(fileName, clone))
            {
                this.Projects[fileName] = clone;
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
}
