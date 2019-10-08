using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    public class TokenCollection : IEnumerable<IToken>
    {
        private List<IToken> tokens;
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

        public List<IToken> ContainsExactMatch(IToken token)
        {
            List<IToken> matches = new List<IToken>();
            foreach (IToken match in tokens)
            {
                // Exact match only. TODO: Consider making more flexible.
                if (token.Name == match.Name && (token.SupportedTypes & match.SupportedTypes) != Types.None)
                {
                    matches.Add(match);
                }
            }
            return matches;
        }

        public List<IToken> GetByName(string name)
        {
            List<IToken> matches = new List<IToken>();
            foreach (IToken match in tokens)
            {
                // Exact match only. TODO: Consider making more flexible.
                if (String.Equals(match.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(match);
                }
            }
            return matches;
        }

        public List<IToken> GetByType(Types type)
        {
            List<IToken> matches = new List<IToken>();
            foreach (IToken match in tokens)
            {
                if ((match.SupportedTypes & type) != Types.None)
                {
                    matches.Add(match);
                }
            }
            return matches;
        }

        public int Count()
        {
            return tokens.Count;
        }

        IEnumerator<IToken> IEnumerable<IToken>.GetEnumerator()
        {
            return ((IEnumerable<IToken>)tokens).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<IToken>)tokens).GetEnumerator();
        }
    }
}
