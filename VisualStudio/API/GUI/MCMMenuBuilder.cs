using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AuroraMonitor.ModSettings.API.Data;

namespace AuroraMonitor.ModSettings.API.GUI
{
    [RegisterTypeInIl2Cpp]
    public class MCMMenuBuilder : MonoBehaviour
    {
        public MCMMenuBuilder() { }
        public MCMMenuBuilder(IntPtr pointer) : base(pointer) { }

        public Dictionary<string, MCMTab> Tabs { get; } = new();
        public MCMTab? CurrentTab { get; set; }
        public int SelectedIndex = 0;
        public string? PreviousMod { get; set; }
        public ConsoleComboBox? Selector { get; set; }
        public UIPanel? ScrollPanel { get; set; }
        public Transform? ScrollPanelOffset { get; set; }
        public GameObject? ScrollBar { get; set; }
        public UISlider? Slider { get; set; }

        #region Unity Methods
        public void Awake()
        {
            Transform Content = transform.Find("GameObject");

            DestroyOld(Content);

            Selector            = CreateSelector(Content);
            ScrollPanel         = CreateScrollPanel(Content);
            ScrollPanelOffset   = CreateScrollPanelOffset(ScrollPanel);
            ScrollBar           = CreateScrollBar(ScrollPanel);
            Slider              = ScrollBar.GetComponentInChildren<UISlider>();
        }

        public void LateUpdate()
        {
            if (CurrentTab == null) return;
            if (InputManager.GetEscapePressed(InterfaceManager.GetPanel<Panel_OptionsMenu>()))
            {
                InterfaceManager.GetPanel<Panel_OptionsMenu>().OnCancel();
                return;
            }

            InterfaceManager.GetPanel<Panel_OptionsMenu>().UpdateMenuNavigationGeneric(ref SelectedIndex, CurrentTab.Items);
            EnableSettings();
            UpdateDescription();

            if (CurrentTab.ScrollBarHeight > 0)
            {
                float scroll = InputManager.GetAxisScrollWheel(InterfaceManager.GetPanel<Panel_OptionsMenu>());
                float amount = 60f / CurrentTab.ScrollBarHeight;

                if (scroll < 0)
                {
                    Slider.value += amount;
                }
                else if (scroll > 0)
                {
                    Slider.value -= amount;
                }
            }
        }

        public void Enable(Panel_OptionsMenu panel)
        {
            GameAudioManager.PlayGUIButtonClick();
            panel.SetTabActive(gameObject);
        }

        public void OnEnable()
        {
            MCMMenu.SetSettingsVisible(true);

            if (Selector.items.Count > 0)
            {
                Selector.items.Sort();

                string SelectedMod = (PreviousMod != null && Selector.items.Contains(PreviousMod)) ? PreviousMod : Selector.items[0];
                Selector.value = SelectedMod;
                SelectMod(SelectedMod);
            }
        }

        public void OnDisable()
        {
            MCMMenu.SetSettingsVisible(false);

            PreviousMod = Selector.value;

            foreach (MCMTab tab in Tabs.Values)
            {
                tab.OverrideConfirmation = false;
            }

            SetConfirmButtonVisible(true);
        }

        public void Notify()
        {
            CurrentTab.OverrideConfirmation = true;
            SetConfirmButtonVisible(true);
        }

        public void SetConfirmButtonVisible(bool visible)
        {
            if (InterfaceManager.GetPanel<Panel_OptionsMenu>() == null)
            {
                if (visible)
                {
                    Logging.LogError($"Could not find the {nameof(Panel_OptionsMenu)}");
                }
            }
            else
            {
                InterfaceManager.GetPanel<Panel_OptionsMenu>().m_SettingsNeedConfirmation = visible;
            }
        }

