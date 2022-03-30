using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genealoge.Models
{
    public enum Gender
    {
        Male, Female, Undefined
    }

    public class Person
    {
        public int Id { get; set; }

        public virtual List<Person> Children { get; set; }
        public virtual List<Person> Parents { get; set; }

        public virtual List<Person> Wifes { get; set; }
        public virtual List<Person> Husbands { get; set; }

        public virtual List<Person> Siblings { get; set; }
        public virtual List<Person> ReversedSiblings { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }

        public Gender Gender { get; set; }

        public DateTime? DateOfBirth {get; set; }
        public string Country { get; set; }
        public string CityOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Info { get; set; }
        public string PhotoPath { get; set; }

        public Person()
        {
            Children = new List<Person>();
            Parents = new List<Person>();

            Wifes = new List<Person>();
            Husbands = new List<Person>();

            Siblings = new List<Person>();
            ReversedSiblings = new List<Person>();
        }
    }
}