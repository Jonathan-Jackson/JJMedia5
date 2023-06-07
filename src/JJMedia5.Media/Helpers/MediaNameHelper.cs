using System;
using System.Collections.Generic;
using System.Linq;

namespace JJMedia5.Media.Helpers {

    public static class MediaNameHelper {

        public static bool IsRomanCharacter(char value)
            => value == 'x' || value == 'X'
            || value == 'i' || value == 'I'
            || value == 'v' || value == 'V';

        public static string RemoveEpisode(string value) {
            string output = value.Trim();

            // this certainly needs to be more complex in the future.. let's just be lazy
            if (output.LastIndexOf('-') > 2) {
                output = output.Substring(0, output.LastIndexOf('-'));
            }

            return output;
        }

        public static string RemoveMetadata(string value) {
            // the start & end may contain (, [ - replace them.
            string output = value;

            while (output.StartsWith('[')) {
                int end = output.IndexOf(']');
                if (end > 0 && end < output.Length - 5) {
                    output = output.Substring(end + 1).Trim();
                }
            }

            while (output.EndsWith(']')) {
                int end = output.LastIndexOf('[');
                if (end > 0 && end < output.Length - 3) {
                    output = output.Substring(0, end).Trim();
                }
            }

            while (output.StartsWith('(')) {
                int end = output.IndexOf(')');
                if (end > 0 && end < output.Length - 5) {
                    output = output.Substring(end + 1).Trim();
                }
            }

            while (output.EndsWith(')')) {
                int end = output.LastIndexOf('(');
                if (end > 0 && end < output.Length - 3) {
                    output = output.Substring(0, end).Trim();
                }
            }

            return output;
        }

        public static readonly IReadOnlyCollection<string> SpecialSeasonNotations = new HashSet<string> {
            "ova",
            "special",
            "movie",
            "ovd",
            "short"
        };

        public static string RemoveSeasonNotation(string value) {
            string output = value.Trim();

            // this certainly needs to be more complex in the future.. let's just be lazy
            var toRemove = SpecialSeasonNotations.Where(s => output.EndsWith(' ' + s));
            foreach (string name in toRemove) {
                output = output.Substring(0, output.LastIndexOf(name)).Trim();
            }

            // Season 1, Season 2... S1, S2..
            var sentanceSplit = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (sentanceSplit.Length > 1) {
                var lastSentence = sentanceSplit.Last();

                // S1, S2..
                if (lastSentence.StartsWith("s", StringComparison.OrdinalIgnoreCase) && lastSentence.Length > 1) {
                    if (lastSentence.Substring(1).All(char.IsDigit) || lastSentence.Substring(1).All(IsRomanCharacter)) {
                        output = string.Join(' ', sentanceSplit.SkipLast(1));
                    }
                }
                // Season 1, Season 2..
                else if (sentanceSplit.Length > 2 && string.Equals("season", sentanceSplit.SkipLast(1).Last(), StringComparison.OrdinalIgnoreCase)) {
                    if (lastSentence.All(char.IsDigit) || lastSentence.All(IsRomanCharacter)) {
                        output = string.Join(' ', sentanceSplit.SkipLast(2));
                    }
                }
                // Roman numerals - lets just be hacky rather then calculate for now.
                else if (lastSentence.All(IsRomanCharacter)) {
                    output = string.Join(' ', sentanceSplit.SkipLast(1));
                }
            }

            return output;
        }
    }
}