using BigSchool1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool1.Controllers
{
    public class CoursesController : Controller
    {
        BigSchoolContext con = new BigSchoolContext();
        // GET: Courses
        public ActionResult Create()
        {
            //get list category
            Course objCourse = new Course();
            objCourse.ListCategory = con.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objcourse)
        {
            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objcourse.ListCategory = con.Categories.ToList();
                return View("Create", objcourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objcourse.LectureId = user.Id;

            con.Courses.Add(objcourse);
            con.SaveChanges();

            return RedirectToAction("Index", "Home");

        }
        public ActionResult Attending()
        {
            BigSchoolContext con = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var ListAttendances = con.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in ListAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LectureId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            BigSchoolContext con = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = con.Courses.Where(c => c.LectureId == currentUser.Id && c.Datetime > DateTime.Now).ToList();

            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }
        public ActionResult Edit(int? id)
        {
            Course course = con.Courses.Find(id);
            course.ListCategory = con.Categories.ToList();
            if (id == null)
            {
                return HttpNotFound();
            }
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course objcourse)
        {
            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objcourse.ListCategory = con.Categories.ToList();
                return View("Edit", objcourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objcourse.LectureId = user.Id;

            con.Courses.AddOrUpdate(objcourse);
            con.SaveChanges();

            return RedirectToAction("Mine", "Courses");
        }
        public ActionResult Delete(int? id)
        {
            Course course = con.Courses.Find(id);
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course course = con.Courses.Find(id);
            Attendance attendance = con.Attendances.Find(id, currentUser.Id);
            con.Attendances.Remove(attendance);
            con.SaveChanges();
            con.Courses.Remove(course);
            con.SaveChanges();
            return RedirectToAction("Mine", "Courses");
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var listFollwee = context.Followings.Where(p => p.FollowerId ==

            currentUser.Id).ToList();

            var listAttendances = context.Attendances.Where(p => p.Attendee ==

            currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)

            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }

}