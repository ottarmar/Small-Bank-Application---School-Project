using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace BankApp
{
    class EricTestKlasser
    {
        public static void RunDemo()
        {
            //TEST


            //TEST 2
        }

        public abstract class Account
        {
            public decimal Balance { get; set; }

            void Deposit(decimal amountToDeposit)
            {
                Balance += amountToDeposit;
            }

            void Withdraw(decimal amountToWithdraw) { }
        }

        public class SavingsAccount : Account
        {

        }

        public class CreditAccount : Account
        {


            /*
            TEMP
            */

            void saveToTileButton_Cilck(object sender)
            {
                SaveFileDialog dialog = new SaveFileDialog();//using Microsoft.Win32;
                dialog.Title = "Välj filnamn";
                dialog.Filter = "Textfil (*.txt)|*.txt|Alla filer (*.*)|*.*";
                dialog.DefaultExt = ".txt";
                bool? result = dialog.ShowDialog(); //bool? = antingen true/false ELLER null
                //result.HasValue //true=har värde. false=null

                if (result.HasValue && result.Value)
                {
                    string textToSave = "";

                    foreach (var item in usersListBox.Items)
                    {
                        textToSave += item.ToString() 
                            + System.Environment.NewLine;
                    }

                    string path = dialog.FileName;
                    File.WriteAllText(path, textToSave); //using System.IO.File
                }
            }
        }

        //abstract class Account

        //class SavingsAccount : Accoutn

        //class CreditAccount : Account

    }
}
