using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Substitutions
{
    public interface ISubstitution
    {
        public void MakeSubstitution(List<TokenCollection> previous, Request next);

        // TODO: Rework this. It's odd that we don't initialize with a token, but
        // we compare to one here.
        public bool ReplacesToken(IToken token);
    }
}
