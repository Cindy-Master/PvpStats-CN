﻿using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using PvpStats.Helpers;
using PvpStats.Types.Match;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PvpStats.Windows.Summary;
internal class CrystallineConflictRecords {

    private Plugin _plugin;
    private SemaphoreSlim _refreshLock = new SemaphoreSlim(1);

    private Dictionary<CrystallineConflictMatch, List<(string, string)>> Superlatives = new();

    internal int LongestWinStreak { get; private set; }
    internal int LongestLossStreak { get; private set; }

    internal CrystallineConflictRecords(Plugin plugin) {
        _plugin = plugin;
    }

    public void Refresh(List<CrystallineConflictMatch> matches) {
        CrystallineConflictMatch? longestMatch = null, shortestMatch = null, closestMatch = null, closestWin = null, closestLoss = null,
            mostKills = null, mostDeaths = null, mostAssists = null, mostDamageDealt = null, mostDamageTaken = null, mostHPRestored = null, mostTimeOnCrystal = null;
        int longestWinStreak = 0, longestLossStreak = 0, spectatedMatchCount = 0, currentWinStreak = 0, currentLossStreak = 0;

        foreach(var match in matches) {
            //track these for spectated matches as well
            if(longestMatch == null) {
                longestMatch = match;
                shortestMatch = match;
                closestMatch = match;
            }
            if(longestMatch == null || match.MatchDuration > longestMatch.MatchDuration) {
                longestMatch = match;
            }
            if(shortestMatch == null || match.MatchDuration < shortestMatch.MatchDuration) {
                shortestMatch = match;
            }
            if(closestMatch == null || match.LoserProgress > closestMatch.LoserProgress) {
                closestMatch = match;
            }

            if(match.IsSpectated) {
                spectatedMatchCount++;
                continue;
            }

            if(mostKills == null || match.LocalPlayerStats?.Kills > mostKills.LocalPlayerStats?.Kills
                || (match.LocalPlayerStats?.Kills == mostKills.LocalPlayerStats?.Kills && match.MatchDuration < mostKills.MatchDuration)) {
                mostKills = match;
            }
            if(mostDeaths == null || match.LocalPlayerStats?.Deaths > mostDeaths.LocalPlayerStats?.Deaths
                || (match.LocalPlayerStats?.Deaths == mostDeaths.LocalPlayerStats?.Deaths && match.MatchDuration < mostDeaths.MatchDuration)) {
                mostDeaths = match;
            }
            if(mostAssists == null || match.LocalPlayerStats?.Assists > mostAssists.LocalPlayerStats?.Assists
                || (match.LocalPlayerStats?.Assists == mostAssists.LocalPlayerStats?.Assists && match.MatchDuration < mostAssists.MatchDuration)) {
                mostAssists = match;
            }
            if(mostDamageDealt == null || match.LocalPlayerStats?.DamageDealt > mostDamageDealt.LocalPlayerStats?.DamageDealt) {
                mostDamageDealt = match;
            }
            if(mostDamageTaken == null || match.LocalPlayerStats?.DamageTaken > mostDamageTaken.LocalPlayerStats?.DamageTaken) {
                mostDamageTaken = match;
            }
            if(mostHPRestored == null || match.LocalPlayerStats?.HPRestored > mostHPRestored.LocalPlayerStats?.HPRestored) {
                mostHPRestored = match;
            }
            if(mostTimeOnCrystal == null || match.LocalPlayerStats?.TimeOnCrystal > mostTimeOnCrystal.LocalPlayerStats?.TimeOnCrystal) {
                mostTimeOnCrystal = match;
            }
            if(match.IsWin && (closestWin == null || match.LoserProgress > closestWin.LoserProgress)) {
                closestWin = match;
            }
            if(match.IsLoss && (closestLoss == null || match.LoserProgress > closestLoss.LoserProgress)) {
                closestLoss = match;
            }

            if(match.IsWin) {
                currentWinStreak++;
                if(currentWinStreak > longestWinStreak) {
                    longestWinStreak = currentWinStreak;
                }
            } else {
                currentWinStreak = 0;
            }
            if(match.IsLoss) {
                currentLossStreak++;
                if(currentLossStreak > longestLossStreak) {
                    longestLossStreak = currentLossStreak;
                }
            } else {
                currentLossStreak = 0;
            }
        }

        try {
            _refreshLock.WaitAsync();
            var addSuperlative = (CrystallineConflictMatch? match, string sup, string val) => {
                if(match == null) return;
                if(Superlatives.ContainsKey(match)) {
                    Superlatives[match].Add((sup, val));
                } else {
                    //_plugin.Log.Debug($"adding superlative {sup} {val} to {match.Id.ToString()}");
                    Superlatives.Add(match, new() { (sup, val) });
                }
            };
            Superlatives = new();
            if(longestMatch != null) {
                addSuperlative(longestMatch, "最长的比赛", ImGuiHelper.GetTimeSpanString((TimeSpan)longestMatch!.MatchDuration));
                addSuperlative(shortestMatch, "最短的比赛", ImGuiHelper.GetTimeSpanString((TimeSpan)shortestMatch!.MatchDuration));
                addSuperlative(closestMatch, "最接近的比分", closestMatch!.LoserProgress.ToString());
                if(mostKills != null) {
                    addSuperlative(mostKills, "最多击杀", mostKills!.LocalPlayerStats!.Kills.ToString());
                    addSuperlative(mostDeaths, "最多死亡", mostDeaths!.LocalPlayerStats!.Deaths.ToString());
                    addSuperlative(mostAssists, "最多协助", mostAssists!.LocalPlayerStats!.Assists.ToString());
                    addSuperlative(mostDamageDealt, "造成的最多伤害", mostDamageDealt!.LocalPlayerStats!.DamageDealt.ToString());
                    addSuperlative(mostDamageTaken, "承受的最多伤害", mostDamageTaken!.LocalPlayerStats!.DamageTaken.ToString());
                    addSuperlative(mostHPRestored, "恢复的最多生命值", mostHPRestored!.LocalPlayerStats!.HPRestored.ToString());
                    addSuperlative(mostTimeOnCrystal, "最长的水晶时间", ImGuiHelper.GetTimeSpanString(mostTimeOnCrystal!.LocalPlayerStats!.TimeOnCrystal));
                }
            }
            LongestWinStreak = longestWinStreak;
            LongestLossStreak = longestLossStreak;
        } finally {
            _refreshLock.Release();
        }
    }

