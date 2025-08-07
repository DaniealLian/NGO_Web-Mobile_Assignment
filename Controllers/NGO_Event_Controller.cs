using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NGO_Web_Demo;
using NGO_Web_Demo.Models;

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

    // GET: Product/Index
    public IActionResult Index()
    {
        var model = db.Events;
        return View(model);
    }

    // GET: Product/CheckId
    public bool CheckId(string Event_Id)
    {
        return !db.Events.Any(e => e.EventID == Event_Id);
    }

    private string NextId()
    {
        string max = db.Events.Max(e => e.EventID) ?? "E000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'E'000");
    }

    // GET: Product/Insert
    public IActionResult Insert()
    {
        var vm = new EventInsertVM
        {
            Event_Id = NextId(),
            Event_Title = "To be completed",
        };

        return View(vm);
    }

    // POST: Product/Insert
    [HttpPost]
    public IActionResult Insert(EventInsertVM vm)
    {
        if (ModelState.IsValid("Id") && db.Events.Any(e => e.EventID == vm.Event_Id))
        {
            ModelState.AddModelError("Id", "Duplicated Id.");
        }

        if (ModelState.IsValid("Photo"))
        {

            var e = hp.ValidatePhoto(vm.Event_Photo);
            if (e != "") ModelState.AddModelError("Photo", e);

        }

        if (ModelState.IsValid)
        {

            db.Events.Add(new()
            {
                EventID = vm.Event_Id,
                EventTitle = vm.Event_Title,
                EventDate = vm.Event_Date,
                EventLocation = vm.Event_Location,
                EventPhotoURL = hp.SavePhoto(vm.Event_Photo, "events"),
            });
            db.SaveChanges();

            TempData["Info"] = "Product inserted.";
            return RedirectToAction("Index");
        }

        return View();
    }

    // GET: Product/Update
    public IActionResult Update(string? id)
    {
        var e = db.Events.Find(id);

        if (e == null)
        {
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
            Event_Photo = null, // Initialize to null for the form
        };

        return View(vm);
    }

    // POST: Product/Update
    [HttpPost]
    public IActionResult Update(EventUpdateVM vm)
    {
        var e = db.Events.Find(vm.Event_Id);

        if (e == null)
        {
            return RedirectToAction("Index");
        }
        if (vm.Event_Photo != null)
        {
            var p = hp.ValidatePhoto(vm.Event_Photo);
            if (p != "") ModelState.AddModelError("Photo", p);

        }

        if (ModelState.IsValid)
        {

            e.EventID = vm.Event_Id;
            e.EventTitle = vm.Event_Title;

            if (vm.Event_Photo != null)
            {
                hp.DeletePhoto(e.EventPhotoURL, "events");
                e.EventPhotoURL = hp.SavePhoto(vm.Event_Photo, "events");
            }
            db.SaveChanges();

            TempData["Info"] = "Product updated.";
            return RedirectToAction("Index");
        }

        vm.Event_PhotoURL = e.EventPhotoURL;
        return View(vm);
    }

    // POST: Product/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var e = db.Events.Find(id);

        if (e != null)
        {
            // TODO
            hp.DeletePhoto(e.EventPhotoURL, "products");
            db.Events.Remove(e);
            db.SaveChanges();

            TempData["Info"] = "Product deleted.";
        }

        return RedirectToAction("Index");
    }
}
