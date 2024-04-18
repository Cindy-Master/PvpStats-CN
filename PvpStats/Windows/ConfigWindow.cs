using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PvpStats.Helpers;
using System.Numerics;

namespace PvpStats.Windows;
internal class ConfigWindow : Window {

    private Plugin _plugin;

    public ConfigWindow(Plugin plugin) : base("PVP 记录 设置") {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(300, 50),
            MaximumSize = new Vector2(400, 800)
        };
        _plugin = plugin;
    }

    public override void Draw() {
        if(ImGui.BeginTabBar("SettingsTabBar")) {
            if(ImGui.BeginTabItem("Interface")) {
                DrawInterfaceSettings();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawInterfaceSettings() {
        ImGui.TextColored(ImGuiColors.DalamudYellow, "记录器窗口");
        //var filterRatio = _plugin.Configuration.FilterRatio;
        //ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2f);
        //if(ImGui.SliderFloat("Filters height ratio", ref filterRatio, 2f, 5f)) {
        //    _plugin.Configuration.FilterRatio = filterRatio;
        //    _plugin.Configuration.Save();
        //}
        //ImGuiHelper.HelpMarker("Controls the denominator of the ratio of the window that will be occupied by the filters child.");
        //var sizeToFit = _plugin.Configuration.SizeFiltersToFit;
        //if(ImGui.Checkbox("Size to fit", ref sizeToFit)) {
        //    _plugin.Configuration.SizeFiltersToFit = sizeToFit;
        //    _plugin.Configuration.Save();
        //}

        var filterHeight = (int)_plugin.Configuration.CCWindowConfig.FilterHeight;
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2f);
        if(ImGui.SliderInt("过滤器子项高度", ref filterHeight, 100, 500)) {
            _plugin.Configuration.CCWindowConfig.FilterHeight = (uint)filterHeight;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        bool minimize = _plugin.Configuration.MinimizeWindow;
        if(ImGui.Checkbox("收缩窗口时缩小", ref minimize)) {
            _plugin.Configuration.MinimizeWindow = minimize;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        bool minimizeDir = _plugin.Configuration.MinimizeDirectionLeft;
        if(ImGui.Checkbox("收缩时锚定窗口左侧", ref minimizeDir)) {
            _plugin.Configuration.MinimizeDirectionLeft = minimizeDir;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        ImGuiHelper.HelpMarker("仅适用于先前的设置。否则锚定在右侧。", true, true);
        bool saveTabSize = _plugin.Configuration.PersistWindowSizePerTab;
        if(ImGui.Checkbox("保存每个选项卡的窗口大小", ref saveTabSize)) {
            _plugin.Configuration.PersistWindowSizePerTab = saveTabSize;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        bool resizeLeft = _plugin.Configuration.ResizeWindowLeft;
        if(ImGui.Checkbox("在选项卡切换时向左调整窗口大小", ref resizeLeft)) {
            _plugin.Configuration.ResizeWindowLeft = resizeLeft;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        bool colorScale = _plugin.Configuration.ColorScaleStats;
        if(ImGui.Checkbox("颜色标尺统计数值", ref colorScale)) {
            _plugin.Configuration.ColorScaleStats = colorScale;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        ImGui.Separator();

        ImGui.TextColored(ImGuiColors.DalamudYellow, "比赛详情窗口");

        bool resizeableWindow = _plugin.Configuration.ResizeableMatchWindow;
        if(ImGui.Checkbox("使窗口可调整大小", ref resizeableWindow)) {
            _plugin.Configuration.ResizeableMatchWindow = resizeableWindow;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        ImGuiHelper.HelpMarker("重新打开窗口以反映更改。", true, true);

        bool showBackgroundImage = _plugin.Configuration.ShowBackgroundImage;
        if(ImGui.Checkbox("显示背景图片", ref showBackgroundImage)) {
            _plugin.Configuration.ShowBackgroundImage = showBackgroundImage;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        bool playerTeamLeft = _plugin.Configuration.LeftPlayerTeam;
        if(ImGui.Checkbox("始终将玩家队伍显示在左侧", ref playerTeamLeft)) {
            _plugin.Configuration.LeftPlayerTeam = playerTeamLeft;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }

        bool anchorTeamNames = _plugin.Configuration.AnchorTeamNames;
        if(ImGui.Checkbox("锚定队伍统计信息", ref anchorTeamNames)) {
            _plugin.Configuration.AnchorTeamNames = anchorTeamNames;
            _plugin.DataQueue.QueueDataOperation(_plugin.Configuration.Save);
        }
        ImGuiHelper.HelpMarker("团队统计行不会受排序影响。", true, true);
        ImGui.Separator();
    }
}
