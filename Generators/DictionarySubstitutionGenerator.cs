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

        private string[] dictionary;
        private Random rand;
        public DictionarySubstitutionGenerator(string dictionaryPath, int maxSubstitutions)
        {
            MaxSubstitutions = maxSubstitutions;
            dictionary = System.IO.File.ReadAllLines(dictionaryPath);
            rand = new Random();
        }

        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults)
        {
            for (int i = 0; i < MaxSubstitutions; ++i)
            {
                if (sequence.Count() == 0)
                {
                    continue;
                }

                RequestSequence newSequence = sequence.Copy();
                int selectedStage = rand.Next(0, newSequence.Count());

                Stage stage = newSequence.Get(selectedStage);

                if (stage.Substitutions.Count == 0)
                {
                    continue;
                }

                int subIndex = rand.Next(0, newSequence.Get(selectedStage).Substitutions.Count);
                ISubstitution sub = newSequence.Get(selectedStage).Substitutions[subIndex];
                newSequence.Get(selectedStage).Substitutions.RemoveAt(subIndex);

                string replacement = dictionary[rand.Next(0, dictionary.Length)];
                SubstituteConstant substituteConstant = new SubstituteConstant(sub.GetTarget(), replacement);
                newSequence.Get(selectedStage).Substitutions.Add(substituteConstant);

                yield return newSequence;
            }
            yield break;
        }
    }
}
