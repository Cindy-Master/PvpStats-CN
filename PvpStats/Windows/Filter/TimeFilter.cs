using Dalamud.Interface.Utility;
using ImGuiNET;
using PvpStats.Types.Match;
using System;
using System.Linq;

namespace PvpStats.Windows.Filter;

public enum TimeRange {
    PastDay,
    PastWeek,
    ThisMonth,
    LastMonth,
    ThisYear,
    LastYear,
    All,
    Season,
    Custom
}

public class TimeFilter : DataFilter {
    public override string Name => "时间";

    public TimeRange StatRange { get; set; } = TimeRange.All;
    public static string[] Range = { "24小时内", "7天内", "本月内", "上个月", "今年", "去年", "全部时间", "按赛季", "自定义" };

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Season { get; set; } = ArenaSeason.Season.Count - 1;
    private string _lastStartTime = "";
    private string _lastEndTime = "";

    public TimeFilter() { }

    internal TimeFilter(Plugin plugin, Action action, TimeFilter? filter = null) : base(plugin, action) {
        if(filter is not null) {
            StatRange = filter.StatRange;
            StartTime = filter.StartTime;
            EndTime = filter.EndTime;
            Season = filter.Season;
        }
    }

    internal override void Draw() {
        int statRangeToInt = (int)StatRange;
        int seasonIndex = Season - 1;
        //ImGui.SetNextItemWidth(float.Min(ImGui.GetContentRegionAvail().X / 2f, ImGuiHelpers.GlobalScale * 125f));
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2f);
        if(ImGui.Combo($"##时间范围选择框", ref statRangeToInt, Range, Range.Length)) {
            _plugin!.DataQueue.QueueDataOperation(() => {
                StatRange = (TimeRange)statRangeToInt;
                Refresh();
            });
        }
        if(StatRange == TimeRange.Custom) {
            if(ImGui.BeginTable("时间过滤器", 2)) {
                ImGui.TableSetupColumn($"c1", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"c2", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text("开始:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth());
                var startTime = StartTime.ToString();
                if(ImGui.InputText($"##开始时间", ref startTime, 50, ImGuiInputTextFlags.None)) {
                    if(startTime != _lastStartTime) {
                        _lastStartTime = startTime;
                        if(DateTime.TryParse(startTime, out DateTime newStartTime)) {
                            _plugin!.DataQueue.QueueDataOperation(() => {
                                StartTime = newStartTime;
                                Refresh();
                            });
                        }
                    }
                }
                ImGui.TableNextColumn();
                ImGui.Text("结束:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                var endTime = EndTime.ToString();
                if(ImGui.InputText($"##结束时间", ref endTime, 50, ImGuiInputTextFlags.None)) {
                    if(endTime != _lastEndTime) {
                        _lastEndTime = endTime;
                        if(DateTime.TryParse(endTime, out DateTime newEndTime)) {
                            _plugin!.DataQueue.QueueDataOperation(() => {
                                EndTime = newEndTime;
                                Refresh();
                            });
                        }
                    }
                }
                ImGui.EndTable();
            }
        } else if(StatRange == TimeRange.Season) {
            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGuiHelpers.GlobalScale * 50f);
            if(ImGui.Combo($"##赛季组合", ref seasonIndex, ArenaSeason.Season.Keys.Select(x => x.ToString()).ToArray(), ArenaSeason.Season.Count)) {
                _plugin!.DataQueue.QueueDataOperation(() => {
                    Season = seasonIndex + 1;
                    Refresh();
                });
            }
        }
    }
}
