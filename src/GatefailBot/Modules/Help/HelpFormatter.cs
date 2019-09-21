using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using GatefailBot.Services;
using Microsoft.VisualBasic;

namespace GatefailBot.Modules.Help
{
    public class HelpFormatter : BaseHelpFormatter
    {
        private ICachedModuleService _cachedModuleService;
        private ulong guildId;
        private readonly List<Command> _commands = new List<Command>();

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            _cachedModuleService =
                (ICachedModuleService)ctx.Services.GetService(typeof(ICachedModuleService));
            guildId = ctx.Guild.Id;
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            if (_cachedModuleService.IsModuleEnabled(guildId, command.Module.ModuleType.FullName).GetAwaiter()
                .GetResult())
            {
                _commands.Add(command);
            }
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            subcommands = subcommands.Where(c =>
                _cachedModuleService.IsModuleEnabled(guildId, nameof(c.Module.ModuleType)).GetAwaiter().GetResult());
            _commands.AddRange(subcommands);
            return this;
        }

        public override CommandHelpMessage Build()
        {
            var isOneCommand = _commands.Count == 1;

            if (isOneCommand)
            {
                return Build(_commands.First());
            }

            return Build(_commands);
        }

        private static CommandHelpMessage Build(IEnumerable<Command> commands)
        {
            var output = string.Join(", ", commands.Select(x => $"`{x.Name}`"));
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Gatefail Bot Help")
                .WithDescription("Listing all commands - Use `!help {commandName}` to get detailed command help");
            if(String.IsNullOrEmpty(output))
            {
                embed.AddField("Commands", "No commands found. It's likely that no modules are enabled. Talk to an admin to enable modules");
            }
            else
            {
                embed.AddField("Commands", output);
            }

            return new CommandHelpMessage(embed: embed.Build());
        }

        private static CommandHelpMessage Build(Command command)
        {
            var title = $"Gatefail Bot Help for {command.Name}";

            var aliases = command.Aliases.Select(x => $"`{x}`").ToList();

            // Get the main name of the command to the top of the list
            aliases.Insert(0, $"`{command.QualifiedName}`");

            var outputtedAliases = string.Join(", ", aliases);

            var embed = new DiscordEmbedBuilder()
                .WithTitle(title);

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                embed.AddField("Summary", command.Description);
            }

            if (aliases.Count > 1)
            {
                embed.AddField("Aliases", outputtedAliases);
            }

            var usages = command.Overloads
                .Select(BuildUsage)
                .ToList();

            string BuildUsage(CommandOverload commandOverload)
            {
                var output = $"`!{command.QualifiedName} ";

                var arguments = commandOverload.Arguments
                    .Select(f =>
                    {
                        var builder = new StringBuilder();

                        builder.Append($"{{{f.Name}}}");

                        if (f.IsOptional)
                        {
                            builder.Append("(Optional)");
                        }

                        return builder.ToString();
                    });

                output += string.Join(" ", arguments) + "`";

                return output;
            }

            var outputtedUsages = string.Join(Environment.NewLine, usages);

            embed.AddField("Usage", outputtedUsages);

            return new CommandHelpMessage(embed: embed.Build());
        }
    }
}