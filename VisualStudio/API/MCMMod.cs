using AuroraMonitor.ModSettings.API.Attributes;

namespace AuroraMonitor.ModSettings.API
{
    public abstract class MCMMod
    {
        public FieldInfo[]? FieldInfos { get; set; }
        public Dictionary<FieldInfo, object?>? ConfirmedValues { get; set; }
        public List<Action>? RefreshActions { get; set; }

        public Visibility? MenuVisibility { get; private set; }
        public Visibility? m_Visibility { get; private set; }
        public Dictionary<FieldInfo, Visibility>? FieldVisibilities { get; private set; }
        public delegate void OnVisibilityChange(bool visible);

        protected MCMMod()
        {
            FieldInfos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            ConfirmedValues = new Dictionary<FieldInfo, object?>(FieldInfos.Length);
            RefreshActions = new List<Action>();

            MenuVisibility = new();
            MenuVisibility.SetVisible(false);
            MenuVisibility.AddListener(
                (visible) =>
                {
                    if (visible)
                    {
                        foreach (FieldInfo field in FieldInfos)
                        {
                            ConfirmedValues[field] = field.GetValue(this);
                        }
                        RefreshGUI();
                    }
                    else
                    {
                        foreach (FieldInfo field in FieldInfos)
                        {
                            field.SetValue(this, ConfirmedValues[field]);
                        }
                    }
                });
            m_Visibility = new(MenuVisibility);
            FieldVisibilities = new Dictionary<FieldInfo, Visibility>(FieldInfos.Length);

            foreach (FieldInfo field in FieldInfos)
            {
                FieldVisibilities.Add(field, new(m_Visibility));
            }

            AttributesHandler.ValidateFields(this);
        }

        protected virtual void OnConfirm() { }
        protected virtual void OnChange(FieldInfo field, object? Old, object? New) { }
        public void CallOnConfirm()
        {
            try
            {
                OnConfirm();
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to call \"OnConfirm()\" failed", e);
            }
            finally
            {
                foreach (FieldInfo field in FieldInfos)
                {
                    ConfirmedValues[field] = field.GetValue(this);
                }
            }
        }
        public void CallOnChange(FieldInfo field, object? Old, object? New)
        {
            try
            {
                OnChange(field, Old, New);
            }
            catch (Exception e)
            {
                throw new BadMemeException("Attempting to call \"OnChange(FieldInfo, object, object)\" failed", e);
            }
        }

        internal void SetFieldValue(FieldInfo field, object? New)
        {
            object? Old = field.GetValue(this);

            if (Old == New) return;

            field.SetValue(this, New);
            CallOnChange(field, Old, New);
        }

        #region AddMCMMod
        public void AddMCMMod(string ModName)
        {
            AddMCMMod(ModName, Position.ASCII, MenuType.All);
        }

        public void AddMCMMod(string ModName, MenuType menuType)
        {
            AddMCMMod(ModName, Position.ASCII, menuType);
        }

        public void AddMCMMod(string ModName, Position position)
        {
            AddMCMMod(ModName, position, MenuType.All);
        }

        public void AddMCMMod(string ModName, Position position, MenuType menuType)
        {

        }
        #endregion
        #region Refresh
        public void RefreshGUI()
        {
            if (!MenuVisibility.IsVisible())
            {
                return;
            }

            foreach (Action refresh in RefreshActions)
            {
                refresh();
            }
        }

        public void AddRefreshAction(Action refresh)
        {
            RefreshActions?.Add(refresh);
        }
        #endregion
        #region Visibility
        public void AddVisibilityListener(OnVisibilityChange listener)
        {
            m_Visibility?.AddListener(listener);
        }
        public void AddVisibilityListener(OnVisibilityChange listener, FieldInfo field)
        {
            GetFieldVisibility(field).AddListener(listener);
        }
        public bool IsFieldVisible(string FieldName)
        {
            return IsFieldVisible(GetFieldWithName(FieldName));
        }
        public bool IsFieldVisible(FieldInfo field)
        {
            return GetFieldVisibility(field).IsVisible();
        }
        #endregion
        #region FieldInfo Extensions
        public FieldInfo[]? GetFields() => FieldInfos;
        public FieldInfo GetFieldWithName(string FieldName)
        {
            if (string.IsNullOrEmpty(FieldName))
            {
                throw new ArgumentException("Attempting to use an invalid string to get a field", nameof(FieldName));
            }

            FieldInfo? field = GetType().GetField(FieldName);

            if (field == null)
            {
                throw new BadMemeException($"Attemping to get field {FieldName} failed and is null");
            }

            return field;
        }
        public Visibility GetFieldVisibility([DisallowNull] FieldInfo field)
        {

            if (FieldVisibilities.TryGetValue(field, out Visibility? visibility))
            {
                return visibility;
            }
            else
            {
                throw new ArgumentException($"The requested field {field.Name} is not part of the class {GetType().Name}");
            }
        }
        #endregion
    }
    public class Visibility
    {
        private List<MCMMod.OnVisibilityChange> VisibilityListeners { get; } = new();
        private List<Visibility> Children { get; } = new();
        private bool ParentVisible { get; set; } = true;
        private bool Visible { get; set; } = true;

        public Visibility() { }
        public Visibility(Visibility parent)
        {
            parent.Children.Add(this);
            SetVisible(parent.IsVisible());
        }

        public void AddChild(Visibility child)
        {
            Children.Add(child);
            child.SetVisible(IsVisible());
        }

        public void RemoveChild(Visibility child)
        {
            throw new BadMemeException("You cant remove children");
        }

        #region Visibility Logic
        public bool IsVisible()
        {
            return ParentVisible && Visible;
        }

        public void SetVisible(bool visible)
        {
            if (visible == IsVisible()) return;

            ChangeVisibility(visible);
        }
        #endregion
        #region Listener Logic
        public void AddListener(MCMMod.OnVisibilityChange listener)
        {
            VisibilityListeners.Add(listener);
        }

        public void ChangeVisibility(bool visible)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                //Children[i].
            }

            for (int j = 0; j < VisibilityListeners.Count; j++)
            {
                VisibilityListeners[j].Invoke(visible);
            }
        }
        #endregion
    }

    public enum Position
    {
        ASCII,
        Top,
        Bottom
    }

    [Flags]
    public enum MenuType
    {
        All,
        MainMenu,
        Sandbox,
        Wintermute
    }
}
