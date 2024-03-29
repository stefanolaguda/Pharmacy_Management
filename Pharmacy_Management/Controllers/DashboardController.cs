﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Pharmacy_Management.Models;

namespace Pharmacy_Management.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private ModelDbContext DbContext = new ModelDbContext();
        // GET: Dashboard
        public ActionResult Index()
        {
            var finishedMedicines = DbContext.Medicines.Where(m => m.Stock <= 5);
            ViewBag.FinishedMedicinesCount = finishedMedicines.Count();
            var medicines = DbContext.Medicines.Where(m => m.Stock > 0).ToList();
            int TotalStock = 0;
            foreach (var item in medicines)
            {
                TotalStock += item.Stock;
            }

            ViewBag.MedicinesCount = TotalStock;

            decimal TotalPayements = 0;
            var OrderPays = DbContext.Orders.ToList();
            foreach (var item in OrderPays)
            {
                TotalPayements += item.TotalPrice;
            }
            ViewBag.TotalOrder = TotalPayements;
            
            return View();
        }

        public ActionResult InventaryPartialView() {
            var medicines = DbContext.Medicines.Where(m => m.Stock > 0).ToList();
            int TotalStock = 0;
            foreach (var item in medicines)
            {
                TotalStock += item.Stock;
            }

            ViewBag.MedicinesCount = TotalStock;
            ViewBag.MedicineGroups = DbContext.TypeProduct.Where(p => p.DescTypeProduct != null).Count();
            return PartialView(medicines);
        }

        public ActionResult PharmacyPartialView()
        {
            ViewBag.SuppliersCount = DbContext.SupplierCompanies.Where(s => s.NameCompany != null).Count();
            ViewBag.Employee = DbContext.Employees.Where(e => e.Pwd != null).Count();
            return PartialView();
        }

        public ActionResult CustomersPartialView()
        {
            var customers = DbContext.Customers;
            ViewBag.Customers = customers.Where(c => c.FirstName != "").Count();
            var PrescritionCustomersCount = DbContext.Pescritions.Where(p => p.IdentifierPrescription != 0).Count();
            ViewBag.PrescritionsCount = PrescritionCustomersCount;
            return PartialView();
        }

        public ActionResult OrdersPartialView()
        {
            var orders = DbContext.Orders.Include(o => o.Quantity);
            ViewBag.OrderTotal = orders.Where(o => o.Medicines != null).Count();

            decimal TotalPayements = 0;
            var OrderPays = DbContext.Orders.ToList();
            foreach (var item in OrderPays)
            {
                TotalPayements += item.TotalPrice;
            }
            ViewBag.TotalOrder = TotalPayements;

            return PartialView(); 
        }

        // Funzioni ricerca in Jquery
        public JsonResult GetMedicineByCodFisc(string codFisc)
        {
            var customer = DbContext.Customers.Where(c => c.CodFisc == codFisc).Include(c => c.Orders).FirstOrDefault();

            List<MedicinesByCodFiscJson> ListBCFJ = new List<MedicinesByCodFiscJson>();

            if (customer != null)
            {
                var ListResults = DbContext.Orders.Where(o => o.IdCustomer == customer.IdCustomer).ToList();

                foreach (var result in ListResults)
                {
                    MedicinesByCodFiscJson SingleMedicine = new MedicinesByCodFiscJson();

                    // INFO CUSTOMER
                    SingleMedicine.CodFisc = customer.CodFisc;
                    SingleMedicine.FirstName = customer.FirstName;
                    SingleMedicine.LastName = customer.LastName;
                    // INFO ORDER
                    SingleMedicine.IdMedicine = result.IdMedicine;
                    SingleMedicine.Quantity = result.Quantity;
                    // INFO MEDICINE
                    Medicines medicineFind = DbContext.Medicines.Find(result.IdMedicine);
                    SingleMedicine.NameMedicine = medicineFind.NameMedicine;
                    SingleMedicine.UrlImg = medicineFind.UrlImg;
                    // INFO DRAWER
                    Drawers drawer = DbContext.Drawers.Find(medicineFind.IdDrawer);
                    SingleMedicine.IdDrawer = medicineFind.IdDrawer;
                    SingleMedicine.IdentifierDrawer = drawer.Identifier;

                    ListBCFJ.Add( SingleMedicine );
                }

                return Json(ListBCFJ, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json("ERRORE", JsonRequestBehavior.AllowGet);
            }
        }
    }
}