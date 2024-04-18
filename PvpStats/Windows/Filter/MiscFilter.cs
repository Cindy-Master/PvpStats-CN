using Dalamud.Interface.Utility;
using ImGuiNET;
using System;

namespace PvpStats.Windows.Filter;
public class MiscFilter : DataFilter {

    public override string Name => "其他";

    public bool MustHaveStats { get; set; }

    public bool ShowDeleted { get; set; }

    public MiscFilter() { }

    internal MiscFilter(Plugin plugin, Action action, MiscFilter? filter = null) : base(plugin, action) {
        if(filter is not null) {
            MustHaveStats = filter.MustHaveStats;
            ShowDeleted = filter.ShowDeleted;
        }
    }

    internal override void Draw() {
        ImGui.BeginTable("其他过滤表", 3, ImGuiTableFlags.NoClip);
        ImGui.TableSetupColumn($"c1", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 3, ImGuiHelpers.GlobalScale * 350f));
        ImGui.TableSetupColumn($"c2", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 3, ImGuiHelpers.GlobalScale * 350f));
        ImGui.TableSetupColumn($"c3", ImGuiTableColumnFlags.WidthFixed, float.Min(ImGui.GetContentRegionAvail().X / 3, ImGuiHelpers.GlobalScale * 350f));
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        bool mustHaveStats = MustHaveStats;
        if(ImGui.Checkbox("必须有赛后统计", ref mustHaveStats)) {
            _plugin!.DataQueue.QueueDataOperation(() => {
                MustHaveStats = mustHaveStats;
                Refresh();
            });
        }
        //bool showDeleted = ShowDeleted;
        //if (ImGui.Checkbox("Show deleted/incomplete", ref showDeleted)) {
        //    _plugin!.DataQueue.QueueDataOperation(() => {
        //        ShowDeleted = showDeleted;
        //        Refresh();
        //    });
        //}
        ImGui.EndTable();
    }
}
