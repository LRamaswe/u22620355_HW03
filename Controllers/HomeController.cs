using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u22620355_HW03.Models;
using System.Data.Entity;
using Antlr.Runtime;
using System.Web.UI.WebControls;
using System.IO;

namespace u22620355_HW03.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Home()
        {
            return View();
        }

        //public ActionResult Maintain()
        //{
        //    return View();  
        //}

        //public ActionResult Report()
        //{
        //    return View();
        //}



        //public async Task<ActionResult> Index()
        //{
        //    var viewModel = new HomeVM
        //    {
        //        types = await db.types.ToListAsync(),
        //        authors = await db.authors.ToListAsync(),
        //        books = await db.books.ToListAsync(),
        //        students = await db.students.ToListAsync(),
        //        borrows = await db.borrows.ToListAsync(),
        //    };

        //    return View(viewModel);
        //}

        private LibraryEntities db = new LibraryEntities();
        public async Task<ActionResult> Index()
        {
            var books = await db.books.Include(b => b.borrows).ToListAsync();
            var students = await db.students.ToListAsync();

            // Determine the status for each book
            var bookList = books.Select(book => new
            {
                book,
                Status = book.borrows.Any(b => b.broughtDate == null) ? "Out" : "Available"
            }).ToList();

            var viewModel = new HomeVM
            {
                books = bookList.Select(b => new books
                {
                    bookId = b.book.bookId,
                    name = b.book.name,
                    pagecount = b.book.pagecount,
                    point = b.book.point,
                    authorId = b.book.authorId,
                    typeId = b.book.typeId,
                    Status = b.Status // Adding status to the model
                }).ToList(),
                students = students
            };

            return View(viewModel);
        }
        public async Task<ActionResult> Maintain()
        {
            var viewModel = new MaintainVM
            {
                types = await db.types.Take(10).ToListAsync(),
                authors = await db.authors.Take(10).ToListAsync(),
                borrows = await db.borrows.Include(b => b.books).Include(b => b.students).Take(10).ToListAsync() // Include related data
            };

            return View("Maintain", viewModel);
        }


        //Modal functions for the Maintain page
        //Type Modal Functions/Methods
        // Create Type
        [HttpPost]
        public async Task<ActionResult> CreateType(types type)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(type);
                await db.SaveChangesAsync();
                return RedirectToAction("Maintain");
            }
            return View("Maintain");
        }
        // Edit Type
        [HttpPost]
        public async Task<ActionResult> EditType(types type)
        {
            if (ModelState.IsValid)
            {
                var existingType = await db.types.FindAsync(type.typeId);
                if (existingType != null)
                {
                    existingType.name = type.name;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Maintain");
            }
            return View("Maintain");
        }
        // Delete Type
        [HttpPost]
        public async Task<ActionResult> DeleteType(int typeId)
        {
            var type = await db.types.FindAsync(typeId);
            if (type != null)
            {
                db.types.Remove(type);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Maintain");
        }



        //Author Modal Functions/Methods
        // Create Author
        [HttpPost]
        public async Task<ActionResult> CreateAuthor(authors author)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction("Maintain");
            }
            return View("Maintain");
        }
        // Edit Author
        [HttpPost]
        public async Task<ActionResult> EditAuthor(authors author)
        {
            if (ModelState.IsValid)
            {
                var existingAuthor = await db.authors.FindAsync(author.authorId);
                if (existingAuthor != null)
                {
                    existingAuthor.name = author.name;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Maintain");
            }
            return View("Maintain");
        }
        // Delete Author
        [HttpPost]
        public async Task<ActionResult> DeleteAuthor(int authorId)
        {
            var type = await db.types.FindAsync(authorId);
            if (type != null)
            {
                db.authors.Remove(await db.authors.FindAsync(authorId)); // Convert.ToInt32(   authorId);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Maintain");
        }



        //Borrow Modal Functions/Methods
        // Create Borrow
        [HttpPost]
        public async Task<ActionResult> CreateBorrow(borrows borrow)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrow);
                await db.SaveChangesAsync();
                return RedirectToAction("Maintain");
            }
            return View("Maintain");
        }
        //Edit Borrow
        public async Task<ActionResult> EditBorrow(int id)
        {
            var borrow = await db.borrows.FindAsync(id);
            if (borrow != null)
            {
                return View("EditBorrow", borrow);
            }
            return View("Maintain");
        }
        //Delete Borrow
        public async Task<ActionResult> DeleteBorrow(int id)
        {
            var borrow = await db.borrows.FindAsync(id);
            if (borrow != null)
            {
                db.borrows.Remove(borrow);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Maintain");
        }


        public ActionResult Report()
        {
            var viewModel = new ReportVM();

            // Retrieve report data if it exists in TempData
            if (TempData.ContainsKey("ReportData"))
            {
                viewModel = TempData["ReportData"] as ReportVM;
                TempData.Keep("ReportData"); // Keep data for later use
            }

            // Add any error message
            if (TempData.ContainsKey("ReportError"))
            {
                ViewBag.ErrorMessage = TempData["ReportError"].ToString();
            }

            return View(viewModel);
        }


        //[HttpPost]
        //public async Task<ActionResult> GenerateReport(int bookId)
        //{
        //    var reportData = new ReportVM
        //    {
        //        books = await db.books.Where(b => b.bookId == bookId).ToListAsync(),
        //        borrows = await db.borrows
        //            .Where(b => b.bookId == bookId)
        //            .Include(b => b.students)  // Include student data
        //            .ToListAsync(),
        //        students = await db.students.ToListAsync()
        //    };

        //    TempData["ReportData"] = reportData;

        //    ViewBag.ShowModal = true;
        //    return View("Report", reportData);
        //}


        [HttpPost]
        public async Task<ActionResult> GenerateReport(int BookId)
        {
            // Retrieve the required report data
            var book = await db.books.Include(b => b.borrows).FirstOrDefaultAsync(b => b.bookId == BookId);

            if (book == null)
            {
                TempData["ReportError"] = "Book not found. Please enter a valid Book ID.";
                return RedirectToAction("Report");
            }

            // Create a ReportVM instance to hold the report data
            var reportData = new ReportVM
            {
                books = new List<books> { book },
                borrows = book.borrows.ToList(),
                students = db.students.Where(s => book.borrows.Any(b => b.studentId == s.studentId)).ToList()
            };

            // Store report data in TempData
            TempData["ReportData"] = reportData;
            return RedirectToAction("Report");
        }

        [HttpPost]
        public ActionResult SaveReport(string fileName, string fileType)
        {
            if (!TempData.ContainsKey("ReportData"))
            {
                ModelState.AddModelError("", "No report data available to save.");
                return RedirectToAction("Report");
            }

            var reportData = TempData["ReportData"] as ReportVM;
            TempData.Keep("ReportData"); // Keep data after save

            // Define file path and name
            var filePath = Path.Combine(Server.MapPath("~/App_Data/Reports"), $"{fileName}.{fileType.ToLower()}");

            // Save as a text file (adjust for PDF/Excel as needed)
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Report for Book: {reportData.books[0].name}");
                writer.WriteLine("Student Borrowed\t\tTaken Date\t\tStatus");

                foreach (var borrow in reportData.borrows)
                {
                    var student = reportData.students.FirstOrDefault(s => s.studentId == borrow.studentId);
                    var status = borrow.broughtDate == null ? "Out" : "Available";
                    writer.WriteLine($"{student?.name} {student?.surname}\t\t{borrow.takenDate}\t\t{status}");
                }
            }

            TempData["SuccessMessage"] = $"Report saved successfully as {fileName}.{fileType.ToLower()}";
            return RedirectToAction("Report");
        }

    }
}