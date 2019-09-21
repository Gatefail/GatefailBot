using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace GatefailBot.Helpers
{
    public class ModuleContainer
    {
        public ModuleContainer(Dictionary<string, Type> modules)
        {
            _loadedModules = modules.ToImmutableDictionary(pair => pair.Key, pair => pair.Value);
        }

        private readonly ImmutableDictionary<string, Type> _loadedModules;
        public ImmutableDictionary<string, Type> GetLoadedModules()
        {
            return _loadedModules;
        }

        public bool ModuleExists(string moduleName)
        {
            return _loadedModules.ContainsKey(moduleName);
        }
    }
}