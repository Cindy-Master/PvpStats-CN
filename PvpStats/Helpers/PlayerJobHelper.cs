using PvpStats.Types.Player;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PvpStats.Helpers;
internal static class PlayerJobHelper {


    internal static Dictionary<Job, string> ChineseNameMap = new Dictionary<Job, string>() {
        { Job.PLD, "骑士" },
        { Job.WAR, "战士" },
        { Job.DRK, "暗黑骑士" },
        { Job.GNB, "绝枪战士" },
        { Job.MNK, "武僧" },
        { Job.DRG, "龙骑士" },
        { Job.NIN, "忍者" },
        { Job.SAM, "武士" },
        { Job.RPR, "钐镰客" },
        { Job.VPR, "Viper" },
        { Job.WHM, "白魔法师" },
        { Job.SCH, "学者" },
        { Job.AST, "占星术士" },
        { Job.SGE, "贤者" },
        { Job.BRD, "吟游诗人" },
        { Job.MCH, "机工士" },
        { Job.DNC, "舞者" },
        { Job.BLM, "黑魔法师" },
        { Job.SMN, "召唤师" },
        { Job.RDM, "赤魔法师" },
        { Job.PCT, "画师" },
    };

    internal static Dictionary<JobSubRole, string> JobSubRoleChineseMap = new Dictionary<JobSubRole, string> {
        { JobSubRole.TANK, "坦克" },
        { JobSubRole.HEALER, "治疗" },
        { JobSubRole.MELEE, "近战" },
        { JobSubRole.RANGED, "远程" },
        { JobSubRole.CASTER, "法师" }
    };

    internal static string GetSubRoleChineseName(Job? job) {
        if(!job.HasValue) return "未知职能";
        JobSubRole? subRole = GetSubRoleFromJob(job.Value);  // 使用已有的方法获取子角色枚举
        if(subRole.HasValue && JobSubRoleChineseMap.ContainsKey(subRole.Value)) {
            return JobSubRoleChineseMap[subRole.Value];  // 返回子角色的中文名称
        } else {
            return "未知职能";  // 如果没有有效的子角色或字典中无此键
        }
    }


    internal static Dictionary<Job, string> AbbreviationNameMap = new Dictionary<Job, string>() {
        { Job.PLD, "Paladin" },
        { Job.WAR, "Warrior" },
        { Job.DRK, "Dark Knight" },
        { Job.GNB, "Gunbreaker" },
        { Job.MNK, "Monk" },
        { Job.DRG, "Dragoon" },
        { Job.NIN, "Ninja" },
        { Job.SAM, "Samurai" },
        { Job.RPR, "Reaper" },
        { Job.VPR, "Viper" },
        { Job.WHM, "White Mage" },
        { Job.SCH, "Scholar" },
        { Job.AST, "Astrologian" },
        { Job.SGE, "Sage" },
        { Job.BRD, "Bard" },
        { Job.MCH, "Machinist" },
        { Job.DNC, "Dancer" },
        { Job.BLM, "Black Mage" },
        { Job.SMN, "Summoner" },
        { Job.RDM, "Red Mage" },
        { Job.PCT, "Pictomancer" },
    };

    internal static Dictionary<JobRole, string> JobRoleName = new Dictionary<JobRole, string>() {
        { JobRole.TANK, "Tank" },
        { JobRole.HEALER, "Healer" },
        { JobRole.DPS, "DPS" },
    };

    internal static Dictionary<JobSubRole, string> JobSubRoleName = new Dictionary<JobSubRole, string>() {
        { JobSubRole.TANK, "Tank" },
        { JobSubRole.HEALER, "Healer" },
        { JobSubRole.MELEE, "Melee" },
        { JobSubRole.RANGED, "Ranged" },
        { JobSubRole.CASTER, "Caster" },
    };

    internal static Dictionary<Job, uint> JobIcons = new() {
        { Job.PLD, 62119 },
        { Job.WAR, 62121 },
        { Job.DRK, 62132 },
        { Job.GNB, 62137 },
        { Job.MNK, 62120 },
        { Job.DRG, 62122 },
        { Job.NIN, 62130 },
        { Job.SAM, 62134 },
        { Job.RPR, 62139 },
        { Job.WHM, 62124 },
        { Job.SCH, 62128 },
        { Job.AST, 62133 },
        { Job.SGE, 62140 },
        { Job.BRD, 62123 },
        { Job.MCH, 62131 },
        { Job.DNC, 62138 },
        { Job.BLM, 62125 },
        { Job.SMN, 62127 },
        { Job.RDM, 62135 },
    };

    public static bool IsAbbreviatedAliasMatch(PlayerAlias abbreviatedPlayer, string fullName) {
        string pattern = "^" + abbreviatedPlayer.FirstName.Replace(".", @"[\w'-]*") + " " + abbreviatedPlayer.LastName.Replace(".", "");
        return Regex.IsMatch(fullName, pattern);
    }

    public static bool IsAbbreviatedAliasMatch(string abbreviatedPlayer, string fullName) {
        return IsAbbreviatedAliasMatch((PlayerAlias)$"{abbreviatedPlayer} whocares", fullName);
    }

    internal static Job? GetJobFromName(string jobName) {
        jobName = jobName.ToLower().Trim();
        foreach(var kvp in AbbreviationNameMap) {
            if(kvp.Value.Equals(jobName, StringComparison.OrdinalIgnoreCase)) {
                return kvp.Key;
            }
        }
        return null;
    }

    internal static string GetNameFromJob(Job job) {
        if(AbbreviationNameMap.ContainsKey(job)) {
            return ChineseNameMap[job];
        } else {
            return "";
        }
    }

    internal static JobRole? GetRoleFromJob(Job job) {
        switch(job) {
            case Job.PLD:
            case Job.WAR:
            case Job.DRK:
            case Job.GNB:
                return JobRole.TANK;
            case Job.WHM:
            case Job.SCH:
            case Job.AST:
            case Job.SGE:
                return JobRole.HEALER;
            case Job.MNK:
            case Job.DRG:
            case Job.NIN:
            case Job.SAM:
            case Job.RPR:
            case Job.BRD:
            case Job.MCH:
            case Job.DNC:
            case Job.BLM:
            case Job.SMN:
            case Job.RDM:
                return JobRole.DPS;
            default:
                return null;
        }
    }

    internal static JobSubRole? GetSubRoleFromJob(Job? job) {
        switch(job) {
            case Job.PLD:
            case Job.WAR:
            case Job.DRK:
            case Job.GNB:
                return JobSubRole.TANK;
            case Job.WHM:
            case Job.SCH:
            case Job.AST:
            case Job.SGE:
                return JobSubRole.HEALER;
            case Job.MNK:
            case Job.DRG:
            case Job.NIN:
            case Job.SAM:
            case Job.RPR:
            case Job.VPR:
                return JobSubRole.MELEE;
            case Job.BRD:
            case Job.MCH:
            case Job.DNC:
                return JobSubRole.RANGED;
            case Job.BLM:
            case Job.SMN:
            case Job.RDM:
            case Job.PCT:
                return JobSubRole.CASTER;
            default:
                return null;
        }
    }

    internal static Job? GetJobFromIcon(uint iconId) {
        foreach(var kvp in JobIcons) {
            if(kvp.Value == iconId) {
                return kvp.Key;
            }
        }
        return null;
    }

    internal static Dictionary<uint, string> WorldNameMap = new();
}
