using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Genealoge.Models;

namespace Genealoge.Controllers
{
    public class PeopleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: People
        public ActionResult Index()
        {
            List<PeopleViewModel> people = new List<PeopleViewModel>();

            foreach (var person in db.Persons.ToList())
            {
                people.Add(new PeopleViewModel {
                    Id = person.Id,
                    FullName = person.FirstName + " " + person.LastName + " " + person.ThirdName});
            }
            return View(people);
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            PersonFullViewModel viewPerson = new PersonFullViewModel(
                person.Id,
                person.FirstName,
                person.LastName,
                person.ThirdName,
                person.PhotoPath,
                person.Gender);

            viewPerson.Children = ToListPersonShortViewModel(person.Children);

            List<Person> Marriage;
            if (person.Wifes.Count > 0) Marriage = person.Wifes;
            else Marriage = person.Husbands;

            viewPerson.Marriage = ToListPersonShortViewModel(Marriage);

            List<Person> Siblings = new List<Person>();
            Siblings.AddRange(person.Siblings);
            Siblings.AddRange(person.ReversedSiblings);
            viewPerson.Siblings = ToListPersonShortViewModel(Siblings);

            foreach (var parent in person.Parents)
            {
                PersonShortViewModel addParent = new PersonShortViewModel();
                addParent.Id = parent.Id;
                addParent.FirstName = parent.FirstName;
                addParent.LastName = parent.LastName;
                addParent.ThirdName = parent.ThirdName;
                addParent.PhotoPath = parent.PhotoPath;
                addParent.Gender = parent.Gender;
                if (parent.Gender == Gender.Female) viewPerson.Mothers.Add(addParent);
                else viewPerson.Fathers.Add(addParent);

            }

            return View(viewPerson);
        }

        List<PersonShortViewModel> ToListPersonShortViewModel(List<Person> persons)
        {
            List<PersonShortViewModel> viewPersons = new List<PersonShortViewModel>();

            foreach (var person in persons)
            {
                PersonShortViewModel addPerson = new PersonShortViewModel();
                addPerson.Id = person.Id;
                addPerson.FirstName = person.FirstName;
                addPerson.LastName = person.LastName;
                addPerson.ThirdName = person.ThirdName;
                addPerson.Gender = person.Gender;
                viewPersons.Add(addPerson);
            }

            return viewPersons;
        }

