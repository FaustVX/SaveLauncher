using System.IO.Compression;
using Spectre.Console;

public interface IAction
{
    public abstract void Execute(SaveFile saveFile);
    public abstract string Title { get; }
}

public sealed class Run : IAction
{
    public string Title => "Swap & Run";

    public void Execute(SaveFile saveFile)
    {
        saveFile.Swap();
        saveFile.Game.Run();
    }
}

public sealed class Rename : IAction
{
    public string Title => "Rename";

    public void Execute(SaveFile saveFile)
    {
        var newName = AnsiConsole.Ask("Enter the new name for the save file:", saveFile.File.Name);
        newName = Path.GetFileName(newName);
        if (newName == saveFile.File.Name)
            return;
        saveFile.File.MoveTo(newName);
        AnsiConsole.MarkupLine($"[green]Save file renamed to {newName}.[/]");
    }
}

public sealed class Delete : IAction
{
    public string Title => "Delete";

    public void Execute(SaveFile saveFile)
    {
        if (AnsiConsole.Confirm("Are you sure you want to delete this save file?", defaultValue: false))
        {
            saveFile.File.Delete();
            AnsiConsole.MarkupLine("[red]Save file deleted.[/]");
        }
    }
}

public sealed class Backup : IAction
{
    public string Title => "Backup";

    public void Execute(SaveFile saveFile)
    {
        var backupName = saveFile.File.Name + ".bak";
        saveFile.File.CopyTo(backupName);
        AnsiConsole.MarkupLine($"[green]Save file backed up as {backupName}.[/]");
    }
}

public sealed class Restore : IAction
{
    public string Title => "Restore";

    public void Execute(SaveFile saveFile)
    {
        var backupName = saveFile.File.Name + ".bak";
        if (File.Exists(backupName))
        {
            saveFile.File.Delete();
            File.Move(backupName, saveFile.File.Name);
            AnsiConsole.MarkupLine($"[green]Save file restored from {backupName}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Backup file {backupName} does not exist.[/]");
        }
    }
}

public sealed class Compress : IAction
{
    public string Title => "Compress";

    public void Execute(SaveFile saveFile)
    {
        var compressedFileName = saveFile.File.Name + ".zip";
        using (var archive = ZipFile.Open(compressedFileName, ZipArchiveMode.Create))
        {
            archive.CreateEntryFromFile(saveFile.File.FullName, saveFile.File.Name);
        }
        AnsiConsole.MarkupLine($"[green]Save file compressed to {compressedFileName}.[/]");
    }
}

public sealed class Decompress : IAction
{
    public string Title => "Decompress";

    public void Execute(SaveFile saveFile)
    {
        var compressedFileName = saveFile.File.Name + ".zip";
        if (File.Exists(compressedFileName))
        {
            saveFile.File.Delete();
            ZipFile.ExtractToDirectory(compressedFileName, saveFile.File.DirectoryName!);
            AnsiConsole.MarkupLine($"[green]Save file decompressed from {compressedFileName}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Compressed file {compressedFileName} does not exist.[/]");
        }
    }
}

public sealed class OpenInExplorer : IAction
{
    public string Title => "Open in Explorer";

    public void Execute(SaveFile saveFile)
    {
        var argument = $"/select, \"{saveFile.File.FullName}\"";
        System.Diagnostics.Process.Start("explorer.exe", argument);
        AnsiConsole.MarkupLine($"[green]Opened {saveFile.File.Name} in Explorer.[/]");
    }
}
