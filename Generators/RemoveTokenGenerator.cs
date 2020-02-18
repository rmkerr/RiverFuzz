using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokens;

namespace Generators
{
    public class RemoveTokenGenerator : IGenerator
    {
        public int MaxRemovals { get; }

        private Random rand;

        public RemoveTokenGenerator(int removalsPerGen)
        {
            MaxRemovals = removalsPerGen;
            rand = new Random();
        }

        public IEnumerable<RequestSequence> Generate(List<KnownEndpoint> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults)
        {
            for (int i = 0; i < MaxRemovals; ++i)
            {
                if (sequence.StageCount() == 0)
                {
                    continue;
                }

                RequestSequence newSequence = sequence.Copy();
                int selectedStage = rand.Next(0, newSequence.StageCount());

                Stage stage = newSequence.Get(selectedStage);

                if (stage.Substitutions.Count == 0)
                {
                    continue;
                }

                // Pick a random token and remove it.
                int subIndex = rand.Next(0, newSequence.Get(selectedStage).Substitutions.Count);
                ISubstitution sub = newSequence.Get(selectedStage).Substitutions[subIndex];
                sub.GetTarget().DeleteToken(stage.Request);

                yield return newSequence;
            }
            yield break;
        }
    }
}