        // GET: People/Create
        public ActionResult Create()
        {
            CreatePersonViewModel newPerson = new CreatePersonViewModel();
            return View(newPerson);
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePersonViewModel newPerson)
        {
            if (ModelState.IsValid)
            {

                Person person = new Person();
                person.FirstName = newPerson.FirstName;
                person.LastName = newPerson.LastName;
                person.ThirdName = newPerson.ThirdName;
                db.Persons.Add(person);
                db.SaveChanges();

                foreach (var child in newPerson.Children)
                {
                    Person addChild = new Person();
                    addChild.FirstName = child.FirstName;
                    addChild.LastName = child.LastName;
                    addChild.ThirdName = child.ThirdName;
                    addChild.Gender = child.Gender;
                    db.Persons.Add(addChild);
                    db.SaveChanges();
                    person.Children.Add(addChild);
                }

                foreach (var marriage in newPerson.Marriage)
                {
                    Person addMarriage = new Person();
                    addMarriage.FirstName = marriage.FirstName;
                    addMarriage.LastName = marriage.LastName;
                    addMarriage.ThirdName = marriage.ThirdName;
                    addMarriage.Gender = marriage.Gender;
                    db.Persons.Add(addMarriage);
                    db.SaveChanges();
                    if (addMarriage.Gender == Gender.Female) person.Wifes.Add(addMarriage);
                    else person.Husbands.Add(addMarriage);

                }

                foreach (var sibling in newPerson.Siblings)
                {
                    Person addSibling = new Person();
                    addSibling.FirstName = sibling.FirstName;
                    addSibling.LastName = sibling.LastName;
                    addSibling.ThirdName = sibling.ThirdName;
                    addSibling.Gender = sibling.Gender;
                    db.Persons.Add(addSibling);
                    db.SaveChanges();
                    person.Siblings.Add(addSibling);
                }

                foreach (var parent in newPerson.Parents)
                {
                    Person addParent = new Person();
                    addParent.FirstName = parent.FirstName;
                    addParent.LastName = parent.LastName;
                    addParent.ThirdName = parent.ThirdName;
                    addParent.Gender = parent.Gender;
                    db.Persons.Add(addParent);
                    db.SaveChanges();
                    person.Parents.Add(addParent);
                }

                WebImage photo = null;
                var newFileName = "";
                //var imagePath = "";

                photo = WebImage.GetImageFromRequest();
                if (photo != null)
                {
                    newFileName = Guid.NewGuid().ToString() + "_" +
                        Path.GetFileName(photo.FileName);
                    person.PhotoPath = @"~/Resourses/" + person.Id + "_" + newFileName;

                    photo.Save(person.PhotoPath);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(newPerson);
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,ThirdName,Gender,PhotoPath")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: People/AddChild/5
        public ActionResult AddChild(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChildShortViewModel newPerson = new ChildShortViewModel();
            List<PersonShortViewModel> potentials = new List<PersonShortViewModel>();
            Person parent = db.Persons.Find(id);
            List<Person> marriages = new List<Person>();
            if (parent.Gender == Gender.Male)
                marriages.AddRange(parent.Wifes);
            else
                marriages.AddRange(parent.Husbands);
            foreach (var person in marriages)
            {
                PersonShortViewModel potential = new PersonShortViewModel();
                potential.Id = person.Id;
                potential.FirstName = person.FirstName;
                potential.LastName = person.LastName;
                potential.ThirdName = person.ThirdName;
                potential.PhotoPath = person.PhotoPath;
                potential.Gender = person.Gender;
                potentials.Add(potential);
            }
            newPerson.PotentialParents.AddRange(potentials);
            SelectList potentialparents = new SelectList(potentials, "Id", "LastName");
            ViewBag.potentialparents = potentialparents;
            return View(newPerson);
        }

        // POST: People/AddChild/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddChild(int? id, ChildShortViewModel child)
        {
            int? Id = id;
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (ModelState.IsValid)
            {
                Person addChild = new Person();
                addChild.FirstName = child.FirstName;
                addChild.LastName = child.LastName;
                addChild.ThirdName = child.ThirdName;
                addChild.Gender = child.Gender;
                db.Persons.Add(addChild);
                db.SaveChanges();

                if (child.PotentialParentId != null)
                {
                    Person parent = db.Persons.Find(child.PotentialParentId);
                    addChild.Parents.Add(parent);
                    db.SaveChanges();
                }
                else
                {
                    Person parent = new Person();
                    parent.FirstName = child.PotentialParent.FirstName;
                    parent.LastName = child.PotentialParent.LastName;
                    parent.ThirdName = child.PotentialParent.ThirdName;
                    if (person.Gender == Gender.Male)
                        parent.Gender = Gender.Female;
                    else
                        parent.Gender = Gender.Male;
                    db.Persons.Add(parent);
                    db.SaveChanges();
                    addChild.Parents.Add(parent);
                    db.SaveChanges();
                }
                person.Children.Add(addChild);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Id });
            }
            return View(person);
        }

        // GET: People/AddMarriage/5
        public ActionResult AddMarriage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            PersonShortViewModel newPerson = new PersonShortViewModel();
            if (person.Gender == Gender.Male)
                newPerson.Gender = Gender.Female;
            else
                newPerson.Gender = Gender.Male;

            return View(newPerson);
        }

        // POST: People/AddMarriage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMarriage(int? id, PersonShortViewModel marriage)
        {
            int? Id = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (ModelState.IsValid)
            {
                Person addMarriage = new Person();
                addMarriage.FirstName = marriage.FirstName;
                addMarriage.LastName = marriage.LastName;
                addMarriage.ThirdName = marriage.ThirdName;
                addMarriage.Gender = marriage.Gender;
                db.Persons.Add(addMarriage);
                db.SaveChanges();

                if (person.Gender == Gender.Male)
                    person.Wifes.Add(addMarriage);
                else
                    person.Husbands.Add(addMarriage);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Id });
            }
            return View(person);
        }

        // GET: People/AddSibling/5
        public ActionResult AddSibling(int? id)
        {
            PersonShortViewModel newPerson = new PersonShortViewModel();
            return View(newPerson);
        }

        // POST: People/AddSibling/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSibling(int? id, PersonShortViewModel sibling)
        {
            int? Id = id;
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (ModelState.IsValid)
            {
                Person addSibling = new Person();
                addSibling.FirstName = sibling.FirstName;
                addSibling.LastName = sibling.LastName;
                addSibling.ThirdName = sibling.ThirdName;
                addSibling.Gender = sibling.Gender;
                db.Persons.Add(addSibling);
                db.SaveChanges();

                person.Siblings.Add(addSibling);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Id });
            }
            return View(person);
        }

        // GET: People/AddParent/5
        public ActionResult AddParent(int? id)
        {
            PersonShortViewModel newPerson = new PersonShortViewModel();
            return View(newPerson);
        }

        // POST: People/AddParent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParent(int? id, PersonShortViewModel parent)
        {
            int? Id = id;
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (ModelState.IsValid)
            {
                Person addParent = new Person();
                addParent.FirstName = parent.FirstName;
                addParent.LastName = parent.LastName;
                addParent.ThirdName = parent.ThirdName;
                addParent.Gender = parent.Gender;
                db.Persons.Add(addParent);
                db.SaveChanges();

                person.Parents.Add(addParent);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Id });
            }
            return View(person);
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.Persons.Find(id);
            db.Persons.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
