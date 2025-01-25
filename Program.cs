using System.Text.Json;
using Spectre.Console;

var actionSelection = new SelectionPrompt<IAction>()
    .PageSize(5)
    .EnableSearch()
    .UseConverter(static a => a.Title)
    .MoreChoicesText("[grey](Move up and down to reveal more saves)[/]")
    .AddChoices([
        new Run(),
        new OpenInExplorer(),
        new Rename(),
        new Backup(),
        new Restore(),
        new Delete(),
        new Compress(),
        new Decompress(),
    ]);

while (true)
{
    var fileSelection = new SelectionPrompt<INode>()
        .PageSize(5)
        .EnableSearch()
        .UseConverter(static save => save.Text)
        .MoreChoicesText("[grey](Move up and down to reveal more saves)[/]");

    foreach (var game in LoadGames())
        fileSelection.AddChoiceGroup(game, game.SaveFiles.Cast<INode>());

    var save = (SaveFile)AnsiConsole.Prompt(fileSelection);

    (Environment.CurrentDirectory, var previousDir) = (save.File.DirectoryName!, Environment.CurrentDirectory);

    try
    {
        AnsiConsole.Prompt(actionSelection).Execute(save);
    }
    finally
    {
        Environment.CurrentDirectory = previousDir;
    }
}

static IEnumerable<Game> LoadGames()
{
    var json = File.ReadAllText("config.json");
    var games = JsonSerializer.Deserialize<IEnumerable<JsonGame>>(json)!;
    foreach (var game in games)
        game.Files = game.Directory.EnumerateFiles(game.SavePattern);
    return games;
}

file sealed record class JsonGame(string Title, ILauncher Launcher, string OriginalSaveName, string Location, string SavePattern)
: Game(Title, Launcher, OriginalSaveName)
{
    public DirectoryInfo Directory => new(Environment.ExpandEnvironmentVariables(Location));

    public override string ToString() => base.ToString();

    protected override Type EqualityContract => base.EqualityContract;
}
