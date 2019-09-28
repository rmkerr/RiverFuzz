using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    class TokenCollection : IEnumerable
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

        private List<IToken> tokens;
        public IEnumerator GetEnumerator()
        {
            return tokens.GetEnumerator();
        }
    }
}
