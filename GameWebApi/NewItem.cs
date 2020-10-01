using System;
using System.ComponentModel.DataAnnotations;

namespace GameWebApi
{
    public class NewItem
    {
        [Range(1, 99)]
        public int Level { get; set; }

        [EnumDataType(typeof(Type))]
        public Type ItemType { get; set; }

        [ValidateCreationDateAttribute]
        public DateTime CreationDate { get; set; }
    }
}