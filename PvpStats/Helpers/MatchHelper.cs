﻿using PvpStats.Types.Match;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PvpStats.Helpers;
public static class MatchHelper {

    public static readonly Dictionary<uint, CrystallineConflictMap> CrystallineConflictMapLookup = new() {
        { 1032, CrystallineConflictMap.Palaistra },
        { 1058, CrystallineConflictMap.Palaistra }, //custom match
        { 1033, CrystallineConflictMap.VolcanicHeart },
        { 1059, CrystallineConflictMap.VolcanicHeart }, //custom match
        { 1034, CrystallineConflictMap.CloudNine },
        { 1060, CrystallineConflictMap.CloudNine }, //custom match
        { 1116, CrystallineConflictMap.ClockworkCastleTown },
        { 1117, CrystallineConflictMap.ClockworkCastleTown }, //custom match
        { 1138, CrystallineConflictMap.RedSands },
        { 1139, CrystallineConflictMap.RedSands } //custom match
    };

    public static readonly Dictionary<ArenaTier, string> ArenaRankLookup = new() {
        { ArenaTier.Bronze, "青铜" },
        { ArenaTier.Silver, "白银" },
        { ArenaTier.Gold, "黄金" },
        { ArenaTier.Platinum, "白金" },
        { ArenaTier.Diamond, "钻石" },
        { ArenaTier.Crystal, "水晶" }
    };

    public static CrystallineConflictMap? GetArena(uint territoryId) {
        if(CrystallineConflictMapLookup.ContainsKey(territoryId)) {
            return CrystallineConflictMapLookup[territoryId];
        } else {
            return null;
        }
    }
    public static Dictionary<CrystallineConflictMatchType, string> MatchTypeChineseMap = new Dictionary<CrystallineConflictMatchType, string>()
{
    { CrystallineConflictMatchType.Casual, "练习赛" },
    { CrystallineConflictMatchType.Ranked, "段位赛" },
    { CrystallineConflictMatchType.Custom, "自定义" },
    { CrystallineConflictMatchType.Unknown, "未知" }
};

    public static string GetMatchTypeChinese(CrystallineConflictMatchType matchType) {
        if(MatchTypeChineseMap.TryGetValue(matchType, out var chineseName)) {
            return chineseName;
        }
        return "未知"; // 默认值，防止字典中没有对应的枚举值
    }


    public static CrystallineConflictMatchType GetMatchType(uint dutyId) {
        switch(dutyId) {
            case 835: //palaistra
            case 836: //volcanic heart
            case 837: //cloud 9
            case 912: //clockwork castletown
            case 967: //red sands
                return CrystallineConflictMatchType.Casual;
            case 838: //palaistra (assumed)
            case 841: //palaistra (assumed)
            case 847: //palaistra (assumed)
            case 850: //palaistra
            case 853: //palaistra
            case 856: //palaistra
            case 859: //palaistra (assumed)
            case 839: //volcanic heart (assumed)
            case 842: //volcanic heart (assumed)
            case 848: //volcanic heart (assumed)
            case 851: //volcanic heart (assumed)
            case 854: //volcanic heart
            case 857: //volcanic heart
            case 860: //volcanic heart (assumed)
            case 840: //cloud 9 (assumed)
            case 843: //cloud 9 (assumed)
            case 849: //cloud 9 (assumed)
            case 852: //cloud 9
            case 855: //cloud 9
            case 858: //cloud 9
            case 861: //cloud 9 (assumed)
            case 913: //clockwork castletown (assumed)
            case 914: //clockwork castletown (assumed)
            case 915: //clockwork castletown (assumed)
            case 916: //clockwork castletown (assumed)
            case 917: //clockwork castletown
            case 918: //clockwork castletown
            case 919: //clockwork castletown (assumed)
            case 920: //clockwork castletown (assumed)
            case 921: //clockwork castletown (assumed)
            case 922: //clockwork castletown (assumed)
            case 968: //red sands (assumed)
            case 969: //red sands (assumed)
            case 970: //red sands
            case 971: //red sands (assumed)
            case 972: //red sands
            case 973: //red sands
            case 974: //red sands (assumed)
            case 975: //red sands (assumed)
            case 976: //red sands (assumed)
            case 977: //red sands (assumed)
                return CrystallineConflictMatchType.Ranked;
            case 862: //palaistra
            case 863: //volcanic heart
            case 864: //cloud 9
            case 923: //clockwork castletown
            case 978: //red sands
                return CrystallineConflictMatchType.Custom;
            default:
                return CrystallineConflictMatchType.Unknown;
        }
    }

    public static bool IsCrystallineConflictTerritory(uint territoryId) {
        return CrystallineConflictMapLookup.ContainsKey(territoryId);
    }

    public static string GetArenaName(CrystallineConflictMap map) {
        return map switch {
            CrystallineConflictMap.Palaistra => "角力学校",
            CrystallineConflictMap.VolcanicHeart => "火山之心",
            CrystallineConflictMap.CloudNine => "九霄云上",
            CrystallineConflictMap.ClockworkCastleTown => "机关大殿",
            CrystallineConflictMap.RedSands => "赤土红砂",
            _ => "Unknown",
        };
    }

    public static CrystallineConflictTeamName GetTeamName(string name) {
        name = name.ToLower().Trim();
        if(Regex.IsMatch(name, @"\bastra\b", RegexOptions.IgnoreCase)) {
            return CrystallineConflictTeamName.Astra;
        } else if(Regex.IsMatch(name, @"\bumbra\b", RegexOptions.IgnoreCase)) {
            return CrystallineConflictTeamName.Umbra;
        }
        return CrystallineConflictTeamName.Unknown;
    }

    public static string GetTeamName(CrystallineConflictTeamName team) {
        switch(team) {
            case CrystallineConflictTeamName.Astra: return "星极队";
            case CrystallineConflictTeamName.Umbra: return "灵极队";
            default: return "Unknown";
        }
    }

    public static ArenaTier GetTier(string name) {
        name = name.ToLower().Trim();
        foreach(var kvp in ArenaRankLookup) {
            if(kvp.Value == name) {
                return kvp.Key;
            }
        }
        return ArenaTier.None;
    }

    public static float? ConvertProgressStringToFloat(string progress) {
        if(float.TryParse(progress.Replace("%", "").Replace(",", "."), out float parseResult)) {
            return parseResult;
        } else {
            return null;
        }
    }

    public static readonly Regex CreditBeforeRegex = new Regex(@"^\d+(?= →)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static readonly Regex StarBeforeRegex = new Regex(@"(?<=^\w*\s*\d*\s*)★*(?=☆*\s*→)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static readonly Regex RiserBeforeRegex = new Regex(@"(?<=^\w*\s?)\d*(?=\s?(★|☆)* →)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static readonly Regex TierBeforeRegex = new Regex(@"^[^\d\s]+(?=\s?\d*\s?(★|☆)*\s?→)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static readonly Regex CreditAfterRegex = new Regex(@"(?<=→ )\d+", RegexOptions.IgnoreCase);
    public static readonly Regex StarAfterRegex = new Regex(@"★*(?=☆*$)", RegexOptions.IgnoreCase);
    public static readonly Regex RiserAfterRegex = new Regex(@"\d*(?=\s?(★|☆)*$)", RegexOptions.IgnoreCase);
    public static readonly Regex TierAfterRegex = new Regex(@"(?<=→\s)[^\d\s]+", RegexOptions.IgnoreCase);
}
