using ImGuiNET;
using System;

namespace PvpStats.Windows.Filter;

public class BookmarkFilter : DataFilter {
    public override string Name => "收藏的对局";
    public bool BookmarkedOnly { get; set; } = false;

    public BookmarkFilter() { }

    internal BookmarkFilter(Plugin plugin, Action action, BookmarkFilter? filter = null) : base(plugin, action) {
        if(filter is not null) {
            BookmarkedOnly = filter.BookmarkedOnly;
        }
    }

    internal override void Draw() {
        bool bookMarkedOnly = BookmarkedOnly;
        if(ImGui.Checkbox("", ref bookMarkedOnly)) {
            _plugin!.DataQueue.QueueDataOperation(() => {
                BookmarkedOnly = bookMarkedOnly;
                Refresh();
            });
        }
    }
}
