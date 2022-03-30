using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genealoge.Models
{
    // вывод ФИО участника в списках
    public class PeopleViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    // вывод короткой информации участника (фото и ФИО)
    public class PersonShortViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }
        public string PhotoPath { get; set; }
        public Gender Gender { get; set; }
    }

    // вывод полей для заполнения информации о участнике (фото, ФИО и родственники)
    public class CreatePersonViewModel
    {
        public int Id { get; set; }

        public virtual List<PersonShortViewModel> Children { get; set; }
        public virtual List<PersonShortViewModel> Marriage { get; set; }
        public virtual List<PersonShortViewModel> Siblings { get; set; }
        public virtual List<PersonShortViewModel> Parents { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }
        public string PhotoPath { get; set; }

        public Gender Gender { get; set; }

        public CreatePersonViewModel()
        {
            Children = new List<PersonShortViewModel>();
            Marriage = new List<PersonShortViewModel>();
            Siblings = new List<PersonShortViewModel>();
            Parents = new List<PersonShortViewModel>();
        }
    }

    // вывод полей для редактирования дополнительной информации
    public class EditInfoViewModel
    {
        public int Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Country { get; set; }
        public string CityOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Info { get; set; }
    }

    // вывод полной информации о участнике
    public class PersonFullViewModel
    {
        public int Id { get; set; }

        public virtual List<PersonShortViewModel> Children { get; set; }
        public virtual List<PersonShortViewModel> Marriage { get; set; }
        public virtual List<PersonShortViewModel> Siblings { get; set; }
        public virtual List<PersonShortViewModel> Fathers { get; set; }
        public virtual List<PersonShortViewModel> Mothers { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }
        public string PhotoPath { get; set; }

        public Gender Gender { get; set; }

        public PersonFullViewModel()
        {
            Children = new List<PersonShortViewModel>();
            Marriage = new List<PersonShortViewModel>();
            Siblings = new List<PersonShortViewModel>();
            Fathers = new List<PersonShortViewModel>();
            Mothers = new List<PersonShortViewModel>();
        }

        public PersonFullViewModel(int id, string fname, string lname, string tname, string path, Gender gender)
        {
            Id = id;
            FirstName = fname;
            LastName = lname;
            ThirdName = tname;
            PhotoPath = path;
            Gender = gender;

            Children = new List<PersonShortViewModel>();
            Marriage = new List<PersonShortViewModel>();
            Siblings = new List<PersonShortViewModel>();
            Fathers = new List<PersonShortViewModel>();
            Mothers = new List<PersonShortViewModel>();
        }
    }

    // вывод полей для добавления ребенка
    public class ChildShortViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }
        public string PhotoPath { get; set; }
        public Gender Gender { get; set; }

        public int? PotentialParentId { get; set; }
        public PersonShortViewModel PotentialParent { get; set; }
        public virtual List<PersonShortViewModel> PotentialParents { get; set; }

        public ChildShortViewModel()
        {
            PotentialParents = new List<PersonShortViewModel>();
        }
    }
}