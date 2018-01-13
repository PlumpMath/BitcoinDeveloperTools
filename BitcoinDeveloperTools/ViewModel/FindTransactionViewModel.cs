using BitcoinDeveloperTools.Model;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinDeveloperTools.ViewModel
{
    class FindTransactionViewModel : FindTransaction
    {
        FindTransaction _findTransaction = new FindTransaction()
        {
            RecievedAmount = Money.Zero,
            SpentAmount = Money.Zero,
            Fee = Money.Zero,
            OutpointCount = 0,
            Block = 0,
            SecretKeyList = null,
            Client = null,
            RecievedCoins = null,
            SpentCoins = null,
            Transaction = null,
        };

        public FindTransactionViewModel()
        {
            this.SecretKeyList = _findTransaction.SecretKeyList;
            this.RecievedAmount = _findTransaction.RecievedAmount;
            this.SpentAmount = _findTransaction.SpentAmount;
            this.Fee = _findTransaction.Fee;
            this.OutpointCount = _findTransaction.OutpointCount;
            this.RecievedCoins = _findTransaction.RecievedCoins;
            this.SpentCoins = _findTransaction.SpentCoins;
            this.Transaction = _findTransaction.Transaction;
            this.Client = _findTransaction.Client;
            this.Block = _findTransaction.Block;
        }

        public Exception Ex { get; private set; }

        public void TxQueryMain(string txInput)
        {
            try
            {
                // Create a client
                Client = new QBitNinjaClient(Network.Main);

                // Parse transaction id to NBitcoin.uint256 so the client can eat it
                var transactionId = uint256.Parse(txInput);

                // Query the transaction
                QBitNinja.Client.Models.GetTransactionResponse transactionResponse = Client.GetTransaction(transactionId).Result;
                
                // Extract Transaction
                Transaction = transactionResponse.Transaction;

                // Extract Recieved Coins: TxOut
                RecievedCoins = transactionResponse.ReceivedCoins;

                // Extract Spent Coins: TxIn
                SpentCoins = transactionResponse.SpentCoins;

                // Extract Fees Paid: Fee
                Fee = Transaction.GetFee(SpentCoins.ToArray());

                //Count Block Confirmations
                Block = transactionResponse.Block.Confirmations;
            }
            catch (Exception ex)
            {
                Ex = ex;
            }
        }

        public void TxQueryTestNet(string txInput)
        {
            try
            {
                // Create a client
                Client = new QBitNinjaClient(Network.TestNet);

                // Parse transaction id to NBitcoin.uint256 so the client can eat it
                var transactionId = uint256.Parse(txInput);

                // Query the transaction
                QBitNinja.Client.Models.GetTransactionResponse transactionResponse = Client.GetTransaction(transactionId).Result;

                // Extract Transaction
                Transaction = transactionResponse.Transaction;

                // Extract Recieved Coins: TxOut
                RecievedCoins = transactionResponse.ReceivedCoins;

                // Extract Spent Coins: TxIn
                SpentCoins = transactionResponse.SpentCoins;

                // Extract Fees Paid: Fee
                Fee = Transaction.GetFee(SpentCoins.ToArray());

                //Count Block Confirmations
                Block = transactionResponse.Block.Confirmations;
            }
            catch (Exception ex)
            {
                Ex = ex;
            }
        }
    }
}   