    public void Draw() {
        if(!_refreshLock.Wait(0)) {
            return;
        }
        try {
            using(var table = ImRaii.Table("streaks", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.NoClip | ImGuiTableFlags.NoSavedSettings)) {
                if(table) {
                    ImGui.TableSetupColumn("标题", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 158f);
                    ImGui.TableSetupColumn($"value", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 50f);

                    ImGui.TableNextColumn();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "最长连胜：");
                    ImGui.TableNextColumn();
                    ImGui.Text(LongestWinStreak.ToString());
                    ImGui.TableNextColumn();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, "最长连败：");
                    ImGui.TableNextColumn();
                    ImGui.Text(LongestLossStreak.ToString());
                }
            }
            ImGui.Separator();
            foreach(var match in Superlatives) {
                var x = match.Value;
                DrawStat(match.Key, match.Value.Select(x => x.Item1).ToArray(), match.Value.Select(x => x.Item2).ToArray());
                ImGui.Separator();
            }
        } finally {
            _refreshLock.Release();
        }
    }

    private void DrawStat(CrystallineConflictMatch match, string[] superlatives, string[] values) {
        using(var table = ImRaii.Table("headertable", 3, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.NoClip | ImGuiTableFlags.NoSavedSettings)) {
            if(table) {
                ImGui.TableSetupColumn("title", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 158f);
                ImGui.TableSetupColumn($"value", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 50f);
                ImGui.TableSetupColumn($"examine", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 45f);

                for(int i = 0; i < superlatives.Length; i++) {
                    ImGui.TableNextColumn();
                    ImGui.AlignTextToFramePadding();
                    ImGui.TextColored(ImGuiColors.DalamudYellow, superlatives[i] + ":");
                    ImGui.TableNextColumn();
                    ImGui.Text(values[i]);
                    ImGui.TableNextColumn();
                    if(i == superlatives.Length - 1) {
                        using(var font = ImRaii.PushFont(UiBuilder.IconFont)) {
                            if(ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##{match.GetHashCode()}--ViewDetails")) {
                                _plugin.WindowManager.OpenMatchDetailsWindow(match);
                            }
                        }
                    }
                }
            }
        }

        using(var table = ImRaii.Table("对局", 6, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.NoClip | ImGuiTableFlags.NoSavedSettings)) {
            if(table) {
                ImGui.TableSetupColumn("时间", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 158f);
                ImGui.TableSetupColumn("地图", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 100f);
                ImGui.TableSetupColumn("职业", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 40f);
                ImGui.TableSetupColumn("结果", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 40f);

                ImGui.TableNextColumn();
                ImGui.Text($"{match.DutyStartTime:MM/dd/yyyy HH:mm}");
                ImGui.TableNextColumn();
                if(match.Arena != null) {
                    ImGui.Text(MatchHelper.GetArenaName((CrystallineConflictMap)match.Arena));
                }
                ImGui.TableNextColumn();
                if(!match.IsSpectated) {
                    string jobChineseName = match.LocalPlayerTeamMember?.Job.HasValue == true ? PlayerJobHelper.ChineseNameMap.GetValueOrDefault(match.LocalPlayerTeamMember.Job.Value, "") : "";
                    ImGui.TextColored(ImGuiHelper.GetJobColor(match.LocalPlayerTeamMember?.Job), jobChineseName);
                    ImGui.TableNextColumn();
                    var color = match.IsWin ? ImGuiColors.HealerGreen : match.IsLoss ? ImGuiColors.DalamudRed : ImGuiColors.DalamudGrey;
                    var result = match.IsWin ? "胜利" : match.IsLoss ? "失败" : "???";
                    ImGui.TextColored(color, result);
                } else {
                    ImGui.Text($"观战");
                }
            }
        }
    }
}
