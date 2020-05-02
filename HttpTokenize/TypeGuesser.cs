using HttpTokenize.Tokens;
using System;
using System.Text.RegularExpressions;

namespace HttpTokenize
{
    public class TypeGuesser
    {
        public static Types GuessTypes(string input)
        {
            Types supported = Types.None;

            // Check if it's an integer
            int intGuess = 0;
            if (int.TryParse(input, out intGuess))
            {
                supported |= Types.Integer;

                // Cases that bool.tryparse won't catch.
                if (intGuess == 0 || intGuess == 1)
                {
                    supported |= Types.Boolean;
                }
            }

            // Check if it's a boolean
            bool boolGuess = false;
            if (bool.TryParse(input, out boolGuess))
            {
                supported |= Types.Boolean;
            }

            // Check if it's a URL. We could use Uri.TryParse here, but it's so slow.
            if (input.StartsWith(@"http://") || input.StartsWith(@"https://") || input.StartsWith(@"ws://") || input.StartsWith(@"wss://"))
            {
                supported |= Types.Url;
                supported |= Types.String;
            }

            if (supported == Types.None)
            {
                supported |= Types.String;
            }

            return supported;
        }
    }
}
