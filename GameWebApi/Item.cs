using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GameWebApi
{
    public class ValidateCreationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            NewItem newItem = (NewItem)validationContext.ObjectInstance;
            Console.WriteLine("Validating new item CreationDate: " + newItem.CreationDate);
            if (newItem.CreationDate > DateTime.UtcNow)
            {
                Console.WriteLine("CreationTime must be in the past");
                return new ValidationResult("CreationTime must be in the past");
            }
            Console.WriteLine("New item creation date validated");
            return ValidationResult.Success;
        }
    }

    public enum Type 
    {
        SWORD, POTION, SHIELD
    }
    
    public class Item
    {
        public Item()
        {
            
        }
        public Item(NewItem i)
        {
            Id = Guid.NewGuid();
            Level = i.Level;
            ItemType = i.ItemType;
            CreationDate = DateTime.UtcNow;
        }
        public Guid Id { get; set; }

        [Range(1,99)]
        public int Level { get; set; }

        [EnumDataType(typeof(Type))]
        public Type ItemType { get; set; }

        [ValidateCreationDateAttribute]
        public DateTime CreationDate;
    }
}