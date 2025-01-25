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
        var name = AnsiConsole.Ask("New file name", saveFile.File.Name);
        name = Path.GetFileName(name);
        if (name == saveFile.File.Name)
            return;
        saveFile.File.MoveTo(Path.Combine(saveFile.File.DirectoryName!, name));
    }
}
