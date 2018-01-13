using BitcoinDeveloperTools.Common;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinDeveloperTools.Model
{
    class FindTransaction : BaseModel
    {
        private List<BitcoinSecret> _secretKeyList;
        private Money _recievedAmount;
        private Money _spentAmount;
        private Money _fee;
        private int _outpointCount;
        private List<ICoin> _recievedCoins;
        private List<ICoin> _spentCoins;
        private Transaction _transaction;
        private QBitNinjaClient _client;
        private int _block;

        public List<BitcoinSecret> SecretKeyList
        {
            get { return _secretKeyList; }

            set { this.SetProperty(ref this._secretKeyList, value); }
        }
        public Money RecievedAmount
        {
            get { return _recievedAmount; }

            set { this.SetProperty(ref this._recievedAmount, value); }
        }
        public Money SpentAmount
        {
            get { return _spentAmount; }

            set { this.SetProperty(ref this._spentAmount, value); }
        }
        public Money Fee
        {
            get { return _fee; }

            set { this.SetProperty(ref this._fee, value); }
        }
        public int OutpointCount
        {
            get { return _outpointCount; }

            set { this.SetProperty(ref this._outpointCount, value); }
        }

        public List<ICoin> RecievedCoins
        {
            get { return _recievedCoins; }

            set { this.SetProperty(ref this._recievedCoins, value); }

        }

        public List<ICoin> SpentCoins
        {
            get { return _spentCoins; }

            set { this.SetProperty(ref this._spentCoins, value); }
        }

        public Transaction Transaction
        {
            get { return _transaction; }

            set { this.SetProperty(ref this._transaction, value); }
        }

        public QBitNinjaClient Client
        {
            get { return _client; }

            set { this.SetProperty(ref this._client, value); }
        }

        public int Block
        {
            get { return _block; }

            set { this.SetProperty(ref this._block, value); }
        }
    }
}
