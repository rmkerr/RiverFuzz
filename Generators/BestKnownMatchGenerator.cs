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

                        // Decide if we should use the value of the token from the example request, or if we should try to find
                        // a token from a previous request that would work as a substitution. We should be able to
                        // make a better decision about this based on static analysis of full set of example requests.
                        bool useOriginalValue = Rand.Next(0, 9) > 0;
                        if (useOriginalValue)
                        {
                            candidateStage.Substitutions.Add(new SubstituteConstant(token, token.Value));
                        }
                        else
                        {
                            // Substituting a token with the same name is usually a good bet, but sometimes gets stuck if there is a false positive.
                            // Add some level of randomness to keep things working.
                            bool useNameOnly = Rand.Next(0, 1) > 0;

                            if (useNameOnly && GetRandomNameMatch(sequenceResults, token.Name, out matchToken, out matchIndex))
                            {
                                candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                            }
                            else if (GetRandomTypeMatch(sequenceResults, token.SupportedTypes, out matchToken, out matchIndex))
                            {
                                candidateStage.Substitutions.Add(new SubstituteNamedToken(token, matchToken.Name, matchIndex, token.SupportedTypes));
                            }
                            else
                            {
                                foundMatch = false;
                                break;
                            }
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
