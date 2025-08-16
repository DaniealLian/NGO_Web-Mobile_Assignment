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

    // GET: NGO_Event_/Event_Index
    public IActionResult Event_Index()
    {
        var model = db.Events.ToList(); // Added ToList() to ensure data is loaded
        return View(model);
    }

    // GET: NGO_Event_/CheckId - Fixed for AJAX validation
    public JsonResult CheckId(string Event_Id)
    {
        bool isAvailable = !db.Events.Any(e => e.EventID == Event_Id);
        return Json(isAvailable);
    }

    private string NextId()
    {
        try
        {
            string max = db.Events.Max(e => e.EventID) ?? "E000";
            int n = int.Parse(max[1..]);
            return $"E{(n + 1):000}"; // Fixed string formatting
        }
        catch
        {
            return "E001";
        }
    }

    // GET: NGO_Event_/Event_Insert
    public IActionResult Event_Insert()
    {
        var vm = new EventInsertVM
        {
            Event_Id = NextId(),
            Event_Title = "",
            Event_Date = DateTime.Today.AddDays(1),
            Event_Location = "",
            Event_Description = ""
        };

        return View(vm);
    }

    // POST: NGO_Event_/Event_Insert
    [HttpPost]
    public IActionResult Event_Insert(EventInsertVM vm)
    {
        // Check for duplicate ID
        if (!string.IsNullOrEmpty(vm.Event_Id) && db.Events.Any(e => e.EventID == vm.Event_Id))
        {
            ModelState.AddModelError("Event_Id", "Duplicated Event ID.");
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
            try
            {
                string? photoUrl = null;
                if (vm.Event_Photo != null)
                {
                    photoUrl = hp.SavePhoto(vm.Event_Photo, "events");
                }

                // FIXED: Only save photo once and don't duplicate
                db.Events.Add(new Event
                {
                    EventID = vm.Event_Id,
                    EventTitle = vm.Event_Title,
                    EventDate = vm.Event_Date,
                    EventLocation = vm.Event_Location,
                    EventDescription = vm.Event_Description,
                    EventPhotoURL = photoUrl // FIXED: Use the photoUrl variable, not save again
                });

                db.SaveChanges();

                TempData["Info"] = "Event inserted successfully.";
                return RedirectToAction("Event_Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving event: {ex.Message}");
            }
        }

        // If we get here, something failed
        return View(vm);
    }

    // GET: NGO_Event_/Event_Update - Fixed typo in method name
    public IActionResult Event_Update(string? id)
    {
        var e = db.Events.Find(id);

        if (e == null)
        {
            TempData["Info"] = "Event not found when getting.";
            return RedirectToAction("Event_Index");
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

    // POST: NGO_Event_/Event_Update
    [HttpPost]
    public IActionResult Event_Update(EventUpdateVM vm)
    {
        var e = db.Events.Find(vm.Event_Id);

        if (e == null)
        {
            TempData["Info"] = "Event not found when posting.";
            return RedirectToAction("Event_Index");
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
            try
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
                return RedirectToAction("Event_Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");
            }
        }

        // Reload the photo URL for display
        vm.Event_PhotoURL = e.EventPhotoURL;
        return View(vm);
    }

    // POST: NGO_Event_/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var e = db.Events.Find(id);

        if (e != null)
        {
            try
            {
                // Delete associated photo
                if (!string.IsNullOrEmpty(e.EventPhotoURL))
                {
                    hp.DeletePhoto(e.EventPhotoURL, "events");
                }

                db.Events.Remove(e);
                db.SaveChanges();

                TempData["Info"] = "Event deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Info"] = $"Error deleting event: {ex.Message}";
            }
        }
        else
        {
            TempData["Info"] = "Event not found.";
        }

        return RedirectToAction("Event_Index");
    }
}