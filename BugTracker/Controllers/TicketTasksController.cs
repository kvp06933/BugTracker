using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class TicketTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketTasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketTasks.Include(t => t.TaskStatus).Include(t => t.TaskType).Include(t => t.Ticket);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketTask = await _context.TicketTasks
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskType)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketTask == null)
            {
                return NotFound();
            }

            return View(ticketTask);
        }

        // GET: TicketTasks/Create
        public IActionResult Create()
        {
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id");
            ViewData["TaskTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id");
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id");
            return View();
        }

        // POST: TicketTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,TicketId,TicketStatusId,TaskTypeId")] TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticketTask.TicketStatusId);
            ViewData["TaskTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticketTask.TaskTypeId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketTask.TicketId);
            return View(ticketTask);
        }

        // GET: TicketTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketTask = await _context.TicketTasks.FindAsync(id);
            if (ticketTask == null)
            {
                return NotFound();
            }
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticketTask.TicketStatusId);
            ViewData["TaskTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticketTask.TaskTypeId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketTask.TicketId);
            return View(ticketTask);
        }

        // POST: TicketTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,TicketId,TicketStatusId,TaskTypeId")] TicketTask ticketTask)
        {
            if (id != ticketTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketTaskExists(ticketTask.Id))
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
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticketTask.TicketStatusId);
            ViewData["TaskTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticketTask.TaskTypeId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketTask.TicketId);
            return View(ticketTask);
        }

        // GET: TicketTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketTask = await _context.TicketTasks
                .Include(t => t.TaskStatus)
                .Include(t => t.TaskType)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketTask == null)
            {
                return NotFound();
            }

            return View(ticketTask);
        }

        // POST: TicketTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketTask = await _context.TicketTasks.FindAsync(id);
            _context.TicketTasks.Remove(ticketTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketTaskExists(int id)
        {
            return _context.TicketTasks.Any(e => e.Id == id);
        }
    }
}
