using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    public class TokenCollection : IEnumerable
    {
        public TokenCollection()
        {
            tokens = new List<IToken>();
        }

        public TokenCollection(TokenCollection existing)
        {
            tokens = new List<IToken>();
            foreach (IToken t in existing)
            {
                tokens.Add(t);
            }
        }

        public TokenCollection Merge(TokenCollection collection)
        {
            TokenCollection merged = new TokenCollection(collection);
            foreach (IToken t in tokens)
            {
                merged.Add(t);
            }
            return merged;
        }

        public void Add(IToken token)
        {
            tokens.Add(token);
        }

        public void Add(TokenCollection collection)
        {
            foreach (IToken t in collection)
            {
                tokens.Add(t);
            }
        }

        public IToken? ContainsExactMatch(IToken token)
        {
            foreach (IToken match in tokens)
            {
                // Exact match only. TODO: Consider making more flexible.
                if (token.Name == match.Name && (token.SupportedTypes & match.SupportedTypes) != Types.None)
                {
                    return match;
                }
            }
            return null;
        }

        public IToken? ContainsTypeMatch(IToken token)
        {
            foreach (IToken match in tokens)
            {
                // Exact match only. TODO: Consider making more flexible.
                if ((token.SupportedTypes & match.SupportedTypes) != Types.None)
                {
                    return match;
                }
            }
            return null;
        }

        public IToken? ContainsCustomMatch(IToken token, Func<IToken, IToken, bool> equal)
        {
            foreach (IToken match in tokens)
            {
                if (equal(token, match))
                {
                    return match;
                }
            }
            return null;
        }

        // TODO: Assumes uniqueness.
        public IToken? GetByName(string name)
        {
            foreach (IToken match in tokens)
            {
                // Exact match only. TODO: Consider making more flexible.
                if (match.Name == name)
                {
                    return match;
                }
            }
            return null;
        }

        private List<IToken> tokens;
        public IEnumerator GetEnumerator()
        {
            return tokens.GetEnumerator();
        }
    }
}
