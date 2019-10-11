using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebView.Data;
using WebView.Models;

namespace WebView.Controllers
{
    public class RequestController : Controller
    {
        private readonly KnownEndpointContext _context;

        public RequestController(KnownEndpointContext context)
        {
            _context = context;
        }

        // GET: Request
        public async Task<IActionResult> Index()
        {
            return View(await _context.KnownRequests.ToListAsync());
        }

        // GET: Request/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestModel = await _context.KnownRequests
                .FirstOrDefaultAsync(m => m.RequestModelID == id);
            if (requestModel == null)
            {
                return NotFound();
            }

            return View(requestModel);
        }

        // GET: Request/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestModelID,Host,RequestText")] RequestModel requestModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestModel);
        }

        // GET: Request/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestModel = await _context.KnownRequests.FindAsync(id);
            if (requestModel == null)
            {
                return NotFound();
            }
            return View(requestModel);
        }

        // POST: Request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestModelID,Host,RequestText")] RequestModel requestModel)
        {
            if (id != requestModel.RequestModelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestModelExists(requestModel.RequestModelID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(requestModel);
        }

        // GET: Request/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestModel = await _context.KnownRequests
                .FirstOrDefaultAsync(m => m.RequestModelID == id);
            if (requestModel == null)
            {
                return NotFound();
            }

            return View(requestModel);
        }

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestModel = await _context.KnownRequests.FindAsync(id);
            _context.KnownRequests.Remove(requestModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestModelExists(int id)
        {
            return _context.KnownRequests.Any(e => e.RequestModelID == id);
        }
    }
}
