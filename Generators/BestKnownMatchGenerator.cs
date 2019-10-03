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
        public IEnumerable<RequestSequence> Generate(List<Request> endpoints, RequestSequence sequence, TokenCollection sequenceResults, List<IRequestTokenizer> tokenizers)
        {
            //Console.WriteLine($"Beginning generation.");
            foreach (Request candidate in endpoints)
            {
                bool match = true;
                TokenCollection requirements = candidate.GetRequirements(tokenizers);
                Stage candidateStage = new Stage(candidate);
                foreach (IToken token in requirements)
                {
                    IToken? tokenMatch = sequenceResults.GetByName(token.Name);
                    if (tokenMatch != null)
                    {
                        candidateStage.Substitutions.Add(new SubstituteNamedToken(token, tokenMatch.Name, token.SupportedTypes));
                        //Console.WriteLine($"{token.Name} matched.");
                    }
                    else
                    {
                        tokenMatch = sequenceResults.GetByType(token.SupportedTypes);
                        if (tokenMatch != null)
                        {
                            candidateStage.Substitutions.Add(new SubstituteNamedToken(token, tokenMatch.Name, token.SupportedTypes));
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
