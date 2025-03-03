using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            // 將對 Customer.GetAllCustomers() 的呼叫改為對 CustomerService.GetAllCustomers() 的呼叫
            List<Customer> customers = CustomerService.GetAllCustomers();
            return View(customers);
        }
    }
}