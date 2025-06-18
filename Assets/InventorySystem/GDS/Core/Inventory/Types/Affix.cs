using System;
using System.Collections.Generic;
using System.Linq;

namespace GDS.Unused {
    abstract public record Requirement(int Value);
    public record LevelRequirement(int Value) : Requirement(Value);
    public record StrRequirement(int Value) : Requirement(Value);
    public record DexRequirement(int Value) : Requirement(Value);
    public record IntRequirement(int Value) : Requirement(Value);

    public record Affix(string Value);

    public static class AffixFn {
        public static List<string> Common = new(){
"+{0} to Strength",
"+{0} to Dexterity",
"+{0} to Intelligence",
"+{0} to Maximum Life",
"+{0} to All Skills",
"+{0} to Mana after each Kill",
"+{0}% to Lightning Resistance",
"+{0}% to Fire Resistance",
"+{0}% to Cold Resistance",
"+{0}% to Chaos Resistance",
"Regenerate {0} Life per second",
"Reflects {0} Damage to attackers",
        };
        public static List<string> Weapon = new() { };
        public static List<string> Boots = new() { };

        static int GetRandomValue() => UnityEngine.Random.Range(1, 75);

        static string GetRandomAffix(List<string> list) {
            var index = UnityEngine.Random.Range(0, list.Count - 1);
            return list[index];
        }

        static string CreateReqText(Requirement req) => req switch {
            LevelRequirement => $"Requires Level <color=#fefefe>{req.Value}</color>",
            StrRequirement => $"<color=#fefefe>{req.Value}</color> Str",
            DexRequirement => $"<color=#D20000>{req.Value}</color> Dex",
            IntRequirement => $"<color=#fefefe>{req.Value}</color> Int",
            _ => ""
        };

        public static List<string> GenerateAffixes() {
            var num = UnityEngine.Random.Range(2, 6);
            // Util.Log("generated affixes", num);
            // var items = Enumerable.Range(0, num).Select(_ => GetRandomAffix(Common));
            var randomItems =
                Common.
                    OrderBy(x => new Random().Next()).
                    Take(num).
                    Select(str => string.Format(str, GetRandomValue())).
                    ToList();
            return randomItems.ToList();
        }

        public static List<string> GenerateRequirements() {
            var allreq = new List<Requirement>(){
                new StrRequirement(GetRandomValue()),
                new DexRequirement(GetRandomValue()),
                new IntRequirement(GetRandomValue()),
            };
            var rnd = UnityEngine.Random.Range(0, 3);
            var req = new List<Requirement>() {
                new LevelRequirement(GetRandomValue()),
                allreq[rnd]
            };
            // var filteredList = list.Where(item => UnityEngine.Random.value < probability).ToList();

            // Util.Log(req);

            return req.Select(CreateReqText).ToList();
        }



    }
}