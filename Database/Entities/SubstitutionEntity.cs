using HttpTokenize.Substitutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class SubstitutionEntity
    {
        public int? id { get; set; }
        public string type { get; set; }
        public string summary { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        public static SubstitutionEntity FromSubstitution(ISubstitution sub)
        {
            SubstitutionEntity model = new SubstitutionEntity();
            model.type = sub.GetType().Name;
            model.summary = sub.ToString();

            return model;
        }
    }
}
