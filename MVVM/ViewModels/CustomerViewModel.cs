using MVVM.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM.ViewModels
{
    // ViewModel 負責處理視圖（View）所需的資料和邏輯
    // 這是 MVVM 模式的核心組件之一，負責連接 Model 和 View
    // 實現 INotifyPropertyChanged 接口以支援屬性變更通知，這是MVVM雙向繫結的核心
    public class CustomerViewModel : INotifyPropertyChanged
    {
        // 事件：當屬性變更時觸發，View 會監聽這個事件來更新 UI
        public event PropertyChangedEventHandler? PropertyChanged;

        // 儲存客戶資料的集合，只提供讀取權限
        // private set 表示只能在類別內部修改
        public List<Customer> Customers { get; private set; }

        // 當前選中用於編輯的客戶
        private Customer? _selectedCustomer;
        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer != value)
                {
                    _selectedCustomer = value;
                    // 如果選擇了新客戶，創建其副本以進行編輯
                    if (_selectedCustomer != null)
                    {
                        CustomerToEdit = new Customer
                        {
                            CustomerID = _selectedCustomer.CustomerID,
                            CustomerName = _selectedCustomer.CustomerName,
                            CustomerLocation = _selectedCustomer.CustomerLocation,
                            Email = _selectedCustomer.Email,
                            Phone = _selectedCustomer.Phone,
                            Address = _selectedCustomer.Address
                        };
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        // 用於編輯的客戶資料物件
        private Customer? _customerToEdit;
        public Customer? CustomerToEdit
        {
            get => _customerToEdit;
            set
            {
                _customerToEdit = value;
                NotifyPropertyChanged();
            }
        }

        // 新增的客戶資料物件，初始化時設定必要屬性的默認值
        private Customer _newCustomer = new Customer
        {
            CustomerID = 0, // 暫時設為0，實際新增時會由 AddCustomer 方法設定正確的ID
            CustomerName = string.Empty,
            CustomerLocation = string.Empty
        };
        
        public Customer NewCustomer
        {
            get => _newCustomer;
            set
            {
                _newCustomer = value;
                NotifyPropertyChanged();
            }
        }

        // 是否有未儲存的變更
        private bool _hasChanges;
        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                _hasChanges = value;
                NotifyPropertyChanged();
            }
        }

        // 錯誤訊息
        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged();
            }
        }

        // 成功訊息
        private string? _successMessage;
        public string? SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                NotifyPropertyChanged();
            }
        }

        // 建構函式：當創建 ViewModel 實例時執行
        public CustomerViewModel()
        {
            try
            {
                // 從 Model 層獲取客戶資料
                Customers = Customer.GetAllCustomers();
                Console.WriteLine($"CustomerViewModel 成功載入了 {Customers.Count} 個客戶資料");
                
                // 初始化新客戶物件
                ResetNewCustomer();
            }
            catch (Exception ex)
            {
                // 如果發生錯誤，建立空列表並記錄錯誤
                Console.WriteLine($"CustomerViewModel 載入資料時發生錯誤: {ex.Message}");
                Customers = new List<Customer>();
                ErrorMessage = $"載入資料時發生錯誤: {ex.Message}";
            }
        }

        // 選擇指定ID的客戶進行編輯
        public void SelectCustomerForEdit(int id)
        {
            var customer = Customer.GetCustomerById(id);
            if (customer != null)
            {
                SelectedCustomer = customer;
            }
        }

        // 保存客戶資料變更
        public bool SaveChanges()
        {
            ErrorMessage = null;
            
            if (CustomerToEdit == null)
            {
                ErrorMessage = "沒有需要儲存的資料";
                return false;
            }

            try
            {
                // 調用 Model 層的方法保存資料
                bool result = Customer.UpdateCustomer(CustomerToEdit);
                if (result)
                {
                    HasChanges = false;
                    // 更新列表中的客戶資料
                    var customerInList = Customers.FirstOrDefault(c => c.CustomerID == CustomerToEdit.CustomerID);
                    if (customerInList != null)
                    {
                        customerInList.CustomerName = CustomerToEdit.CustomerName;
                        customerInList.CustomerLocation = CustomerToEdit.CustomerLocation;
                        customerInList.Email = CustomerToEdit.Email;
                        customerInList.Phone = CustomerToEdit.Phone;
                        customerInList.Address = CustomerToEdit.Address;
                    }
                    return true;
                }
                else
                {
                    ErrorMessage = "無法保存資料，可能找不到指定的客戶";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"保存資料時發生錯誤: {ex.Message}";
                return false;
            }
        }

        // 新增客戶
        public bool AddCustomer()
        {
            ErrorMessage = null;
            SuccessMessage = null;
            
            try
            {
                // 調用 Model 層的方法新增客戶
                bool result = Customer.AddCustomer(NewCustomer);
                if (result)
                {
                    // 更新客戶列表
                    var updatedCustomers = Customer.GetAllCustomers();
                    Customers = updatedCustomers;
                    NotifyPropertyChanged(nameof(Customers));
                    
                    // 設置成功消息
                    SuccessMessage = "客戶已成功新增";
                    
                    // 重設新客戶表單
                    ResetNewCustomer();
                    
                    return true;
                }
                else
                {
                    ErrorMessage = "無法新增客戶";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"新增客戶時發生錯誤: {ex.Message}";
                return false;
            }
        }
        
        // 刪除客戶
        public bool DeleteCustomer(int id)
        {
            ErrorMessage = null;
            SuccessMessage = null;
            
            try
            {
                // 調用 Model 層的方法刪除客戶
                bool result = Customer.DeleteCustomer(id);
                if (result)
                {
                    // 更新客戶列表
                    var updatedCustomers = Customer.GetAllCustomers();
                    Customers = updatedCustomers;
                    NotifyPropertyChanged(nameof(Customers));
                    
                    // 設置成功消息
                    SuccessMessage = "客戶已成功刪除";
                    return true;
                }
                else
                {
                    ErrorMessage = "無法刪除客戶，可能找不到指定的客戶";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"刪除客戶時發生錯誤: {ex.Message}";
                return false;
            }
        }
        
        // 重設新客戶表單
        public void ResetNewCustomer()
        {
            NewCustomer = new Customer
            {
                CustomerID = 0, // 暫時設為0，實際新增時會由 AddCustomer 方法設定正確的ID
                CustomerName = string.Empty,
                CustomerLocation = string.Empty
            };
        }

        // 當資料變更時呼叫此方法，通知View更新UI
        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}