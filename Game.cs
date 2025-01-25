using System.Diagnostics;

internal interface INode
{
    string Text { get; }
}

public record class Game(string Title, int SteamId, string OriginalSaveName) : INode
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
    {
        var psi = new ProcessStartInfo($"steam://rungameid/{SteamId}")
        {
            UseShellExecute = true,
        };
        Process.Start(psi)!.WaitForExit();
    }
}

public readonly record struct SaveFile(Game Game, FileInfo File) : INode
{
    public readonly string Text => File.Name;
    public bool IsOriginalFile => File.Name == Game.OriginalSaveName;

    public readonly void Swap()
    {
        if (IsOriginalFile)
            return;
        File.Replace(File.Directory!.GetFiles(Game.OriginalSaveName)[0].FullName, File.FullName);
    }
}
