using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcoinDeveloperTools.Common;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace BitcoinDeveloperTools.Model
{
    public class SendCoin: BaseModel
    {
        private List<ISecret> _secretKeyList;
        private Money _currentBalance;
        private Money _recieverAmount;
        private Money _minerFee;
        private Money _changeBackAmount;
        private List<ICoin> _coinList;
        private Transaction _transaction;
        private int _blockConfirmations;

        public List<ISecret> SecretKeyList
        {
            get { return _secretKeyList; }

            set { this.SetProperty(ref this._secretKeyList, value); }
        }

        public Money CurrentBalance
        {
            get { return _currentBalance; }

            set { this.SetProperty(ref this._currentBalance, value); }
        }

        public Money RecieverAmount
        {
            get { return _recieverAmount; }

            set { this.SetProperty(ref this._recieverAmount, value); }
        }

        public Money MinerFee
        {
            get { return _minerFee; }

            set { this.SetProperty(ref this._minerFee, value); }
        }

        public Money ChangeBackAmount
        {
            get { return _changeBackAmount; }

            set { this.SetProperty(ref this._changeBackAmount, value); }
        }

        public List<ICoin> CoinList
        {
            get { return _coinList; }

            set { this.SetProperty(ref this._coinList, value); }
        }

        public Transaction Transaction
        {
            get { return _transaction; }

            set { this.SetProperty(ref this._transaction, value); }
        }

        public int BlockConfirmations
        {
            get { return _blockConfirmations; }

            set { this.SetProperty(ref this._blockConfirmations, value); }
        }
    }

}
