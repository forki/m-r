﻿using System;
using System.Web.Mvc;
using SimpleCQRS;

namespace CQRSGui.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private FakeBus.FakeBus _bus;
        private ReadModel.IReadModelFacade _readmodel;

        public HomeController()
        {
            _bus = ServiceLocator.Bus;
            _readmodel = new ReadModel.ReadModelFacade();
        }

        public ActionResult Index()
        {
            ViewData.Model = _readmodel.GetInventoryItems();

            return View();
        }

        public ActionResult Details(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            _bus.Send(Messages.toCommand(new Commands.CreateInventoryItem(Guid.NewGuid(), name)));

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name, int version)
        {
            _bus.Send(Messages.toCommand(new Commands.RenameInventoryItem(id, name, version)));

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id, int version)
        {
            _bus.Send(Messages.toCommand(new Commands.DeactivateInventoryItem(id, version)));
            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            _bus.Send(Messages.toCommand(new Commands.CheckInItemsToInventory(id, number, version)));
            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            _bus.Send(Messages.toCommand(new Commands.RemoveItemsFromInventory(id, number, version)));
            return RedirectToAction("Index");
        }
    }
}
