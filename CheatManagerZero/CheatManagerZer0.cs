//#define DEBUG_PROGRAM
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static BZCommon.GameHelper;
using BZCommon.GUIHelper;
using CheatManagerZer0.Configuration;
using CheatManagerZer0.NewCommands;
using System;

#if DEBUG_PROGRAM
using System.Collections;
#endif

namespace CheatManagerZer0
{
    public class CheatManagerZer0 : MonoBehaviour
    {  
        public CheatManagerZer0 Instance { get; private set; }
        internal static WarpTargets warpTargets = new WarpTargets();
        internal ButtonControl buttonControl;
        internal ButtonText buttonText;
        internal TechnologyMatrix techMatrix;        
        private Vector2 scrollPos;

        private static Rect windowRect = new Rect(Screen.width - (Screen.width / Config.ASPECT), 0, Screen.width / Config.ASPECT, (Screen.height / 4 * 3) - 2);
        private Rect drawRect;
        private Rect scrollRect;

        internal FMODAsset warpSound;

        public Utils.MonitoredValue<bool> isSeaglideFast = new Utils.MonitoredValue<bool>();
        public Utils.MonitoredValue<bool> isHoverBikeMoveOnWater = new Utils.MonitoredValue<bool>();
        public Event<Player.MotorMode> onPlayerMotorModeChanged = new Event<Player.MotorMode>();
        public Utils.MonitoredValue<bool> isSeamothCanFly = new Utils.MonitoredValue<bool>();
        public Event<string> onConsoleCommandEntered = new Event<string>();
        public Event<bool> onFilterFastChanged = new Event<bool>();
        public Event<object> onSeamothSpeedValueChanged = new Event<object>();
        public Event<object> onExosuitSpeedValueChanged = new Event<object>();
        public Event<object> onHoverbikeSpeedValueChanged = new Event<object>();
        public Event<object> onSeatruckSpeedValueChanged = new Event<object>();

        internal List<TechTypeData>[] tMatrix;
        internal List<GuiItem>[] scrollItemsList;

        internal List<TechTypeData> FullTechMatrix = new List<TechTypeData>();        

        internal List<GuiItem> commands = new List<GuiItem>();
        internal List<GuiItem> toggleCommands = new List<GuiItem>();
        internal List<GuiItem> daynightTab = new List<GuiItem>();
        internal List<GuiItem> weatherTab = new List<GuiItem>();
        internal List<GuiItem> categoriesTab = new List<GuiItem>();
        internal List<GuiItem> vehicleSettings = new List<GuiItem>();
        internal List<GuiItem> sliders = new List<GuiItem>();

        internal bool isWeatherEnabled = true;
        internal bool isActive;        
        internal bool initToggleButtons = false;               
        internal string prevCwPos = null;
        internal string seamothName;
        internal string exosuitName;
        internal string hoverbikeName;
        internal string seatruckName;

        internal float seamothSpeedMultiplier;
        internal float exosuitSpeedMultiplier;
        internal float hoverbikeSpeedMultiplier;
        internal float seatruckSpeedMultiplier;        

        private const int SPACE = 4;
        private const int ITEMSIZE = 22;
        private const int SLIDERHEIGHT = 30;
        private const int MAXSHOWITEMS = 3;

        private string windowTitle;
        private int normalButtonID = -1;
        private int toggleButtonID = -1;
        private int daynightTabID = 4;
        private int categoriesTabID = 0;
        private int scrollviewID = -1;
        private int vehicleSettingsID = -1;
        private int currentdaynightTab = 4;
        private int currentTab = 0;
        private int weatherTabID = -1;
        private bool filterFast;

#if DEBUG_PROGRAM
            internal static float crTimer = 10;           
#endif

