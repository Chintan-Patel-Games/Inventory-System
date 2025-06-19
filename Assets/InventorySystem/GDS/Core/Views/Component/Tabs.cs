using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static GDS.Core.Dom;
namespace GDS.Core.Views {

    public record TabData(string Name, VisualElement Content);
    public record TabDataVO(string Name, VisualElement Content, VisualElement Button) : TabData(Name, Content);

    public class TabButtons : Component<int> {

        public TabButtons(List<TabData> tabDataList, Action<int> Callback) {
            Buttons = tabDataList.Select((data, index) => TabButton(data, index, Callback)).ToArray();
            this.Div("tab-buttons", Buttons);
        }

        VisualElement[] Buttons;
        VisualElement TabButton(TabData tabData, int index, Action<int> callback) => Button("tab-button active", tabData.Name, () => callback(index));

        public override void Render(int index) {
            foreach (var button in Buttons) button.WithoutClass("active");
            Buttons[index].WithClass("active");
        }

    }

    public class TabCarousel : Component<int> {

        public TabCarousel(List<TabData> tabDataList, Action<int> PrevCallback, Action<int> NextCallback) {
            var max = tabDataList.Count() - 1;
            Title = Label("label tab-title", "");
            TitleText = tabDataList.Select(data => data.Name).ToArray();
            this.Div("tab-carousel",
                Button("<", () => PrevCallback(max)),
                Title,
                Button(">", () => NextCallback(max))
            );
        }

        Label Title;
        string[] TitleText;

        public override void Render(int index) {
            Title.text = TitleText[index];
        }
    }

    public class TabContents : Component<int> {

        public TabContents(List<TabData> tabDataList) {
            Contents = tabDataList.Select(data => data.Content).ToArray();
            this.Div("tab-content", Contents);
        }

        VisualElement[] Contents;

        public override void Render(int index) {
            foreach (var content in Contents) content.Hide();
            Contents[index].Show();
        }
    }

    public class Tabs {
        public Component<int>[] Components { get; private set; }
        public int CurrentIndex = 0;

        public Tabs Init(params Component<int>[] components) {
            Components = components;
            Render(CurrentIndex);
            return this;
        }

        public void Render(int index) {
            foreach (var c in Components) c.Data = index;
        }

        public void SetIndex(int index) {
            CurrentIndex = index;
            Render(CurrentIndex);
        }

        public void Next(int maxIndex) {
            var nextIndex = CurrentIndex + 1;
            CurrentIndex = nextIndex > maxIndex ? 0 : nextIndex;
            Render(CurrentIndex);
        }

        public void Prev(int maxIndex) {
            var prevIndex = CurrentIndex - 1;
            CurrentIndex = prevIndex < 0 ? maxIndex : prevIndex;
            Render(CurrentIndex);
        }

    }

}