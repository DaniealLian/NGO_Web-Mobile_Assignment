using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NGO_Web_Demo;
using NGO_Web_Demo.Models;
using Demo.Models; // Added for Event model

namespace NGO_Web_Demo.Controllers;

public class NGO_Event_Controller : Controller
{
    private readonly DB db;
    private readonly Helper hp;

    public NGO_Event_Controller(DB db, Helper hp)
    {
        this.db = db;
        this.hp = hp;
    }

    // GET: NGO_Event/Index
    public IActionResult Event_Index()
    {
        var model = db.Events; // Added ToList() to ensure data is loaded
        return View(model);
    }

    // GET: NGO_Event/CheckId
    public bool CheckId(string Event_Id)
    {
        return !db.Events.Any(e => e.EventID == Event_Id);
    }

    private string NextId()
    {
        //var maxEvent = db.Events.OrderByDescending(e => e.EventID).FirstOrDefault();
        //if (maxEvent == null)
        //{
        //    return "E001";
        //}

        string max = db.Events.Max(e => e.EventID) ?? "E000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'E'000");
    }

    // GET: NGO_Event/Insert
    public IActionResult Event_Insert()
    {
        var vm = new EventInsertVM
        {
            Event_Id = NextId(),
            Event_Title = "",
            Event_Date = DateTime.Today.AddDays(1), // Default to tomorrow
            Event_Location = "",
            Event_Description = ""

        };

        return View(vm);
    }

    // POST: NGO_Event/Insert
    [HttpPost]
    public IActionResult Event_Insert(EventInsertVM vm)
    {
        // Check for duplicate ID
        if (ModelState.IsValid("Event_Id") && db.Events.Any(e => e.EventID == vm.Event_Id))
        {
            ModelState.AddModelError("Event_Id", "Duplicated Event ID.");
        }

        // Validate photo if provided
        if (vm.Event_Photo != null && ModelState.IsValid("Event_Photo"))
        {
            var error = hp.ValidatePhoto(vm.Event_Photo);
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("Event_Photo", error);
            }
        }

        if (ModelState.IsValid)
        {
            string? photoUrl = null;
            if (vm.Event_Photo != null)
            {
                photoUrl = hp.SavePhoto(vm.Event_Photo, "events");
            }

            db.Events.Add(new()
            {
                EventID = vm.Event_Id,
                EventTitle = vm.Event_Title,
                EventDate = vm.Event_Date,
                EventLocation = vm.Event_Location,
                EventDescription = vm.Event_Description,
                EventPhotoURL = hp.SavePhoto(vm.Event_Photo, "events"),
            });

            db.SaveChanges();

            TempData["Info"] = "Event inserted successfully.";
            return RedirectToAction("Event_Index");
        }
        else if (ModelState.IsValid!)
        {
            TempData["Info"] = "Event inserted unsuccessfully.";
            return RedirectToAction("Event_Index");
        }
        return View(vm);
    }

    // GET: NGO_Event/Update
    public IActionResult Even_Update(string? id)
    {
        var e = db.Events.Find(id);

        if (e == null)
        {
            TempData["Info"] = "Event not found.";
            return RedirectToAction("Index");
        }

        var vm = new EventUpdateVM
        {
            Event_Id = e.EventID,
            Event_Title = e.EventTitle,
            Event_Date = e.EventDate,
            Event_Location = e.EventLocation,
            Event_Description = e.EventDescription,
            Event_PhotoURL = e.EventPhotoURL,
            Event_Photo = null,
        };

        return View(vm);
    }

    // POST: NGO_Event/Update
    [HttpPost]
    public IActionResult Event_Update(EventUpdateVM vm)
    {
        var e = db.Events.Find(vm.Event_Id);

        if (e == null)
        {
            TempData["Info"] = "Event not found.";
            return RedirectToAction("Index");
        }

        // Validate photo if provided
        if (vm.Event_Photo != null)
        {
            var error = hp.ValidatePhoto(vm.Event_Photo);
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("Event_Photo", error);
            }
        }

        if (ModelState.IsValid)
        {
            e.EventTitle = vm.Event_Title;
            e.EventDate = vm.Event_Date;
            e.EventLocation = vm.Event_Location;
            e.EventDescription = vm.Event_Description;

            // Handle photo update
            if (vm.Event_Photo != null)
            {
                // Delete old photo if exists
                if (!string.IsNullOrEmpty(e.EventPhotoURL))
                {
                    hp.DeletePhoto(e.EventPhotoURL, "events");
                }
                e.EventPhotoURL = hp.SavePhoto(vm.Event_Photo, "events");
            }

            db.SaveChanges();

            TempData["Info"] = "Event updated successfully.";
            return RedirectToAction("Index");
        }

        // Reload the photo URL for display
        vm.Event_PhotoURL = e.EventPhotoURL;
        return View(vm);
    }

    // POST: NGO_Event/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var e = db.Events.Find(id);

        if (e != null)
        {
            // Delete associated photo
            if (!string.IsNullOrEmpty(e.EventPhotoURL))
            {
                hp.DeletePhoto(e.EventPhotoURL, "events"); // Fixed folder name
            }

            db.Events.Remove(e);
            db.SaveChanges();

            TempData["Info"] = "Event deleted successfully.";
        }
        else
        {
            TempData["Info"] = "Event not found.";
        }

        return RedirectToAction("Index");
    }
}