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
    public class EndpointController : Controller
    {
        private readonly KnownEndpointContext _context;

        public EndpointController(KnownEndpointContext context)
        {
            _context = context;
        }

        // GET: Endpoint
        public async Task<IActionResult> Index()
        {
            return View(await _context.KnownEndpoints.ToListAsync());
        }

        // GET: Endpoint/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endpointModel = await _context.KnownEndpoints
                .FirstOrDefaultAsync(m => m.EndpointModelID == id);
            if (endpointModel == null)
            {
                return NotFound();
            }

            return View(endpointModel);
        }

        // GET: Endpoint/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Endpoint/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EndpointModelID,Name")] EndpointModel endpointModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(endpointModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(endpointModel);
        }

        // GET: Endpoint/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endpointModel = await _context.KnownEndpoints.FindAsync(id);
            if (endpointModel == null)
            {
                return NotFound();
            }
            return View(endpointModel);
        }

        // POST: Endpoint/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EndpointModelID,Name")] EndpointModel endpointModel)
        {
            if (id != endpointModel.EndpointModelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(endpointModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EndpointModelExists(endpointModel.EndpointModelID))
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
            return View(endpointModel);
        }

        // GET: Endpoint/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endpointModel = await _context.KnownEndpoints
                .FirstOrDefaultAsync(m => m.EndpointModelID == id);
            if (endpointModel == null)
            {
                return NotFound();
            }

            return View(endpointModel);
        }

        // POST: Endpoint/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var endpointModel = await _context.KnownEndpoints.FindAsync(id);
            _context.KnownEndpoints.Remove(endpointModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EndpointModelExists(int id)
        {
            return _context.KnownEndpoints.Any(e => e.EndpointModelID == id);
        }
    }
}
