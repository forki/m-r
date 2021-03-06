﻿using System;
using System.Web.Mvc;

namespace CQRSGui.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private FakeBus.FakeBus _bus;

        public HomeController()
        {
            _bus = ServiceLocator.Bus;
        }

        public ActionResult Index()
        {
            ViewData.Model = ReadModel.GetInventoryItems();

            return View();
        }

        public ActionResult Details(Guid id)
        {
            ViewData.Model = ReadModel.GetInventoryItemDetails(id);
            return View();
        }

        public ActionResult Ledgers(Guid id)
        {
            ViewData.Model = ReadModel.GetInventoryItemLedgers(id);
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            _bus.Send(Messages.toCommand(Commands.InventoryItemCommand.NewCreate(Guid.NewGuid(), name)));

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = ReadModel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name, int version)
        {
            _bus.Send(Messages.toCommand(Commands.InventoryItemCommand.NewRename(id, name, version)));

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id, int version)
        {
            _bus.Send(Messages.toCommand(Commands.InventoryItemCommand.NewDeactivate(id, version)));
            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = ReadModel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            _bus.Send(Messages.toCommand(Commands.InventoryItemCommand.NewCheckInItems(id, number, version)));
            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = ReadModel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            _bus.Send(Messages.toCommand(Commands.InventoryItemCommand.NewRemoveItems(id, number, version)));
            return RedirectToAction("Index");
        }
    }
}
