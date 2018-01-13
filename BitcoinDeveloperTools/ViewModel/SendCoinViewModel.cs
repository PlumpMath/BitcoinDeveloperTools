using BitcoinDeveloperTools.Model;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinDeveloperTools.ViewModel
{
    class SendCoinViewModel : SendCoin
    {
        SendCoin _sendCoin = new SendCoin()
        {
            RecieverAmount = Money.Zero,
            CurrentBalance = Money.Zero,
            MinerFee = Money.Zero,
            Transaction = null,
            SecretKeyList = null,
        };

        public SendCoinViewModel()
        {
            this.RecieverAmount = _sendCoin.RecieverAmount;
            this.CurrentBalance = _sendCoin.CurrentBalance;
            this.MinerFee = _sendCoin.MinerFee;
            this.Transaction = _sendCoin.Transaction;
            this.SecretKeyList = _sendCoin.SecretKeyList;
        }

    }
}
