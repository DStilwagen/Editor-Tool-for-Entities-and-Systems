using UnityEditor.IMGUI.Controls;

namespace ECSTools.ListViews.Data {
    
    //Placeholder for system title element will later have a search bar added to this element.
    public class SystemTitleElement : TreeViewItem
    {
        public SystemTitleElement(int id, int depth, string displayName) : base(id, depth, displayName)
        {
        }
    }
}