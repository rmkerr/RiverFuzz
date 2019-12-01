using HttpTokenize;
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

        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults)
        {
            // Only build off sequences that have an actual result.
            if (sequence.GetLastResponse() == null || (int)sequence.GetLastResponse().Status < 300)
            {
                // Search for an endpoint that we can append to the end of this request sequence.
                foreach (RequestResponsePair endpoint in endpoints)
                {
                    bool foundMatch = true;
                    TokenCollection requirements = endpoint.InputTokens;
                    Stage candidateStage = new Stage(endpoint.Request);

                    foreach (IToken token in requirements)
                    {
                        int matchIndex = 0;
                        IToken? matchToken = null;

                        // Substituting a token with the same name is usually a good bet, but sometimes gets stuck if there is a false positive.
                        // Add some level of randomness to keep things working. 
                        int skip = Rand.Next(0, 2);

                        if (skip >= 2 && GetRandomNameMatch(sequenceResults, token.Name, out matchToken, out matchIndex))
                        {
                            // Find a token that has the same name as this one.
                            candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                        }
                        else if ( skip >= 1 && GetRandomTypeMatch(sequenceResults, token.SupportedTypes, out matchToken, out matchIndex))
                        {
                            // Find a token that has the same type as this one.
                            candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                        }
                        else
                        {
                            // Use the original value from the example request.
                            candidateStage.Substitutions.Add(new SubstituteConstant(token, token.Value));
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
        }

        // TODO: Combine this with the one below, since it's basically copied and pasted.
        private bool GetRandomNameMatch(List<TokenCollection> source, string name, out IToken? selection, out int sourceCollection)
        {
            List<Tuple<int, IToken>> matches = new List<Tuple<int, IToken>>();

            for (int sourceIndex = 0; sourceIndex < source.Count; ++sourceIndex)
            {
                List<IToken> searchResults = source[sourceIndex].GetByName(name);
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

        private bool GetRandomTypeMatch(List<TokenCollection> source, Types supported, out IToken? selection, out int sourceCollection)
        {
            List<Tuple<int, IToken>> matches = new List<Tuple<int, IToken>>();

            for (int sourceIndex = 0; sourceIndex < source.Count; ++sourceIndex)
            {
                List<IToken> searchResults = source[sourceIndex].GetByType(supported);
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
