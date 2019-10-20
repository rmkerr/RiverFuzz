using HttpTokenize;
using HttpTokenize.RequestSequence;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generators
{
    public class BestKnownMatchGenerator : IGenerator
    {
        private Random Rand;
        public BestKnownMatchGenerator()
        {
            Rand = new Random();
        }

        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, TokenCollection initialTokens)
        {
            foreach (RequestResponsePair endpoint in endpoints)
            {
                bool foundMatch = true;
                TokenCollection requirements = endpoint.InputTokens;
                Stage candidateStage = new Stage(endpoint.Request);
                foreach (IToken token in requirements)
                {
                    int matchIndex = 0;
                    IToken? matchToken = null;

                    // Name only is usually a good bet, but sometimes gets stuck if there is a false positive.
                    bool useNameOnly = Rand.Next(0, 1) > 0;

                    if (useNameOnly && GetRandomNameMatch(sequence, token.Name, out matchToken, out matchIndex))
                    {
                        candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                    }
                    else if (GetRandomTypeMatch(sequence, token.SupportedTypes, out matchToken, out matchIndex))
                    {
                        candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                    }
                    else
                    {
                        foundMatch = false;
                        break;
                    }
                }
                if (foundMatch)
                {
                    RequestSequence newSequence = sequence.Copy();
                    newSequence.Add(candidateStage);
                    yield return newSequence;
                }
            }
        }

        // TODO: Combine this with the one below, since it's basically copied and pasted.
        private bool GetRandomNameMatch(RequestSequence source, string name, out IToken? selection, out int sourceCollection)
        {
            List<Tuple<int, IToken>> matches = new List<Tuple<int, IToken>>();

            List<IToken> searchResults = source.InitialTokens.GetByName(name);
            foreach (IToken token in searchResults)
            {
                matches.Add(new Tuple<int, IToken>(0, token));
            }

            for (int sourceIndex = 1; sourceIndex <= source.Responses.Count; ++sourceIndex)
            {
                searchResults = source.Responses[sourceIndex].Results.GetByName(name);
                foreach (IToken token in searchResults)
                {
                    matches.Add(new Tuple<int, IToken>(sourceIndex, token));
                }
            }
            
            // Select a match at random.
            if (matches.Count > 0)
            {
                int index = Rand.Next(0, matches.Count);
                sourceCollection = matches[index].Item1;
                selection = matches[index].Item2;
                return true;
            }
            else
            {
                // No matches.
                selection = null;
                sourceCollection = 0;
                return false;
            }
        }

        private bool GetRandomTypeMatch(RequestSequence source, Types supported, out IToken? selection, out int sourceCollection)
        {
            List<Tuple<int, IToken>> matches = new List<Tuple<int, IToken>>();

            for (int sourceIndex = 0; sourceIndex < source.Count; ++sourceIndex)
            {
                List<IToken> searchResults = source[sourceIndex].Results.GetByType(supported);
                foreach (IToken token in searchResults)
                {
                    matches.Add(new Tuple<int, IToken>(sourceIndex, token));
                }
            }

            // Select a match at random.
            if (matches.Count > 0)
            {
                int index = Rand.Next(0, matches.Count);
                sourceCollection = matches[index].Item1;
                selection = matches[index].Item2;
                return true;
            }
            else
            {
                // No matches.
                selection = null;
                sourceCollection = 0;
                return false;
            }
        }
    }
}
