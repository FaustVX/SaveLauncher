using System.ComponentModel;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<RunCommand>();
app.Run(args);

file sealed class RunCommand : Command<RunCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Path to config. Defaults to [underline]./config.json.[/]")]
        [CommandArgument(0, "[configPath]")]
        public string ConfigPath { get; init; } = "config.json";
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var actionSelection = new SelectionPrompt<IAction>()
            .PageSize(5)
            .EnableSearch()
            .UseConverter(static action => action.Title)
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
                .MoreChoicesText(actionSelection.MoreChoicesText);

            foreach (var game in LoadGames(settings))
                fileSelection.AddChoiceGroup(game, game.SaveFiles.Cast<INode>());

            fileSelection.AddChoice(new Quit());

            if (AnsiConsole.Prompt(fileSelection) is not SaveFile save)
            {
                AnsiConsole.MarkupLine("[red]Quitting the application...[/]");
                break;
            }

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

        return 0;

        static IEnumerable<Game> LoadGames(Settings settings)
        {
            var json = File.ReadAllText(settings.ConfigPath);
            var games = JsonSerializer.Deserialize<IEnumerable<JsonGame>>(json)!;
            foreach (var game in games)
                game.Files = game.Directory.EnumerateFiles(game.SavePattern);
            return games;
        }
    }
}

file sealed record class JsonGame(string Title, ILauncher Launcher, string OriginalSaveName, string Location, string SavePattern)
: Game(Title, Launcher, OriginalSaveName)
{
    public DirectoryInfo Directory => new(Environment.ExpandEnvironmentVariables(Location));

    public override string ToString() => base.ToString();

    protected override Type EqualityContract => base.EqualityContract;
}

file sealed class Quit : INode
{
    public string Text => "[red]Quit[/]";
}
