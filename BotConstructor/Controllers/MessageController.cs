﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotConstructor.Database.Models;
using BotConstructor.Web.Models.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BotConstructor.Web.Controllers
{
    public class MessageController : Controller
    {
        private ApplicationContext _context; 

        public MessageController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            return View(_context.Messages.Include(x => x.Bot).ToList());
        }

        public async Task<IActionResult> Edit(int id = 0)
        {
            var model = new MessageViewModel();
            if(id > 0)
            {
                var msg = await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);
                model.Id = msg.Id;
                model.InputMessage = msg.InputMessage;
                model.OutputMessage = msg.OutputMessage;
                model.BotId = msg.BotId; 
            }
            model.Bots = _context.Bots.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();

            return View(model); 
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Message msg;

                if (model.Id > 0) 
                    msg = await _context.Messages.FirstAsync(x => x.Id == model.Id);
                else
                {
                    msg = new Message();
                    _context.Messages.Add(msg); 
                }

                msg.InputMessage = model.InputMessage;
                msg.OutputMessage = model.OutputMessage;
                msg.BotId = model.BotId;

                await _context.SaveChangesAsync();

                return RedirectToAction("List");
            }

            return View(model);
        }
    }
}