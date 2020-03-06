using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokens;

namespace Generators
{
    public class DictionarySubstitutionGenerator : IGenerator
    {
        public int MaxSubstitutions { get; }

        private List<string> dictionary;
        private Random rand;
        public DictionarySubstitutionGenerator(List<string> entries, int maxSubstitutions)
        {
            MaxSubstitutions = maxSubstitutions;
            dictionary = entries;
            rand = new Random();
        }

        public IEnumerable<RequestSequence> Generate(List<KnownEndpoint> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults)
        {
            for (int i = 0; i < MaxSubstitutions; ++i)
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

                int subIndex = rand.Next(0, newSequence.Get(selectedStage).Substitutions.Count);
                ISubstitution sub = newSequence.Get(selectedStage).Substitutions[subIndex];
                newSequence.Get(selectedStage).Substitutions.RemoveAt(subIndex);

                string replacement = dictionary[rand.Next(0, dictionary.Count)];
                SubstituteConstant substituteConstant = new SubstituteConstant(sub.GetTarget(), replacement);
                newSequence.Get(selectedStage).Substitutions.Add(substituteConstant);

                yield return newSequence;
            }
            yield break;
        }
    }
}
