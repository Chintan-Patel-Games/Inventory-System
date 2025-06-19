
using System.Collections.Generic;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

using GDS.Core;
using GDS.Core.Events;


namespace GDS.Unused {

    public class PoeTooltipView : VisualElement {
        public PoeTooltipView(VisualElement root) {

            // styleSheets.Add(Resources.Load<StyleSheet>("Styles/PoeTooltip"));

            this.root = root;
            Tooltip = Dom.Div("tooltip");

            this.
                Div("tooltip-container",
                    Tooltip);
            this.Hide();

        }

        VisualElement root;
        VisualElement Tooltip;
        Task delayTask;
        CancellationTokenSource cts = new();

        public void Render(Item item) {
            // Util.Log("should render tooltip for item", data);



            Tooltip.Clear();
            Tooltip.Div(
                ItemName(item),
                Requirements(item),
                Affixes(item)
            ).IgnorePick();
        }

        void SetPosition(Rect bounds) {
            float left, top;
            top = bounds.yMin - worldBound.height - 21;
            left = bounds.center.x - worldBound.width / 2;

            if (top < 0) {
                top = 0;
                left = bounds.xMin - worldBound.width;
                if (left < 0) {
                    left = bounds.xMax;
                }
            } else {
                if (left < 0) {
                    left = 0;
                }
                if (left + worldBound.width > root.worldBound.width) {
                    left = root.worldBound.width - worldBound.width;
                }
            }

            style.left = left;
            style.top = top;
            style.visibility = Visibility.Visible;
        }

        VisualElement ItemName(Item item) { return Dom.Label("name-line", item.ItemBase.Name); }
        VisualElement Requirements(Item item) {
            var requirements = string.Join(", ", AffixFn.GenerateRequirements());
            return Dom.Label("requirement-line", requirements);
        }

        VisualElement Affixes(Item item) {
            var affixes = AffixFn.GenerateAffixes()
                .Select(text => Dom.Label("affix-line", text));
            return Dom.Div(affixes.ToArray());
        }


    }
}