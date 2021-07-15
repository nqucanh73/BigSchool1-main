using BigSchool1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool1.Controllers
{
    public class AttendancesController : ApiController
    { 
        [HttpPost]
        public IHttpActionResult Attend(Course attendanceDto)
        {
            BigSchoolContext con = new BigSchoolContext();
            var userID = User.Identity.GetUserId();

            if (con.Attendances.Any(p => p.Attendee == userID && p.CourseId ==
            attendanceDto.Id))
            {
                con.Attendances.Remove(con.Attendances.SingleOrDefault(p =>
                p.Attendee == userID && p.CourseId == attendanceDto.Id));
                con.SaveChanges();
                return Ok("cancel");
            }
            var attendance = new Attendance()
            {
                CourseId = attendanceDto.Id,
                Attendee =
                User.Identity.GetUserId()
            };
            con.Attendances.Add(attendance);
            con.SaveChanges();
            return Ok();
        }
    }
}
