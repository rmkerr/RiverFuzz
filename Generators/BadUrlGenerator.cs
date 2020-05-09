using HttpTokenize;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generators
{
    class BadUrlGenerator : IGenerator
    {
        private int _maxSubstitutions;
        private Random _rand;
        public BadUrlGenerator(int maxSubstitutions)
        {
            _maxSubstitutions = maxSubstitutions;
            _rand = new Random();
        }
        public IEnumerable<RequestSequence> Generate(List<KnownEndpoint> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults)
        {
            for (int i = 0; i < _maxSubstitutions; ++i)
            {
                if (sequence.StageCount() == 0)
                {
                    continue;
                }

                RequestSequence newSequence = sequence.Copy();
                int selectedStage = _rand.Next(0, newSequence.StageCount());

                Stage stage = newSequence.Get(selectedStage);

                if (stage.Substitutions.Count == 0)
                {
                    continue;
                }



                int subIndex = _rand.Next(0, newSequence.Get(selectedStage).Substitutions.Count);
                ISubstitution sub = newSequence.Get(selectedStage).Substitutions[subIndex];
                newSequence.Get(selectedStage).Substitutions.RemoveAt(subIndex);

                string replacement = createBadUrl();
                SubstituteConstant substituteConstant = new SubstituteConstant(sub.GetTarget(), replacement);
                newSequence.Get(selectedStage).Substitutions.Add(substituteConstant);

                yield return newSequence;
            }
            yield break;
        }

        private string createBadUrl()
        {
            // I can do way better than this, but this should be fine for a POC.
            string[] schemes = { "http://", "blah://", "://", "http://://", "" };
            string[] hosts = { "example.com", "/", "//", "\\", "\\\\", "*", ".", "..", "\u200B", "test\uFF0F", "", "test:test@test.com" };
            string[] paths = { "/test/path", "/", "", "/../", "/../../" };

            string result = schemes[_rand.Next(0, schemes.Length)] + hosts[_rand.Next(0, hosts.Length)] + paths[_rand.Next(0, paths.Length)];

            // Consider URL encoding.
            if (_rand.Next(0, 2) == 0)
            {
                return result;
            }
            return Uri.EscapeUriString(result);
        }
    }
}
