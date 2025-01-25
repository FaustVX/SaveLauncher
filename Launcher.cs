using System.Diagnostics;
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(SteamLauncher), "steam")]
[JsonDerivedType(typeof(UbisoftLauncher), "ubisoft")]
[JsonDerivedType(typeof(PathLauncher), "path")]
public interface ILauncher
{
    public abstract void Run(Game game);
}

public sealed record class SteamLauncher(int ID) : ILauncher
{
    public void Run(Game game)
    {
        var psi = new ProcessStartInfo($"steam://rungameid/{ID}")
        {
            UseShellExecute = true,
        };
        Process.Start(psi);
    }
}

public sealed record class UbisoftLauncher(int ID) : ILauncher
{
    public void Run(Game game)
    {
        var psi = new ProcessStartInfo($"uplay://launch/{ID}/0")
        {
            UseShellExecute = true,
        };
        Process.Start(psi);
    }
}

public sealed record class PathLauncher(string Path) : ILauncher
{
    public void Run(Game game)
    {
        Process.Start(Path);
    }
}
