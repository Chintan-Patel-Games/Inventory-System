using UnityEngine.UIElements;

namespace GDS.Core.Views {
    public class TooltipView : Component<BagItem> {
        public TooltipView() {
            this.Add("tooltip", ItemName.WithClass("tooltip-item-name"));
        }
        public Label ItemName = new();
        override public void Render(BagItem bagItem) {
            ItemName.text = bagItem.Item.Name();
        }
    }
}