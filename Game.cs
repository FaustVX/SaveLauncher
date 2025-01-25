internal interface INode
{
    string Text { get; }
}

public record class Game(string Title, ILauncher Launcher, string OriginalSaveName) : INode
{
    public IEnumerable<SaveFile> SaveFiles { get; private set; } = default!;
    public IEnumerable<FileInfo> Files
    {
        set => SaveFiles = [.. GetSaveFiles(value)];
    }

    public string Text => Title;

    private IEnumerable<SaveFile> GetSaveFiles(IEnumerable<FileInfo> files)
    {
        foreach (var file in files)
            yield return new SaveFile(this, file);
    }

    public void Run()
    => Launcher.Run(this);
}

public readonly record struct SaveFile(Game Game, FileInfo File) : INode
{
    public readonly string Text
    {
        get
        {
            if (IsOriginalFile)
                return $"[underline]{File.Name}[/]";
            return File.Name;
        }
    }

    public bool IsOriginalFile => File.Name == Game.OriginalSaveName;

    public readonly void Swap()
    {
        if (IsOriginalFile)
            return;
        var name = File.FullName;
        var original = File.Directory!.GetFiles(Game.OriginalSaveName)[0];
        original.MoveTo(original.FullName + ".bak");
        File.MoveTo(Path.Combine(File.DirectoryName!, Game.OriginalSaveName));
        original.MoveTo(name);
    }
}
