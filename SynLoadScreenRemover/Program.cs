﻿using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using SynLoadScreenRemover.Types;
using System;
using Noggog;

namespace SynLoadScreenRemover
{
    internal class Program
    {
        static Lazy<Settings> LazySettings = new();
        static Settings config => LazySettings.Value;
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .SetAutogeneratedSettings("Settings", "settings.json", out LazySettings)
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynLSR.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var stat = state.PatchMod.Statics.AddNew("None");
            state.LoadOrder.PriorityOrder.LoadScreen().WinningOverrides().ForEach(ls =>
            {
                var nls = state.PatchMod.LoadScreens.GetOrAddAsOverride(ls);
                nls.LoadingScreenNif.SetTo(stat);
                if (config.RemoveLoreText)
                {
                    nls.Description = "";
                }
            });
        }
    }
}