using HttpTokenize;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generators
{
    public class BestKnownMatchGenerator
    {
        public BestKnownMatchGenerator()
        {

        }

        // TODO: This seems inefficient.
        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, TokenCollection sequenceResults, List<IRequestTokenizer> tokenizers)
        {
            Random rnd = new Random();
            //Console.WriteLine($"Beginning generation.");
            foreach (RequestResponsePair endpoint in endpoints)
            {
                bool match = true;
                TokenCollection requirements = endpoint.InputTokens;
                Stage candidateStage = new Stage(endpoint.Request);
                foreach (IToken token in requirements)
                {
                    List<IToken> tokenMatch = sequenceResults.GetByName(token.Name);
                    if (tokenMatch.Count != 0)
                    {
                        int selection = rnd.Next(0, tokenMatch.Count);
                        candidateStage.Substitutions.Add(new SubstituteNamedToken(token, tokenMatch[selection].Name, token.SupportedTypes));
                        //Console.WriteLine($"{token.Name} matched by {tokenMatch[selection]}.");
                    }
                    else
                    {
                        tokenMatch = sequenceResults.GetByType(token.SupportedTypes);
                        if (tokenMatch.Count != 0)
                        {
                            candidateStage.Substitutions.Add(new SubstituteNamedToken(token, tokenMatch[rnd.Next(0, tokenMatch.Count)].Name, token.SupportedTypes));
                            //Console.WriteLine($"{token.Name} matched.");
                        }
                        else
                        {
                            //Console.WriteLine($"{token.Name} not matched.");
                            match = false;
                            break;
                        }
                    }
                }
                if (match)
                {
                    RequestSequence newSequence = sequence.Copy();
                    newSequence.Add(candidateStage);
                    yield return newSequence;
                }
            }
        }
    }
}