        public void CallOnConfirm()
        {
            foreach (MCMTab tab in Tabs.Values)
            {
                if (!tab.OverrideConfirmation) continue;

                tab.OverrideConfirmation = false;

                foreach (MCMMod mod in tab.MCMMods)
                {
                    mod.CallOnConfirm();
                }
            }

            SetConfirmButtonVisible(false);
        }

        #endregion
        #region ModSelectors
        #endregion
        #region Create Objects
        public ConsoleComboBox CreateSelector(Transform content)
        {
            ConsoleComboBox? selector = null;

            try
            {
                selector = content.Find("Quality").GetComponent<ConsoleComboBox>();
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to find \"Quality\" failed", e);
            }
            finally
            {
                selector?.items.Clear();

                Vector3 offset = new(200, 0);

                Transform transform = selector.transform;
                transform.localPosition -= offset;

                transform.Find("Button_Increase").localPosition += offset;
                transform.Find("Label_Value").localPosition += offset;

                Transform background = transform.Find("Console_Background");
                background.localPosition += offset / 2f;
                background.GetComponent<UISprite>().width += 200;

                EventDelegate.Set(selector.onChange, new Action(() => SelectMod(selector.value)));
            }

            return selector;
        }

        public UIPanel CreateScrollPanel(Transform content)
        {
            UIPanel? panel = null;

            try
            {
                panel = NGUITools.AddChild<UIPanel>(content.gameObject);
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to add a new panel to the transform failed", e);
            }
            finally
            {
                if (panel != null)
                {
                    panel.gameObject.name = "MCM_ScrollPanel";
                    panel.baseClipRegion = new(0, 0, 2000, 520);
                    panel.clipOffset = new(500, -260);
                    panel.clipping = UIDrawCall.Clipping.SoftClip;
                    panel.depth = 100;
                }
                else
                {
                    Logging.LogError("The UIPanel is still null");
                }
            }

            return panel;
        }

        public Transform CreateScrollPanelOffset(UIPanel content)
        {
            GameObject? offset = null;

            try
            {
                offset = NGUITools.AddChild(content.gameObject);
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to add a new GameObject to the UIPanel failed", e);
            }
            finally
            {
                if (offset != null)
                {
                    offset.name = "MCM_Offset";
                }
                else
                {
                    Logging.LogError("Offset is still null");
                }
            }

            return offset.transform;
        }

        public GameObject CreateScrollBar(UIPanel content)
        {
            Panel_CustomXPSetup customModePanel = InterfaceManager.GetPanel<Panel_CustomXPSetup>();
            GameObject? ScrollBarParent = null;
            GameObject? ScrollBarPrefab = null;
            GameObject? ScrollBar = null;

            try
            {
                ScrollBarParent = customModePanel.m_Scrollbar;
                ScrollBarPrefab = ScrollBarParent.transform.GetChild(0).gameObject;
                ScrollBar = NGUITools.AddChild(gameObject, ScrollBarPrefab);
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to find and create needed GameObjects failed", e);
            }
            finally
            {
                if (ScrollBar != null)
                {
                    ScrollBar.name = "MCM_ScrollBar";
                    ScrollBar.transform.localPosition = new Vector2(415, -40);

                    UISlider slider = ScrollBar.GetComponentInChildren<UISlider>(true);
                    slider.backgroundWidget.GetComponent<UISprite>().height = (int)content.height;
                    slider.foregroundWidget.GetComponent<UISprite>().height = (int)content.height;

                    ScrollBar.transform.Find("Glow").GetComponent<UISprite>().height = (int)content.height + 44;

                    EventDelegate.Set(slider.onChange, new Action(() => OnScroll(slider, true)));
                }
                else
                {
                    Logging.LogError(
                        string.Format(
                            "ScrollBarParent is {0}, ScrollBarPrefab is {1} and ScrollBar is {2}",
                            (ScrollBarParent == null) ? "null" : "UNKNOWN",
                            (ScrollBarPrefab == null) ? "null" : "UNKNOWN",
                            (ScrollBar == null) ? "null" : "UNKNOWN"));
                }
            }

            return ScrollBar;
        }
        #endregion
        #region Methods
        public bool DestroyOld(Transform content)
        {
            for (int i = content.childCount - 1; i >= 2; --i)
            {
                Transform setting = content.GetChild(i);

                setting.parent = null;
                Destroy(setting.gameObject);
            }

            return true;
        }