        public void Awake()
        {
            Instance = this;
            useGUILayout = false;

#if DEBUG_PROGRAM
            isActive = true;
#endif
            UpdateTitle();            
            warpSound = ScriptableObject.CreateInstance<FMODAsset>();
            warpSound.path = "event:/tools/gravcannon/fire";

            techMatrix = new TechnologyMatrix();
            tMatrix = new List<TechTypeData>[18];
            techMatrix.InitTechMatrixList(ref tMatrix);

            techMatrix.InitFullTechMatrixList(ref FullTechMatrix);
            FullTechMatrix.Sort();

            /*
            if (Main.isExistsSMLHelperV2)
            {
                techMatrix.IsExistsModdersTechTypes(ref tMatrix, techMatrix.Known_Modded_TechTypes);
            }
            else
            {
                Debug.LogWarning("[CheatManager] Warning: 'SMLHelper.V2' not found! Some functions are not available!");
            }
            */
            techMatrix.SortTechLists(ref tMatrix);
            
            buttonText = new ButtonText();                        

            drawRect = SNWindow.InitWindowRect(windowRect, true);            

            List<Rect> commandRects = SNWindow.SetGridItemsRect(drawRect, 4, 3, ITEMSIZE, SPACE, SPACE, true, true);            

            SNGUI.CreateGuiItemsGroup(buttonText.Buttons, commandRects, GuiItemType.NORMALBUTTON, ref commands, new GuiItemColor());
            SNGUI.SetGuiItemsGroupLabel("Commands", commandRects.GetLast(), ref commands, new GuiItemColor(GuiColor.White));            

            List<Rect> toggleCommandRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref commandRects), drawRect.width, drawRect.height), 4, 5, ITEMSIZE, SPACE, SPACE, true, true);
            SNGUI.CreateGuiItemsGroup(buttonText.ToggleButtons, toggleCommandRects, GuiItemType.TOGGLEBUTTON, ref toggleCommands, new GuiItemColor(GuiColor.Red, GuiColor.Green));
            SNGUI.SetGuiItemsGroupLabel("Toggle Commands", toggleCommandRects.GetLast(), ref toggleCommands, new GuiItemColor(GuiColor.White));
                        
            List<Rect> daynightTabrects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref toggleCommandRects), drawRect.width, drawRect.height), 6, 1, 24, SPACE, SPACE, true, true);
            SNGUI.CreateGuiItemsGroup(buttonText.DayNightTab, daynightTabrects, GuiItemType.TAB, ref daynightTab, new GuiItemColor());
            SNGUI.SetGuiItemsGroupLabel("Day/Night Speed:", daynightTabrects.GetLast(), ref daynightTab, new GuiItemColor(GuiColor.White));
            
            List<Rect> weatherTabrects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref daynightTabrects), drawRect.width, drawRect.height), 5, 1, ITEMSIZE, SPACE, SPACE, true, true);
            SNGUI.CreateGuiItemsGroup(buttonText.WeatherCommands, weatherTabrects, GuiItemType.TOGGLEBUTTON, ref weatherTab, new GuiItemColor(GuiColor.Red, GuiColor.Green));
            SNGUI.SetGuiItemsGroupLabel("Weather Settings:", weatherTabrects.GetLast(), ref weatherTab, new GuiItemColor(GuiColor.White));

            List<Rect> categoriesTabrects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref weatherTabrects), drawRect.width, drawRect.height), 4, 5, ITEMSIZE, SPACE, SPACE, true, true);
            SNGUI.CreateGuiItemsGroup(buttonText.CategoriesTab, categoriesTabrects, GuiItemType.TAB, ref categoriesTab, new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White));
            SNGUI.SetGuiItemsGroupLabel("Categories:", categoriesTabrects.GetLast(), ref categoriesTab, new GuiItemColor(GuiColor.White));

            float nextYpos = SNWindow.GetNextYPos(ref categoriesTabrects);
            scrollRect = new Rect(drawRect.x + SPACE, nextYpos, drawRect.width - (SPACE * 2), drawRect.height - nextYpos);
            
            List<Rect>[] scrollItemRects = new List<Rect>[tMatrix.Length + 2];

            for (int i = 0; i < tMatrix.Length; i++)
            {
                float width = drawRect.width;

                if (i == 0 && tMatrix[0].Count > MAXSHOWITEMS)
                    width -= 20;

                else if (tMatrix[i].Count * (ITEMSIZE + SPACE) > scrollRect.height)
                    width -= 20;                
                
                scrollItemRects[i] = SNWindow.SetGridItemsRect(new Rect(0, 0, width, tMatrix[i].Count * (ITEMSIZE + SPACE)), 1, tMatrix[i].Count, ITEMSIZE, SPACE, 2, false, false, true);
            }
            
            scrollItemRects[tMatrix.Length] = SNWindow.SetGridItemsRect(new Rect(0, 0, drawRect.width - 20, warpTargets.Targets.Count * (ITEMSIZE + SPACE)), 1, warpTargets.Targets.Count, ITEMSIZE, SPACE, 2, false, false, true);
            scrollItemRects[tMatrix.Length + 1] = SNWindow.SetGridItemsRect(new Rect(0, 0, drawRect.width - 20, FullTechMatrix.Count * (ITEMSIZE + SPACE)), 1, FullTechMatrix.Count, ITEMSIZE, SPACE, 2, false, false, true);

            scrollItemsList = new List<GuiItem>[tMatrix.Length + 2];
            
            for (int i = 0; i < tMatrix.Length; i++)
            {
                scrollItemsList[i] = new List<GuiItem>();
                CreateTechGroup(tMatrix[i], scrollItemRects[i], GuiItemType.NORMALBUTTON, ref scrollItemsList[i], new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White),  GuiItemState.NORMAL, true, FontStyle.Normal, TextAnchor.MiddleLeft);                
            }
            
            scrollItemsList[tMatrix.Length] = new List<GuiItem>();
            scrollItemsList[tMatrix.Length + 1] = new List<GuiItem>();

            AddListToGroup(warpTargets.Targets, scrollItemRects[tMatrix.Length], GuiItemType.NORMALBUTTON, ref scrollItemsList[tMatrix.Length], new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White), GuiItemState.NORMAL, true, FontStyle.Normal, TextAnchor.MiddleLeft);

            AddTechListToGroup(FullTechMatrix, scrollItemRects[tMatrix.Length + 1], GuiItemType.NORMALBUTTON, ref scrollItemsList[tMatrix.Length + 1], new GuiItemColor(GuiColor.Gray, GuiColor.Green, GuiColor.White), GuiItemState.NORMAL, true, FontStyle.Normal, TextAnchor.MiddleLeft);

            var searchSeaGlide = new TechnologyMatrix.TechTypeSearch(TechType.Seaglide);
            string seaglideName = tMatrix[1][tMatrix[1].FindIndex(searchSeaGlide.EqualsWith)].Name;

            var searchSeamoth = new TechnologyMatrix.TechTypeSearch(TechType.Seamoth);
            seamothName = tMatrix[0][tMatrix[0].FindIndex(searchSeamoth.EqualsWith)].Name;

            var searchExosuit = new TechnologyMatrix.TechTypeSearch(TechType.Exosuit);
            exosuitName = tMatrix[0][tMatrix[0].FindIndex(searchExosuit.EqualsWith)].Name;

            var searchHoverBike = new TechnologyMatrix.TechTypeSearch(TechType.Hoverbike);
            hoverbikeName = tMatrix[0][tMatrix[0].FindIndex(searchHoverBike.EqualsWith)].Name;

            var searchSeaTruck = new TechnologyMatrix.TechTypeSearch(TechType.SeaTruck);
            seatruckName = tMatrix[0][tMatrix[0].FindIndex(searchSeaTruck.EqualsWith)].Name;


            string[] vehicleSetButtons = { $"{seamothName} Can Fly", $"{seaglideName} Speed Fast" , $"{hoverbikeName} Move on Water" };

            float scrollRectheight = (MAXSHOWITEMS + 1) * (scrollItemsList[0][0].Rect.height + 2);
            float y = scrollRect.y + scrollRectheight + SPACE;

            List<Rect> vehicleSettingsRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, y, drawRect.width, drawRect.height), 2, 2, ITEMSIZE, SPACE, SPACE, false, true);
            SNGUI.CreateGuiItemsGroup(vehicleSetButtons, vehicleSettingsRects, GuiItemType.TOGGLEBUTTON, ref vehicleSettings, new GuiItemColor(GuiColor.Red, GuiColor.Green));
            SNGUI.SetGuiItemsGroupLabel("Vehicle settings:", vehicleSettingsRects.GetLast(), ref vehicleSettings, new GuiItemColor(GuiColor.White));

            string[] sliderLabels = { $"{seamothName} speed multiplier:", $"{exosuitName} speed multiplier:", $"{hoverbikeName} speed multiplier:" };

            List<Rect> slidersRects = SNWindow.SetGridItemsRect(new Rect(drawRect.x, SNWindow.GetNextYPos(ref vehicleSettingsRects), drawRect.width, drawRect.height), 1, 3, SLIDERHEIGHT, SPACE, SPACE, false, false);
            SNGUI.CreateGuiItemsGroup(sliderLabels, slidersRects, GuiItemType.HORIZONTALSLIDER, ref sliders, new GuiItemColor());

            sliders[0].OnChangedEvent = onSeamothSpeedValueChanged;
            sliders[1].OnChangedEvent = onExosuitSpeedValueChanged;
            sliders[2].OnChangedEvent = onHoverbikeSpeedValueChanged;

            commands[(int)Commands.BackWarp].Enabled = false;
            commands[(int)Commands.BackWarp].State = GuiItemState.PRESSED;
            
            daynightTab[4].State = GuiItemState.PRESSED;
            categoriesTab[0].State = GuiItemState.PRESSED;

            seamothSpeedMultiplier = 1;
            exosuitSpeedMultiplier = 1;
            hoverbikeSpeedMultiplier = 1;
            seatruckSpeedMultiplier = 1;
            
            buttonControl = new ButtonControl();            
        }

        public void AddListToGroup(List<string[]> names, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {            
            for (int i = 0; i < names.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = names[i][1],
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }            
        }


        public void AddTechListToGroup(List<TechTypeData> techTypeDatas, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            for (int i = 0; i < techTypeDatas.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = techTypeDatas[i].Name,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }
        }
        public void CreateTechGroup(List<TechTypeData> techTypeDatas, List<Rect> rects, GuiItemType type, ref List<GuiItem> guiItems, GuiItemColor itemColor,
                                               GuiItemState state = GuiItemState.NORMAL, bool enabled = true, FontStyle fontStyle = FontStyle.Normal,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            guiItems.Clear();
                       
            for (int i = 0; i < techTypeDatas.Count; i++)
            {
                guiItems.Add(new GuiItem()
                {
                    Name = techTypeDatas[i].Name,
                    Type = type,
                    Enabled = enabled,
                    Rect = rects[i],
                    ItemColor = itemColor,
                    State = state,
                    FontStyle = fontStyle,
                    TextAnchor = textAnchor
                });
            }
        }
     
        public void OnDestroy()
        {            
            commands = null;            
            toggleCommands = null;
            daynightTab = null;
            categoriesTab = null;
            vehicleSettings = null;
            tMatrix = null;
            initToggleButtons = false;
            prevCwPos = null;
            warpSound = null;            
            isActive = false;            
            onConsoleCommandEntered.RemoveHandler(this, OnConsoleCommandEntered);
            onFilterFastChanged.RemoveHandler(this, OnFilterFastChanged);
        }

        public void Start()
        {                       
            onConsoleCommandEntered.AddHandler(this, new Event<string>.HandleFunction(OnConsoleCommandEntered));
            onFilterFastChanged.AddHandler(this, new Event<bool>.HandleFunction(OnFilterFastChanged));

#if DEBUG_PROGRAM                        

            StartCoroutine(DebugProgram());
#endif
        }        

        private void OnFilterFastChanged(bool enabled)
        {
            filterFast = enabled;
        }

        private void OnConsoleCommandEntered(string command)
        {
            if (command.Equals("weather"))
            {
                isWeatherEnabled = !isWeatherEnabled;

                if (!isWeatherEnabled)
                {
                    WeatherManager.main.debugLightningEnabled = false;
                    WeatherManager.main.debugPrecipitationEnabled = false;
                    WeatherManager.main.debugWindEnabled = false;
                    GameModeUtils.ActivateCheat(GameModeOption.NoCold);
                }
                else
                {
                    WeatherManager.main.debugLightningEnabled = true;
                    WeatherManager.main.debugPrecipitationEnabled = true;
                    WeatherManager.main.debugWindEnabled = true;
                    GameModeUtils.DeactivateCheat(GameModeOption.NoCold);
                }
            }

            UpdateButtonsState();
            Debug.Log(command);
        }        

#if DEBUG_PROGRAM
        private IEnumerator DebugProgram()
        {
            yield return new WaitForSeconds(crTimer);            

            print($"[CheatManager] Coroutine Debugger, recall every {crTimer} seconds\n");            
            if (isActive)
                StartCoroutine(DebugProgram());
        }
#endif
        
        internal void UpdateTitle()
        {            
           windowTitle = $"CheatManagerZer\u00F8 v.{Config.VERSION}, {Config.KEYBINDINGS["ToggleWindow"]} Toggle Window, {Config.KEYBINDINGS["ToggleMouse"]} Toggle Mouse";
        }

        public void Update()
        {
            if (Player.main != null)
            {                
                if (Input.GetKeyDown(Config.KEYBINDINGS["ToggleWindow"]))
                {
                    isActive = !isActive;
                }

                if (isActive)
                {
                    if (Input.GetKeyDown(Config.KEYBINDINGS["ToggleMouse"]))
                    {
                        UWE.Utils.lockCursor = !UWE.Utils.lockCursor;
                    }                                        

                    if(!initToggleButtons && !uGUI.main.loading.IsLoading)
                    {                        
                        SetToggleButtons();                                                
                        initToggleButtons = true;
                        UpdateButtonsState();
                    }
                    if (normalButtonID != -1)
                    {                        
                        buttonControl.NormalButtonControl(normalButtonID, ref commands, ref toggleCommands);
                    }

                    if (toggleButtonID != -1)
                    {                        
                        buttonControl.ToggleButtonControl(toggleButtonID, ref toggleCommands);
                    }

                    
                    if (daynightTabID != -1)
                    {
                        buttonControl.DayNightButtonControl(daynightTabID, ref currentdaynightTab, ref daynightTab);
                    }
                    

                    if (weatherTabID != -1)
                    {
                        buttonControl.WeatherButtonControl(weatherTabID, ref weatherTab);
                    }

                    if (categoriesTabID != -1)
                    {
                        if (categoriesTabID != currentTab)
                        {
                            categoriesTab[currentTab].State = GuiItemState.NORMAL;
                            categoriesTab[categoriesTabID].State = GuiItemState.PRESSED;
                            currentTab = categoriesTabID;
                            scrollPos = Vector2.zero;
                        }
                    }

                    if (scrollviewID != -1)
                    {
                        buttonControl.ScrollViewControl(currentTab, ref scrollviewID, ref scrollItemsList[currentTab], ref tMatrix, ref commands);
                    }

                    if (vehicleSettingsID != -1)
                    {
                        if (vehicleSettingsID == 0)
                        {
                            isSeamothCanFly.Update(!isSeamothCanFly.value);
                            vehicleSettings[0].State = SNGUI.ConvertBoolToState(isSeamothCanFly.value);
                        }
                        else if (vehicleSettingsID == 1)
                        {
                            isSeaglideFast.Update(!isSeaglideFast.value);
                            vehicleSettings[1].State = SNGUI.ConvertBoolToState(isSeaglideFast.value);                           
                        }
                        else if (vehicleSettingsID == 2)
                        {
                            isHoverBikeMoveOnWater.Update(!isHoverBikeMoveOnWater.value);
                            vehicleSettings[2].State = SNGUI.ConvertBoolToState(isHoverBikeMoveOnWater.value);
                        }
                    }
                }                                                       
            }
        }

        
        private void SetToggleButtons()
        {
            foreach (KeyValuePair<string, string> kvp in Config.Section_toggleButtons)
            {
                bool.TryParse(kvp.Value, out bool result);

                if (result)
                {
                    ExecuteCommand("", kvp.Key);
                }
            }
        }
        

        internal void UpdateButtonsState()
        {
            toggleCommands[(int)ToggleCommands.freedom].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoSurvival));
            toggleCommands[(int)ToggleCommands.creative].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoBlueprints));
            toggleCommands[(int)ToggleCommands.survival].State = SNGUI.ConvertBoolToState(GameModeUtils.RequiresSurvival());
            toggleCommands[(int)ToggleCommands.hardcore].State = SNGUI.ConvertBoolToState(GameModeUtils.IsPermadeath());
            toggleCommands[(int)ToggleCommands.fastbuild].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastBuildCheat);
            toggleCommands[(int)ToggleCommands.fastscan].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastScanCheat);
            toggleCommands[(int)ToggleCommands.fastgrow].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastGrowCheat);
            toggleCommands[(int)ToggleCommands.fasthatch].State = SNGUI.ConvertBoolToState(NoCostConsoleCommand.main.fastHatchCheat);
            toggleCommands[(int)ToggleCommands.filterfast].State = SNGUI.ConvertBoolToState(filterFast);
            toggleCommands[(int)ToggleCommands.nocost].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoCost));
            toggleCommands[(int)ToggleCommands.noenergy].State = SNGUI.ConvertBoolToState(GameModeUtils.IsCheatActive(GameModeOption.NoEnergy));
            toggleCommands[(int)ToggleCommands.nosurvival].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoSurvival));
            toggleCommands[(int)ToggleCommands.oxygen].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoOxygen));
            //toggleCommands[(int)ToggleCommands.radiation].State = SNGUI.ConvertBoolToState(GameModeUtils.IsOptionActive(GameModeOption.NoRadiation));
            toggleCommands[(int)ToggleCommands.invisible].State = SNGUI.ConvertBoolToState(GameModeUtils.IsInvisible());
            //toggleCommands[(int)ToggleCommands.shotgun].State = shotgun cheat
            toggleCommands[(int)ToggleCommands.nodamage].State = SNGUI.ConvertBoolToState(NoDamageConsoleCommand.main.GetNoDamageCheat());
            //toggleCommands[(int)ToggleCommands.noinfect].State = SNGUI.ConvertBoolToState(NoInfectConsoleCommand.main.GetNoInfectCheat());
            toggleCommands[(int)ToggleCommands.alwaysday].State = SNGUI.ConvertBoolToState(AlwaysDayConsoleCommand.main.GetAlwaysDayCheat());
            toggleCommands[(int)ToggleCommands.overpower].Enabled = GameModeUtils.RequiresSurvival();

            if (toggleCommands[(int)ToggleCommands.overpower].Enabled)
                toggleCommands[(int)ToggleCommands.overpower].State = SNGUI.ConvertBoolToState(OverPowerConsoleCommand.main.GetOverPowerCheat());

            vehicleSettings[0].State = SNGUI.ConvertBoolToState(isSeamothCanFly.value);
            vehicleSettings[1].State = SNGUI.ConvertBoolToState(isSeaglideFast.value);

            weatherTab[(int)Weather.weather].State = isWeatherEnabled ? GuiItemState.NORMAL : GuiItemState.PRESSED;
            weatherTab[(int)Weather.lightning].State = WeatherManager.main.debugLightningEnabled ? GuiItemState.NORMAL : GuiItemState.PRESSED;
            weatherTab[(int)Weather.precipitation].State = WeatherManager.main.debugPrecipitationEnabled ? GuiItemState.NORMAL : GuiItemState.PRESSED;
            weatherTab[(int)Weather.wind].State = WeatherManager.main.debugWindEnabled ? GuiItemState.NORMAL : GuiItemState.PRESSED;
            weatherTab[(int)Weather.cold].State = GameModeUtils.IsOptionActive(GameModeOption.NoCold) ? GuiItemState.PRESSED : GuiItemState.NORMAL;
        }       
        
        public void OnGUI()
        {
            if (!isActive)
                return;
            
            SNWindow.CreateWindow(windowRect, windowTitle);

            normalButtonID = SNGUI.DrawGuiItemsGroup(ref commands);
            toggleButtonID = SNGUI.DrawGuiItemsGroup(ref toggleCommands);
            daynightTabID = SNGUI.DrawGuiItemsGroup(ref daynightTab);
            weatherTabID = SNGUI.DrawGuiItemsGroup(ref weatherTab);
            categoriesTabID = SNGUI.DrawGuiItemsGroup(ref categoriesTab);

            if (currentTab == 0)
            {
                scrollviewID = SNScrollView.CreateScrollView(scrollRect, ref scrollPos, ref scrollItemsList[currentTab], "Select Item in Category:", categoriesTab[currentTab].Name, MAXSHOWITEMS);

                vehicleSettingsID = SNGUI.DrawGuiItemsGroup(ref vehicleSettings);

                SNHorizontalSlider.CreateHorizontalSlider(sliders[0].Rect, ref seamothSpeedMultiplier, 1f, 5f, sliders[0].Name, sliders[0].OnChangedEvent);
                SNHorizontalSlider.CreateHorizontalSlider(sliders[1].Rect, ref exosuitSpeedMultiplier, 1f, 5f, sliders[1].Name, sliders[1].OnChangedEvent);
                SNHorizontalSlider.CreateHorizontalSlider(sliders[2].Rect, ref hoverbikeSpeedMultiplier, 1f, 5f, sliders[2].Name, sliders[2].OnChangedEvent);
            }
            else
            {
                scrollviewID = SNScrollView.CreateScrollView(scrollRect, ref scrollPos, ref scrollItemsList[currentTab], "Select Item in Category:", categoriesTab[currentTab].Name);
            }
        }       
        
    }
}


