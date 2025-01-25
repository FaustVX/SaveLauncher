using System.Text.Json;
using Spectre.Console;

AnsiConsole.MarkupLine("[underline red]Hello[/] World!");

var selection = new SelectionPrompt<INode>()
    .Title("What's your [green]favorite fruit[/]?")
    .PageSize(5)
    .EnableSearch()
    .UseConverter(static save => save.Text)
    .MoreChoicesText("[grey](Move up and down to reveal more saves)[/]");

foreach (var game in LoadGames())
    selection.AddChoiceGroup(game, game.SaveFiles.Cast<INode>());

var save = (SaveFile)AnsiConsole.Prompt(selection);

save.Swap();
save.Game.Run();

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

