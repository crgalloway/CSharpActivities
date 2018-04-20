using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Activities.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Activities
{
    public class ActivityController : Controller
    {
        private ActivitiesContext _context;
        public ActivityController(ActivitiesContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("Clean")]
        public IActionResult Clean()
        {
            List<Event> allEvents = _context.events.ToList();
            DateTime now = DateTime.Now;
            foreach (Event eachEvent in allEvents)
            {
                DateTime checkedTime = eachEvent.StartTime;
                TimeSpan addedMinutes = new TimeSpan(0, eachEvent.DurationInMinutes, 0);
                DateTime endTime = checkedTime.Add(addedMinutes);
                int check = DateTime.Compare(endTime,now);
                if(check<0)
                {
                    List<Attending> attendingUsers = _context.attending.Where( g => g.AttendingEventId == eachEvent.EventId ).ToList();;
                    foreach(var eachUser in attendingUsers)
                    {
                        _context.attending.Remove(eachUser);
                    }
                    _context.events.Remove(eachEvent);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Home");
        }
        [HttpGet]
        [Route("Home")]
        public IActionResult Home()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Confirm that user is logged in
            {
                ViewBag.allEvents = _context.events.Include( e => e.Attendees ).Include( e => e.CreatedBy ).OrderBy( e => e.StartTime).ToList(); //Get all events
                ViewBag.activeUser = _context.users.Single( u => u.UserId == (int)activeId); //Get active User
                return View();
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Confirm that user is logged in
            {
                return View();
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [Route("New/Event")]
        public IActionResult NewEvent(EventViewModel model)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Confirm that user is logged in
            {
                if(ModelState.IsValid)
                {
                    DateTime startDate = (DateTime)model.StartDate;
                    DateTime startTime = (DateTime)model.StartTime;
                    DateTime combined = startDate.Date + startTime.TimeOfDay;
                    int durationInMinutes;
                    if(model.DurationUnits == "Days")
                    {
                        durationInMinutes = model.Duration*1440;
                    }
                    else if (model.DurationUnits == "Hours")
                    {
                        durationInMinutes = model.Duration*60;
                    }
                    else{
                        durationInMinutes = model.Duration;
                    }
                    Event newEvent = new Event{
                        Title = model.Title,
                        Description = model.Description,
                        DurationInMinutes = durationInMinutes,
                        StartTime = combined,
                        CreatedByUserId = (int)activeId
                    };
                    _context.Add(newEvent);
                    _context.SaveChanges();
                    int newId = _context.events.OrderBy( e => e.CreatedAt ).Last().EventId;
                    return RedirectToAction("ViewEvent", new{eventId = newId});
                }
                return View("New");
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        [Route("add/User/{eventId}")]
        public IActionResult AddGuest(int eventId)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                var attending = false;
                List<Event> joinedEvent = _context.events.Where( e => e.EventId == eventId ).Include( e => e.Attendees ).ToList();
                if( joinedEvent.Count == 1 )
                {
                    foreach (var user in joinedEvent[0].Attendees)
                    {
                        if(user.AttendingUserId == (int)activeId)
                        {
                            attending=true;
                            break;
                        }
                    }
                    if(!attending)
                    {
                        DateTime JoiningStartTime = joinedEvent[0].StartTime;
                        TimeSpan addedMinutes = new TimeSpan(0, joinedEvent[0].DurationInMinutes, 0);
                        DateTime JoiningEndTime = joinedEvent[0].StartTime.Add(addedMinutes);
                        var user = _context.users.Include( u => u.Attending ).ThenInclude( a => a.Event ).Single( u => u.UserId == activeId);
                        bool CanAttend = true;
                        foreach(var each in user.Attending)
                        {
                            TimeSpan EachAddedMinutes = new TimeSpan(0, each.Event.DurationInMinutes, 0);
                            DateTime EachEndTime = each.Event.StartTime.Add(addedMinutes);
                            int compare1 = DateTime.Compare(JoiningStartTime, EachEndTime); //Will be neg if new event starts before other ends
                            int compare2 = DateTime.Compare(each.Event.StartTime, JoiningEndTime); //will be neg if new event ends after other starts
                            if(compare1 <0 && compare2 <0)
                            {
                                CanAttend = false;
                                break;
                            }
                        }
                        if(CanAttend)
                        {
                            Attending newAttendee = new Attending{
                                AttendingEventId = eventId,
                                AttendingUserId = (int)activeId
                            };
                            _context.Add(newAttendee);
                            _context.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Home");
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        [Route("remove/User/{eventId}")]
        public IActionResult RemoveGuest(int eventId)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                Attending canceledAttendee = _context.attending.SingleOrDefault( g => g.AttendingEventId == eventId && g.AttendingUserId == (int)activeId);
                _context.attending.Remove(canceledAttendee);
                _context.SaveChanges();
                return RedirectToAction("Home");
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        [Route("remove/Event/{eventId}")]
        public IActionResult DeleteEvent(int eventId) //Remove wedding and associated guest RSVP's
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                List<Event> canceledEvent = _context.events.Where( e => e.EventId == eventId).ToList();
                if (canceledEvent.Count == 1  && canceledEvent[0].CreatedByUserId == (int)activeId)
                {
                    var attendingUsers = _context.attending.Where( g => g.AttendingEventId == eventId );
                    foreach(var each in attendingUsers)
                    {
                        _context.attending.Remove(each);
                    }
                    _context.events.Remove(canceledEvent[0]);
                    _context.SaveChanges();
                }
                return RedirectToAction("Home");
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        [Route("Event/{eventId}")]
        public IActionResult ViewEvent(int eventId)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                List<Event> viewableEvent = _context.events.Where( e => e.EventId == eventId).Include( e => e.CreatedBy ).Include( e => e.Attendees ).ThenInclude( a => a.User ).ToList();
                if(viewableEvent.Count == 1)
                {
                    ViewBag.viewEvent = viewableEvent[0];
                    ViewBag.activeId = (int)activeId;
                    return View();
                }
                return RedirectToAction("Home");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}