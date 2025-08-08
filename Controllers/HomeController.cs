using Microsoft.AspNetCore.Mvc;
using NGO_Web_Demo;

namespace Demo.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment en;
    private readonly Helper hp;

    public HomeController(IWebHostEnvironment en, Helper hp)
    {
        this.en = en;
        this.hp = hp;
    }

    // GET: Home/Index
    public IActionResult Index()
    {
        return View();
    }

    // POST: Home/Index
    [HttpPost]
    public IActionResult Index(IFormFile photo) // TODO
    {
        // TODO

        if (ModelState.IsValid("photo"))
        {
            // TODO
            //var path = Path.Combine(en.WebRootPath, "uploads", photo.FileName);
            //using var stream = System.IO.File.Create(path);
            //photo.CopyTo(stream);

            var e = hp.ValidatePhoto(photo);
            if (e != "") ModelState.AddModelError("photo", e);
        }

        if (ModelState.IsValid)
        {
            hp.SavePhoto(photo, "uploads");

            TempData["Info"] = "Photo uploaded.";
            return RedirectToAction();
        }

        return View();
    }

    // GET: Home/Browse
    public IActionResult Browse()
    {
        // TODO
        var path = Path.Combine(en.WebRootPath, "uploads");
        var files = Directory.GetFiles(path, "*.jpg").Select(p => Path.GetFileName(p));

        return View(files);
    }

    // POST: Home/Delete
    [HttpPost]
    public IActionResult Delete(string file)
    {
        // TODO
        hp.DeletePhoto(file, "uploads");

        TempData["Info"] = "Photo deleted.";
        return RedirectToAction("Browse");
    }

    // POST: Home/DeleteAll
    [HttpPost]
    public IActionResult DeleteAll()
    {
        var path = Path.Combine(en.WebRootPath, "uploads");
        var files = Directory.GetFiles(path, "*.jpg");

        foreach (var file in files)
        {
            System.IO.File.Delete(file);
        }

        TempData["Info"] = "All photos deleted.";
        return RedirectToAction("Browse");
    }
}
