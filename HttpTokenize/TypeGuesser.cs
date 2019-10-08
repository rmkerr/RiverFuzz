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

            bool boolGuess = false;
            if (bool.TryParse(input, out boolGuess))
            {
                supported |= Types.Boolean;
            }

            if (supported == Types.None)
            {
                supported |= Types.String;
            }

            return supported;
        }
    }
}