        public void EnableSettings()
        {
            if (CurrentTab == null) return;
            if (CurrentTab.Items == null) return;
            if (ScrollPanel == null) return;
            if (ScrollPanelOffset == null) return;
            if (Slider == null) return;
            

            if (Utils.GetMenuMovementVertical(InterfaceManager.GetPanel<Panel_OptionsMenu>(), true, true) == 0f) return;
            if (SelectedIndex == 0)
            {
                Slider.value = 0;
                return;
            }

            GameObject setting = CurrentTab.Items[SelectedIndex];

            float y = -setting.transform.localPosition.y;
            float top = ScrollPanelOffset.localPosition.y + Main.GridCellHeight;
            float bottom = ScrollPanelOffset.localPosition.y + ScrollPanel.height - Main.GridCellHeight;

            if (y < top)
            {
                Slider.value += GetSliderValue(y, top, CurrentTab.ScrollBarHeight);
                GameAudioManager.PlayGUIScroll();
            }
            else if (y > bottom)
            {
                Slider.value += GetSliderValue(y, bottom, CurrentTab.ScrollBarHeight);
                GameAudioManager.PlayGUIScroll();
            }
        }

        public static float GetSliderValue(float one, float two, float three)
        {
            return (one - two) / three;
        }

        public void UpdateDescription()
        {
            GameObject setting = CurrentTab.Items[SelectedIndex];
            Description description = setting.GetComponent<Description>();

            if (description == null) return;

            UILabel label = InterfaceManager.GetPanel<Panel_OptionsMenu>().m_OptionDescriptionLabel;

            UserInterfaceUtilities.SetupLabel(label, description.Text, FontStyle.Normal, UILabel.Crispness.Always, NGUIText.Alignment.Left, UILabel.Overflow.ResizeFreely, false, 16, 16, Color.white, false);

            label.transform.parent = setting.transform;
            label.transform.localPosition = new(655, 0);
            label.gameObject.SetActive(true);
        }
        #endregion
        #region Delagates
        public void SelectMod(string name)
        {
            if (CurrentTab != null)
            {
                NGUITools.SetActive(CurrentTab.Grid.gameObject, false);
            }

            SelectedIndex = 0;
            CurrentTab = Tabs[name];
            NGUITools.SetActive(CurrentTab.Grid.gameObject, true);

            ResizeScrollBar(CurrentTab);
            EnableSettings();
        }

        public void OnScroll(UISlider slider, bool sound)
        {
            ScrollPanelOffset.localPosition = new Vector2(0, slider.value * (CurrentTab.ScrollBarHeight));

            if (sound) GameAudioManager.PlayGUIScroll();
        }

        public void ResizeScrollBar(MCMTab tab)
        {
            int children        = tab.Grid.transform.childCount;
            float height        = children * Main.GridCellHeight;
            float maxHeight     = Slider.value * tab.ScrollBarHeight;

            if (tab == CurrentTab)
            {
                tab.ScrollBarHeight = height - ScrollPanel.height;

                ScrollbarThumbResizer resizer = Slider.GetComponent<ScrollbarThumbResizer>();
                resizer.SetNumSteps((int)ScrollPanel.height, (int)height);

                Slider.value = Mathf.Clamp01(maxHeight / Mathf.Max(1, tab.ScrollBarHeight));
                OnScroll(Slider, false);
            }
            else
            {
                tab.ScrollBarHeight = children * Main.GridCellHeight - ScrollPanel.height;
            }

            ScrollBar.SetActive(CurrentTab.ScrollBarHeight > 0);
        }
        #endregion
    }
}
