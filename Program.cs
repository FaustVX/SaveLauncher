using Spectre.Console;

ReadOnlySpan<Game> games =
[
    new Game("Aotenjo", 3066570, "player_1.aotenjoprofile")
    {
        Files =
        [
            new(Environment.ExpandEnvironmentVariables(@"%appdata%\..\LocalLow\Aotenjo\Aotenjo\player_1.aotenjoprofile")),
            new(Environment.ExpandEnvironmentVariables(@"%appdata%\..\LocalLow\Aotenjo\Aotenjo\player_2.aotenjoprofile")),
        ]
    }
];

AnsiConsole.MarkupLine("[underline red]Hello[/] World!");

var selection = new SelectionPrompt<INode>()
    .Title("What's your [green]favorite fruit[/]?")
    .PageSize(5)
    .EnableSearch()
    .UseConverter(static save => save.Text)
    .MoreChoicesText("[grey](Move up and down to reveal more saves)[/]");

foreach (var game in games)
{
    selection.AddChoiceGroup(game, game.SaveFiles.Cast<INode>());
}

var save = (SaveFile)AnsiConsole.Prompt(selection);

save.Swap();
save.Game.Run();
