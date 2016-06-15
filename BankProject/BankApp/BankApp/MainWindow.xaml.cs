using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BankLogic _bankLogic;

        public MainWindow()
        {
            InitializeComponent();
            _bankLogic = BankLogic.GetInstance();
            InitializeGUI();
        }

        public void InitializeGUI()
        {
            btnRemoveCustomer.IsEnabled = false;
            lblSelectedCustomer.Content = null;
            SetActiveWindow(gridMainPage);
            UpdateCustomersLists();
        }
        private void PrintCustomers()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Customers Information";
            dialog.Filter = "Textfil (*.txt)|*.txt|Alla filer (*.*)|*.*";
            dialog.DefaultExt = ".txt";
            bool? result = dialog.ShowDialog();

            if (result.HasValue)
            {
                if (result.Value)
                {
                    string textToSave = "";
                    foreach (var item in listBCustomers.Items)
                    {
                        textToSave += item.ToString() + System.Environment.NewLine;
                    }
                    string path = dialog.FileName;
                    File.WriteAllText(path, textToSave);
                }
            }
        }
        #region Data_Updates
        //////////
        private void listCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Sets the active customer
            if (listBCustomers.SelectedItem != null)
            {
                btnRemoveCustomer.IsEnabled = true;
                _bankLogic.SetActiveCustomer(gridMainPage.Visibility == Visibility.Visible ? listBCustomers.SelectedIndex : listBCustomers.SelectedIndex);
                UpdateGUIHeaders();
            }
        }

        private void RemoveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Remove Customer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No){}
            else
            {
                if (listBCustomers.SelectedItem != null)
                {
                    listBRemovedCustomerInfo.ItemsSource = _bankLogic.RemoveCustomer();
                    InitializeGUI();
                }
            }
        }

        private void btnAddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFirstName.Text) && !string.IsNullOrEmpty(txtLastName.Text))
            {
                double SSN; 
                int SSNlength = 10;
                if (double.TryParse(txtCustomerSSN.Text, out SSN) && txtCustomerSSN.Text.Length == SSNlength)
                {
                    if (_bankLogic.AddCustomer(txtFirstName.Text, txtLastName.Text, txtCustomerSSN.Text))
                    {
                        MessageBox.Show("Successfully added new customer!");
                        UpdateCustomersLists();
                        txtFirstName.Clear();
                        txtLastName.Clear();
                        txtCustomerSSN.Clear();
                    }   
                }
                else
                    MessageBox.Show("Invalid social security number, please type in format \"YYMMDDXXXX\"");
            }
            else
                MessageBox.Show("Please fill in your first name and/or last name!");
        }

        private void UpdateCustomerName_Click(object sender, RoutedEventArgs e)
        {
            if(_bankLogic.ChangeCustomerName(txtNewFirstName.Text, txtNewLastName.Text))
            {
                MessageBox.Show("Successfully updated customer name");
                UpdateGUIHeaders();
                UpdateCustomersLists();
            }
            else
                MessageBox.Show("Customer name not updated. Customer must be given both a first and last name");
        }

        //ACCOUNT
        //////////
        private void cmbBoxAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBoxAccounts.SelectedItem != null || cmbBoxAccounts.SelectedItem != null)
            {
                _bankLogic.SetActiveAccount(gridUpdateCustomer.Visibility == Visibility.Visible ? cmbBoxAccounts.SelectedIndex : cmbBoxAccounts.SelectedIndex);
                listBAccountsTransactions.Items.Clear();
                UpdateAccountInfos(_bankLogic.DisplayAccountInfo());
                btnPrintTransactions.IsEnabled = true;
            }
        }

        private void btnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (_bankLogic.ActiveAccount != null)
            {
                decimal amount = 0;
                bool freeWithdrawUsed = false;
                if (decimal.TryParse(txtAmount.Text, out amount))
                {
                    if (amount >= 0)
                    {

                        if (_bankLogic.ActiveAccount is SavingsAccount)
                        {
                            freeWithdrawUsed = ((SavingsAccount)_bankLogic.ActiveAccount).FreeWithdrawDate.Year == DateTime.Now.Year;

                        }
                        if (_bankLogic.Withdraw(amount))
                        {
                            string withdrawMessage = $"Withdrew {amount} credits";
       
                            if (_bankLogic.ActiveAccount is SavingsAccount)
                            {
                                if (!freeWithdrawUsed)
                                {
                                    withdrawMessage += "\r\n\r\nFree withdraw used!";
                                }
                                else
                                {
                                    withdrawMessage += "\r\n\r\nWithdraw fee added: "
                                        + (amount * ((SavingsAccount)_bankLogic.ActiveAccount).WithdrawCharge - amount)
                                        + " credits";
                                }
                            }

                            MessageBox.Show(withdrawMessage);
                            listBAccountsTransactions.Items.Clear();
                            UpdateAccountInfos(_bankLogic.DisplayAccountInfo());
                        }
                        else
                            MessageBox.Show("Not enough dineros on your account, amigo");
                    }
                    else
                        MessageBox.Show("Cant withdraw negative number, bro!");
                }
                else
                    MessageBox.Show("Error, amount to withdraw incorrectly entered");
            }
            else
                MessageBox.Show("Select an account to withdraw from");
        }

        private void btnDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (_bankLogic.ActiveAccount != null)
            {
                decimal amount = 0;

                if (decimal.TryParse(txtAmount.Text, out amount))
                {
                    if (amount >= 0)
                    {
                        if(_bankLogic.Deposit(amount))
                        {
                            MessageBox.Show($"Deposited {amount} credits");

                            listBAccountsTransactions.Items.Clear();
                            UpdateAccountInfos(_bankLogic.DisplayAccountInfo());
                        }
                        else
                            MessageBox.Show("Error, amount to deposit incorrectly entered");
                    }
                    else
                        MessageBox.Show("Cant deposit negative number, bro!");
                }
                else
                    MessageBox.Show("Error, amount to deposit incorrectly entered");
            }
            else
                MessageBox.Show("Select an account to deposit to");
        }

        private void btnRemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (_bankLogic.ActiveAccount != null)// && cmbBoxAccounts.SelectedItem != null)
            {
                    listBAccountsTransactions.Items.Clear();
                    listBAccountsTransactions.Items.Add(_bankLogic.CloseAccount());
                    listBAccountsTransactions.SelectedIndex = listBAccountsTransactions.Items.Count - 1;

                    UpdateAccountsLists();
                    cmbBoxAccounts.SelectedItem = null;
            }   
        }

        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            int addedSuccessfully = -1;

            if ((sender as Button).Name == "btnAddSavingsAccount")
                addedSuccessfully = _bankLogic.AddSavingsAccount();
            else if ((sender as Button).Name == "btnAddCreditAccount")
                addedSuccessfully = _bankLogic.AddCreditAccount();

            MessageBox.Show(addedSuccessfully != -1 ? "Success!" : "Failed to add new account!");
            UpdateAccountsLists();
        }

        #endregion

        #region GUI_NavigationButtons
        //////////////////////////
        private void btnMenuNavigation_Click(object sender, RoutedEventArgs e)
        {
            string butt = (sender as Button).Name;

            switch (butt)
            {
                //case "btnAddCustomerPage":
                //    {
                //        txtFirstName.Clear();
                //        txtLastName.Clear();
                //        txtCustomerSSN.Clear();
                //      //  SetActiveWindow(gridNewCustomer);
                //        break;
                //    }
                //case "btnBackFromNewCustomer":
                //    {
                //        SetActiveWindow(gridMainPage);
                //        break;
                //    }
                case "btnEditCustomerPage":
                    {
                        if (_bankLogic.ActiveCustomer != null)
                        {
                            UpdateAccountsLists();
                            cmbBoxAccounts.SelectedItem = null;

                            SetActiveWindow(gridUpdateCustomer);
                        }
                        else
                            MessageBox.Show("No customer selected");
                        break;
                    }
                //case "btnBackFromEditCustomer":
                //    {
                //        SetActiveWindow(gridMainPage);
                //        cmbBoxAccounts.ItemsSource = null;
                //        cmbBoxTransactions.ItemsSource = null;
                //        break;
                //    }
                //case "btnRemoveCustomerPage":
                //    {
                //        listBCustomers.SelectedItem = null;
                //        SetActiveWindow(gridRemoveCustomer);
                //        break;
                //    }
                //case "btnBackFromCustomerRemove":
                //    {
                //        _bankLogic.ActiveCustomer = null;
                //        listBRemoveCustomer.SelectedItem = null;
                //        SetActiveWindow(gridMainPage);
                //        break;
                //    }
                //case "btnCustomerTransactionsPage":
                //    {
                //        listBAccountsTransactions.Items.Clear();
                //        SetActiveWindow(gridTransactions);
                //        break;
                //    }
                //case "btnBackFromTransactions":
                //    {
                //        cmbBoxTransactions.SelectedItem = null;
                //        btnPrintTransactions.IsEnabled = false;
                //        SetActiveWindow(gridCustomerInfo);
                //        break;
                //    }
                //case "btnCustomerAccountsPage":
                //    {
                //        listBAccounts.Items.Clear();
                //        SetActiveWindow(gridCustomerAccounts);
                //        break;
                //    }
                //case "btnBackFromCustomerAccounts":
                //    {
                //        cmbBoxAccounts.SelectedItem = null;
                //        SetActiveWindow(gridCustomerInfo);
                //        break;
                //    }
                //case "btnUpdateCustomerInfoPage":
                //    {
                //        txtNewFirstName.Text = _bankLogic.ActiveCustomer.FName;
                //        txtNewLastName.Text = _bankLogic.ActiveCustomer.LName;
                //        SetActiveWindow(gridUpdateCustomer);
                //        break;
                //    }
                case "btnBackFromUpdateCustomer":
                    {
                        SetActiveWindow(gridMainPage);
                        listBAccountsTransactions.Items.Clear();
                        break;
                    }
                case "btnPrintTransactions":
                    {
                        PrintTransactions();
                        break;
                    }
                case "btnPrintCustList":
                    {
                        PrintCustomers();
                        break;
                    }
                default:
                    break;
            }
        }

        private void PrintTransactions()
        {
            if (cmbBoxAccounts.SelectedItem != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save Transaction Information";
                dialog.Filter = "Textfil (*.txt)|*.txt|Alla filer (*.*)|*.*";
                dialog.DefaultExt = ".txt";
                bool? result = dialog.ShowDialog();

                if (result.HasValue)
                {
                    if (result.Value)
                    {
                        string textToSave = "";
                        foreach (var item in _bankLogic.GetTransactions())
                        {
                            textToSave += item.ToString() + System.Environment.NewLine;
                        }
                        string path = dialog.FileName;
                        File.WriteAllText(path, textToSave);
                    }
                }
            }
        }

        #endregion

        #region GUI_Updates

        private void UpdateCustomersLists()
        {
            listBCustomers.ItemsSource = _bankLogic.GetCustomers();
        }

        private void UpdateAccountsLists()
        {
            if (_bankLogic.ActiveCustomer.Accounts != null)
            {
                cmbBoxAccounts.ItemsSource = _bankLogic.ActiveCustomer.GetAccounts();
            }
            else
            {
                cmbBoxAccounts.Items.Clear();
            }
        }

        private void UpdateAccountInfos(IList<string> listContent)
        {
            foreach (var item in listContent)
            {
                listBAccountsTransactions.Items.Add(item);
            }
            //listBAccountsTransactions.ItemsSource = _bankLogic.DisplayAccountInfo();
        }

        private void SetActiveWindow(Grid activeWindow)
        {
            gridMainPage.Visibility = Visibility.Hidden;
            gridUpdateCustomer.Visibility = Visibility.Hidden;
            activeWindow.Visibility = Visibility.Visible;
        }

        private void UpdateGUIHeaders()
        {
            lblSelectedCustomer.Content = $"{_bankLogic.ActiveCustomer.FName} {_bankLogic.ActiveCustomer.LName}";
        }
        #endregion

        private void btnUnselectCustomer_Click(object sender, RoutedEventArgs e)
        {
            listBCustomers.UnselectAll();
            _bankLogic.ActiveCustomer = null;
            btnRemoveCustomer.IsEnabled = false;
            //InitializeGUI();
        }
    }
}
