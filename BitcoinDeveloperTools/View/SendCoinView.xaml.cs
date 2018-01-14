using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BitcoinDeveloperTools.ViewModel;
using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BitcoinDeveloperTools.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendCoinView : Page
    {
        private SendCoinViewModel primaryVM = new SendCoinViewModel();
        private FindTransactionViewModel queryVM = new FindTransactionViewModel();
        DataPackage dataPackage = new DataPackage();

        public SendCoinView()
        {
            this.InitializeComponent();
        }


        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //#region CREATE NEW PRIVKEY
            ////var network = Network.TestNet;
            ////Key privateKey = new Key();
            ////var bitcoinPrivateKey = privateKey.GetWif(network);
            //#endregion
            ErrorText.Text = string.Empty;
            try
            {
                #region IMPORT PRIVKEY
                var bitcoinPrivateKey = new BitcoinSecret(PrivateKeyBox.Text);
                var network = bitcoinPrivateKey.Network;
                var address = bitcoinPrivateKey.GetAddress();
                #endregion

                //Create Client and Query Tx
                var client = new QBitNinjaClient(network);
                var transactionId = uint256.Parse(OwnerTxBox.Text);
                var transactionResponse = client.GetTransaction(transactionId).Result;


                // find most suitable transaction
                var historyTransaction = client.GetBalance(bitcoinPrivateKey.ScriptPubKey, true).Result;

                // create transaction
                primaryVM.Transaction = new Transaction();
                primaryVM.CoinList = new List<ICoin>();

                // add inputs to tx
                decimal currentBalanceDecimal = 0;
                foreach (var tx in historyTransaction.Operations)
                {
                    GetTransactionResponse currentTx = client.GetTransaction(new uint256(OwnerTxBox.Text)).Result;

                    foreach (var coin in currentTx.ReceivedCoins)
                        if (((Money)coin.Amount).Satoshi > 0
                            && coin.CanGetScriptCode
                            && coin.GetScriptCode() == bitcoinPrivateKey.ScriptPubKey
                            )
                        {
                            currentBalanceDecimal += ((NBitcoin.Coin)coin).Amount.ToDecimal(MoneyUnit.BTC);
                            primaryVM.CoinList.Add(coin);
                            primaryVM.Transaction.Inputs.Add(new TxIn(coin.Outpoint, bitcoinPrivateKey.ScriptPubKey));
                        }
                }


                // calculate what amount of bitcoin needed to be send
                primaryVM.CurrentBalance = new Money(currentBalanceDecimal, MoneyUnit.BTC);
                primaryVM.MinerFee = new Money(Decimal.Parse(MinerAmountBox.Text), MoneyUnit.BTC);
                primaryVM.RecieverAmount = new Money(Decimal.Parse(SendAmountBox.Text), MoneyUnit.BTC);
                primaryVM.ChangeBackAmount = primaryVM.CurrentBalance - primaryVM.RecieverAmount - primaryVM.MinerFee;

                //Verify Recievers Address
                var recieverAddress = new BitcoinPubKeyAddress(SendAddressBox.Text);

                //Package Recievers Transaction Details
                TxOut recieverTxOut = new TxOut()
                {
                    Value = primaryVM.RecieverAmount,
                    ScriptPubKey = recieverAddress.ScriptPubKey
                };
                primaryVM.Transaction.Outputs.Add(recieverTxOut);

                //Package ChangeBack Transaction Details
                if (primaryVM.ChangeBackAmount > 0)
                {
                    TxOut changeBackTxOut = new TxOut()
                    {
                        Value = primaryVM.ChangeBackAmount,
                        ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
                    };
                    primaryVM.Transaction.Outputs.Add(changeBackTxOut);
                }
                else
                {

                }
                ////Add message to coin
                //var message = SignTxBox.Text;
                //var bytes = Encoding.UTF8.GetBytes(message);
                //TxOut signedMessage = new TxOut()
                //{
                //    Value = Money.Zero,
                //    ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
                //};
                //transaction.Outputs.Add(signedMessage);

                //var keys = new List<ExtKey>();
                //keys.Add(masterKey.ExtKey.Derive((uint)1)); 
                //Add Secret Keys to list
                primaryVM.SecretKeyList = new List<ISecret>
            {
                bitcoinPrivateKey
            };

                //Sign Transaction
                primaryVM.Transaction.Sign(primaryVM.SecretKeyList.ToArray(), primaryVM.CoinList.ToArray());

                //Verify Transaction Build Success
                var builder = new TransactionBuilder();
                NBitcoin.Policy.TransactionPolicyError[] error = null;
                var checker = (builder.Verify(primaryVM.Transaction, out error));
                if (checker == false)
                {
                    ErrorText.Foreground = (SolidColorBrush)Resources["GreenColor"];
                    ErrorText.Text = "Transaction Succecfully Completed Awaiting Confirmations...";
                }
                else
                {
                    ErrorText.Text = "Error Building Transaction, Please Try Again";
                }


                // Broadcast your transaction to all miners
                BroadcastResponse broadcastResponse = client.Broadcast(primaryVM.Transaction).Result;
                if (!broadcastResponse.Success)
                {
                    var errorcode = new Exception(broadcastResponse.Error.Reason);
                    ErrorText.Text = errorcode.ToString();
                }
                historyTransaction = client.GetBalance(bitcoinPrivateKey.ScriptPubKey, true).Result;
                uint256[] txHistoryArray = new uint256[25];
                var count = 0;
                foreach (var tx in historyTransaction.Operations)
                {

                    txHistoryArray[count] = tx.TransactionId;
                    count += 1;
                }

                TxCreated.Text = txHistoryArray[0].ToString();
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Nothing Sent, Make sure you are filling out form completely and accurately"; 
            }
        }
        private void FindTransaction_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FindTransactionView));
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainView));
        }
        private void SendAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //SendAmountBox.Visibility = Visibility.Visible;
        }

        private void SendAmountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MinerAmountBox.Visibility = Visibility.Visible;
        }

        private void MinerAmountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //OwnerTxBox.Visibility = Visibility.Visible;
        }

        private void OwnerTxBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //PrivateKeyBox.Visibility = Visibility.Visible;
        }

        private void PrivateKeyBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //SignTxBox.Visibility = Visibility.Visible;
            //SendButton.Visibility = Visibility.Visible;
        }

        private void SignTxBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //SendButton.Visibility = Visibility.Visible;
        }

        private void NewTxCopyButton_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(TxCreated.Text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
