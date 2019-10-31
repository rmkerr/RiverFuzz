using HttpTokenize.Substitutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModels.Models
{
    public class SubstitutionModel
    {
        public int? id { get; set; }
        public string type { get; set; }
        public string summary { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        public static SubstitutionModel FromSubstitution(ISubstitution sub)
        {
            SubstitutionModel model = new SubstitutionModel();
            model.type = sub.GetType().Name;
            model.summary = model.ToString();

            return model;
        }
    }
}
