using ImGuiNET;
using System;

namespace PvpStats.Windows.Filter;
public class LocalPlayerFilter : DataFilter {
    public override string Name => "当前角色";

    public override string HelpMessage => "只包括使用当前登录角色的比赛。如果您使用多个角色并希望分别查看结果,大人请用这个。";
    public bool CurrentPlayerOnly { get; set; }
    public LocalPlayerFilter() { }

    internal LocalPlayerFilter(Plugin plugin, Action action, LocalPlayerFilter? filter = null) : base(plugin, action) {
        if(filter is not null) {
            CurrentPlayerOnly = filter.CurrentPlayerOnly;
        }
    }

    internal override void Draw() {
        bool currentPlayerOnly = CurrentPlayerOnly;
        if(ImGui.Checkbox("仅显示当前角色", ref currentPlayerOnly)) {
            _plugin!.DataQueue.QueueDataOperation(() => {
                CurrentPlayerOnly = currentPlayerOnly;
                Refresh();
            });
        }
    }
}
