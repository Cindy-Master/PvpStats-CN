using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using PvpStats.Helpers;
using PvpStats.Types.Match;

namespace PvpStats.Windows.Summary;
internal class CrystallineConflictPvPProfile {

    private Plugin _plugin;

    public CrystallineConflictPvPProfile(Plugin plugin) {
        _plugin = plugin;
    }

    public void Refresh() {

    }

    public unsafe void Draw() {
        var pvpProfile = PvPProfile.Instance();
        ImGuiHelper.HelpMarker("使用数据来自你的pvp文件", false);
        ImGui.TextColored(ImGuiColors.DalamudYellow, "练习赛:");
        if(pvpProfile != null) {
            DrawTable(pvpProfile->CrystallineConflictCasualMatches, pvpProfile->CrystallineConflictCasualMatchesWon);
        }
        ImGui.Separator();
        ImGui.TextColored(ImGuiColors.DalamudYellow, "当前赛季段位:");
        if(pvpProfile != null) {
            DrawTable(pvpProfile->CrystallineConflictRankedMatches, pvpProfile->CrystallineConflictRankedMatchesWon);
            ImGui.NewLine();
            using(var table = ImRaii.Table("练习赛", 2, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.NoClip | ImGuiTableFlags.NoSavedSettings)) {
                if(table) {
                    ImGui.TableSetupColumn("简介", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 158f);
                    ImGui.TableSetupColumn($"value", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 45f);
                    ImGui.TableNextColumn();
                    ImGui.Text("当前段位: ");
                    ImGui.TableNextColumn();
                    PlayerRank curRank = new() {
                        Tier = (ArenaTier)pvpProfile->CrystallineConflictCurrentRank,
                        Riser = pvpProfile->CrystallineConflictCurrentRiser,
                        Stars = pvpProfile->CrystallineConflictCurrentRisingStars,
                        Credit = pvpProfile->CrystallineConflictCurrentCrystalCredit
                    };

                    ImGui.Text($"{curRank}");
                    ImGui.TableNextColumn();
                    ImGui.Text("最高等级: ");
                    ImGui.TableNextColumn();
                    PlayerRank peakRank = new() {
                        Tier = (ArenaTier)pvpProfile->CrystallineConflictHighestRank,
                        Riser = pvpProfile->CrystallineConflictHighestRiser,
                        Stars = pvpProfile->CrystallineConflictHighestRisingStars,
                        Credit = pvpProfile->CrystallineConflictHighestCrystalCredit
                    };
                    ImGui.Text($"{peakRank}");
                }
            }
        }
    }

    private void DrawTable(int matches, int wins) {
        using(var table = ImRaii.Table("练习赛", 3, ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.NoClip | ImGuiTableFlags.NoSavedSettings)) {
            if(table) {
                ImGui.TableSetupColumn("简介", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 158f);
                ImGui.TableSetupColumn($"value", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 45f);
                ImGui.TableSetupColumn($"rate", ImGuiTableColumnFlags.WidthFixed, ImGuiHelpers.GlobalScale * 45f);

                ImGui.TableNextColumn();
                ImGui.Text("比赛: ");
                ImGui.TableNextColumn();
                ImGui.Text($"{matches.ToString("N0")}");
                ImGui.TableNextColumn();

                ImGui.TableNextColumn();
                ImGui.Text("胜利: ");
                ImGui.TableNextColumn();
                ImGui.Text($"{wins.ToString("N0")}");
                ImGui.TableNextColumn();
                if(matches > 0) {
                    ImGui.Text($"{string.Format("{0:P}%", (double)wins / (matches))}");
                }
            }
        }
    }
}
