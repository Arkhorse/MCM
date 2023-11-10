namespace AuroraMonitor.ModSettings.API
{
    public class MCMMenu
    {
        public static HashSet<MCMMod> MainMenuMods { get; } = new();
        public static HashSet<MCMMod> SandboxMods { get; } = new();
        // not yet supported as mods do not work in WinterMute
        public static HashSet<MCMMod> WinterMuteMods { get; } = new();
        public static SortedDictionary<string, List<MCMMod>> SortedMods { get; } = new();

        // Set this to true whenever ML supports WinterMute mods again
        public static bool WinterMuteSupported { get; } = false;

        public bool RegisterSettings(MCMMod mod, string Name, MenuType menuType)
        {
            if (mod == null) return false;
            if (string.IsNullOrEmpty(Name)) return false;
            
            if (menuType.HasFlag(MenuType.All))
            {
                MainMenuMods.Add(mod);
                SandboxMods.Add(mod);
                WinterMuteMods.Add(mod);
            }
            else if (menuType.HasFlag(MenuType.MainMenu))
            {
                MainMenuMods.Add(mod);
            }
            else if (menuType.HasFlag(MenuType.Sandbox))
            {
                SandboxMods.Add(mod);
            }
            else if (menuType.HasFlag(MenuType.Wintermute))
            {
                WinterMuteMods.Add(mod);
            }

            if (SortedMods.TryGetValue(Name, out List<MCMMod>? Settings))
            {
                Settings.Add(mod);
            }
            else
            {
                Settings = new List<MCMMod> { mod };
                SortedMods.Add(Name, Settings);
            }

            return true;
        }

        public static bool BuildGUI(Panel_OptionsMenu optionsMenu)
        {
            GameObject? MCMSettingsTab = null;

            return true;
        }

        public static void SetSettingsVisible(bool visible)
        {
            HashSet<MCMMod> settings = GetMCMMods();

            foreach (MCMMod mod in settings)
            {
                mod.MenuVisibility?.SetVisible(visible);
            }
        }

        public static bool HasVisibleSettings()
        {
            HashSet<MCMMod> settings = GetMCMMods();

            foreach (MCMMod mod in settings)
            {
                return mod.m_Visibility.IsVisible();
            }

            return false;
        }

        public static HashSet<MCMMod> GetMCMMods()
        {
            bool WinterMuteActive = WinterMuteSupported && GameManager.IsStoryMode();
            return (GameManager.IsMainMenuActive()) ? MainMenuMods : (WinterMuteActive) ? WinterMuteMods : SandboxMods;
        }
    }
}
