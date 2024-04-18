using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using PvpStats.Helpers;
using System;
using System.Collections.Generic;

namespace PvpStats.Windows.Filter;

public enum StatSource {
    LocalPlayer,
    Teammate,
    Opponent,
    Spectated
}

public class StatSourceFilter : DataFilter {

    public override string Name => "数据分数";
    internal bool AllSelected { get; set; }
    internal bool InheritFromPlayerFilter { get; set; }
    public Dictionary<StatSource, bool> FilterState { get; set; } = new();

    public static Dictionary<StatSource, string> FilterNames => new() {
        { StatSource.LocalPlayer, "当前角色" },
        { StatSource.Teammate, "队友" },
        { StatSource.Opponent, "对手" },
        { StatSource.Spectated, "观战的比赛" }
    };

    public StatSourceFilter() { }

    internal StatSourceFilter(Plugin plugin, Action action, StatSourceFilter? filter = null) : base(plugin, action) {
        FilterState = new() {
                {StatSource.LocalPlayer, true },
                {StatSource.Teammate, true },
                {StatSource.Opponent, true },
                {StatSource.Spectated, true },
            };

        if(filter is not null) {
            foreach(var category in filter.FilterState) {
                FilterState[category.Key] = category.Value;
            }
        }
        UpdateAllSelected();
    }

    private void UpdateAllSelected() {
        AllSelected = true;
        foreach(var category in FilterState) {
            AllSelected = AllSelected && category.Value;
        }
    }

    internal override void Draw() {
        using var table = ImRaii.Table("数据分数表", 4, ImGuiTableFlags.NoClip);
        if(table) {
            ImGui.TableSetupColumn($"c1", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 4, ImGuiHelpers.GlobalScale * 150f));
            ImGui.TableSetupColumn($"c2", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 4, ImGuiHelpers.GlobalScale * 150f));
            ImGui.TableSetupColumn($"c3", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 4, ImGuiHelpers.GlobalScale * 150f));
            ImGui.TableSetupColumn($"c4", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 4, ImGuiHelpers.GlobalScale * 200f));
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            bool allSelected = AllSelected;
            if(ImGui.Checkbox($"勾选全部##{GetHashCode()}", ref allSelected)) {
                _plugin!.DataQueue.QueueDataOperation(() => {
                    foreach(var category in FilterState) {
                        FilterState[category.Key] = allSelected;
                    }
                    AllSelected = allSelected;
                    Refresh();
                });
            }
            ImGui.TableNextColumn();
            bool inheritFromPlayerFilter = InheritFromPlayerFilter;
            if(ImGui.Checkbox($"玩家过滤器继承##{GetHashCode()}", ref inheritFromPlayerFilter)) {
                _plugin!.DataQueue.QueueDataOperation(() => {
                    InheritFromPlayerFilter = inheritFromPlayerFilter;
                    Refresh();
                });
            }
            ImGuiHelper.HelpMarker("只包括符合玩家过滤器所有条件的玩家的统计数据。");
            ImGui.TableNextRow();

            foreach(var category in FilterState) {
                ImGui.TableNextColumn();
                bool filterState = category.Value;
                if(ImGui.Checkbox($"{FilterNames[category.Key]}##{GetHashCode()}", ref filterState)) {
                    _plugin!.DataQueue.QueueDataOperation(() => {
                        FilterState[category.Key] = filterState;
                        UpdateAllSelected();
                        Refresh();
                    });
                }
            }
        }
    }
}
