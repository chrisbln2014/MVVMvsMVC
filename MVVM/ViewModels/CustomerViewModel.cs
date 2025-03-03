using MVVM.Models;

namespace MVVM.ViewModels
{
    public class CustomerViewModel
    {
        public List<Customer> Customers { get; private set; }

        public CustomerViewModel()
        {
            try
            {
                Customers = Customer.GetAllCustomers();
                Console.WriteLine($"CustomerViewModel 成功載入了 {Customers.Count} 個客戶資料");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CustomerViewModel 載入資料時發生錯誤: {ex.Message}");
                Customers = new List<Customer>();
            }
        }
    }
}