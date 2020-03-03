using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class DictionaryEntryEntity
    {
        public int? id { get; set; }
        public int dictionary_id { get; set; }
        public string content { get; set; }
    }
}
