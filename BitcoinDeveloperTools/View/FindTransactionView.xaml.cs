using BitcoinDeveloperTools.ViewModel;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BitcoinDeveloperTools.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FindTransactionView : Page
    {
        private FindTransactionViewModel primaryVM = new FindTransactionViewModel();
        DataPackage dataPackage = new DataPackage();

        public FindTransactionView()
        {
            this.InitializeComponent();
            this.DataContext = primaryVM;
        }
        private void FindTxBtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            ListTxSummary.Items.Clear();
            primaryVM.RecievedCoins = null;
            primaryVM.SpentCoins = null;
            primaryVM.Block = 0;
            try
            {
                if (MainTxBtn.IsChecked == true)
                {
                    primaryVM.TxQueryMain(TxInputBox.Text);
                }
                else
                {
                    primaryVM.TxQueryTestNet(TxInputBox.Text);
                }

                if (primaryVM.RecievedCoins != null)
                {
                    primaryVM.RecievedAmount = Money.Zero;
                    ListTxSummary.Items.Add("All Recieved Coins");
                }

                //Sum: Total Recieved
                foreach (var recievedCoin in primaryVM.RecievedCoins)
                {
                    primaryVM.RecievedAmount = (Money)recievedCoin.Amount.Add(primaryVM.RecievedAmount);
                }
                //List: Recieved Coins
                if (MainTxBtn.IsChecked == true)
                {
                    foreach (Coin coin in primaryVM.RecievedCoins)
                    {
                        var amount = coin.Amount;

                        var paymentScript = coin.ScriptPubKey;
                        var address = paymentScript.GetDestinationAddress(Network.Main);
                        ListTxSummary.Items.Add($"Recieved +{amount.ToDecimal(MoneyUnit.BTC)}BTC To: {address} : ScriptPubKey: {paymentScript}");
                    }
                }
                else
                {
                    foreach (Coin coin in primaryVM.RecievedCoins)
                    {
                        var amount = coin.Amount;

                        var paymentScript = coin.ScriptPubKey;
                        var address = paymentScript.GetDestinationAddress(Network.TestNet);
                        ListTxSummary.Items.Add($"Recieved +{amount.ToDecimal(MoneyUnit.BTC)}BTC To: {address} : ScriptPubKey: {paymentScript}");
                    }
                }


                primaryVM.SpentAmount = Money.Zero;
                ListTxSummary.Items.Add("All Spent Coins");
                //Sum: Total Spent
                foreach (var spentCoin in primaryVM.SpentCoins)
                {
                    primaryVM.SpentAmount = (Money)spentCoin.Amount.Add(primaryVM.SpentAmount);
                }
                //List: Total Spent
                if (MainTxBtn.IsChecked == true)
                {
                    foreach (Coin coin in primaryVM.SpentCoins)
                    {
                        Money amount = coin.Amount;

                        var paymentScript = coin.ScriptPubKey;
                        var address = paymentScript.GetDestinationAddress(Network.Main);
                        ListTxSummary.Items.Add($"Spent -{amount.ToDecimal(MoneyUnit.BTC)}BTC From: {address} : ScriptPubKey: {paymentScript}");
                    }
                }
                else
                {
                    foreach (Coin coin in primaryVM.SpentCoins)
                    {
                        Money amount = coin.Amount;

                        var paymentScript = coin.ScriptPubKey;
                        var address = paymentScript.GetDestinationAddress(Network.TestNet);
                        ListTxSummary.Items.Add($"Spent -{amount.ToDecimal(MoneyUnit.BTC)}BTC From: {address} : ScriptPubKey: {paymentScript}");
                    }
                }


                // Previous Outpoints
                var inputs = primaryVM.Transaction.Inputs;
                primaryVM.OutpointCount = inputs.Count;
                var firstOutpoint = primaryVM.RecievedCoins.First().Outpoint;
                var firstTransaction = primaryVM.Client.GetTransaction(firstOutpoint.Hash).Result.Transaction;
                ListTxSummary.Items.Add("All Outpoints");
                ListTxSummary.Items.Add($"#({primaryVM.OutpointCount + 1}) Current Outpoint = {firstOutpoint.Hash} , {firstOutpoint.N} , {firstTransaction.IsCoinBase.ToString()}");

                var countOrder = primaryVM.OutpointCount;
                foreach (TxIn input in inputs)
                {
                    OutPoint previousOutpoint = input.PrevOut;
                    var previousTransaction = primaryVM.Client.GetTransaction(previousOutpoint.Hash).Result.Transaction;
                    ListTxSummary.Items.Add($"#({countOrder}) Prev Outpoint = {previousOutpoint.Hash} , {previousOutpoint.N} , {previousTransaction.IsCoinBase.ToString()}");
                    countOrder -= 1;
                }
            }
            catch (Exception ex)
            {
                TxInputBox.Text = ex.ToString();
            }
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainView));
        }

        private void PasteTxButton_Click(object sender, RoutedEventArgs Exception)
        {
            PasteErrorText.Text = string.Empty;
            DataPackageView PasteData = Clipboard.GetContent();
            try
            {
                if (PasteData.Contains(StandardDataFormats.Text))
                {
                    IAsyncOperation<string> ClipboardAsync = PasteData.GetTextAsync();
                    string StringData = ClipboardAsync.GetResults();
                    TxInputBox.Text = StringData;
                }
            }
            catch (Exception)
            {
                PasteErrorText.Text = "Try Again...";

            }
        }

        private void SendCoins_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendCoinView));
        }
    }
}