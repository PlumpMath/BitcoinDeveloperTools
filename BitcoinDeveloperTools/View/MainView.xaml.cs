using BitcoinDeveloperTools.ViewModel;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
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
    public sealed partial class MainView : Page
    {
        private MainViewModel defaultViewModel = new MainViewModel();
        //Create folder for App Data Storage
        StorageFolder storageFolder = ApplicationData.Current.LocalCacheFolder;
        DataPackage dataPackage = new DataPackage();


        public MainView()
        {
            this.InitializeComponent();
            this.DataContext = defaultViewModel;
            
            try
            {
                IfWorksAsync();
            }
            catch (Exception e)
            {
                ErrorText.Text = "Could not 'Create' or 'Read' TestNet Address | Try Reinstalling...";
            }
        }

        public async void ReadBtcTestAddress_Async()
        {
            var folderFile = await storageFolder.GetFolderAsync("BTCDEVSECURE");
            var addressFile = await folderFile.GetFileAsync("btc_test_address.txt");
            var keyFile = await folderFile.GetFileAsync("btc_test_key.txt");
            var btcTestAddress = await FileIO.ReadTextAsync(addressFile);
            var secretkey = await FileIO.ReadTextAsync(keyFile);
            BtcTestAddress.Text = btcTestAddress.ToString();
            BtcTestKey.Text = secretkey.ToString();

        }

        private async void CreateBtcTestAddress_Async()
        {
            var network = Network.TestNet;
            Key privateKey = new Key();
            var secretKey = privateKey.GetBitcoinSecret(Network.TestNet);
            var btcAddress = secretKey.PubKey.GetAddress(Network.TestNet);

            var folderFile = await storageFolder.CreateFolderAsync("BTCDEVSECURE", CreationCollisionOption.ReplaceExisting);
            var addressFile = await folderFile.CreateFileAsync("btc_test_address.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(addressFile, btcAddress.ToString());
            var secretFile = await folderFile.CreateFileAsync("btc_test_key.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(secretFile, secretKey.ToString());
            //var faToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(addressFile);
            BtcTestAddress.Text = btcAddress.ToString();
            BtcTestKey.Text = secretKey.ToString();
        }

        public async Task<bool> IfStorageItemExist(StorageFolder folder, string itemName)
        {
            try
            {
                IStorageItem item = await folder.TryGetItemAsync(itemName);
                return (item != null);
            }
            catch (Exception ex)
            {
                // Should never get here 
                return false;
            }
        }

        public async void IfWorksAsync()
        {
            if (await IfStorageItemExist(storageFolder, "BTCDEVSECURE"))
            {
                // file/folder exists.
                ReadBtcTestAddress_Async();
                ErrorText.Foreground = (SolidColorBrush)Resources["GreenColor"];
                ErrorText.Text = "Succesfully Read TestNet Address";
            }
            else
            {
                // file/folder does not exist.
                CreateBtcTestAddress_Async();
                ErrorText.Foreground = (SolidColorBrush)Resources["GreenColor"];
                ErrorText.Text = "Succesfully Created TestNet Address";
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendCoinView));

        }

        private void RecieveButton_Click(object sender, RoutedEventArgs e)
        {
            //RecieveButton.Background = (SolidColorBrush)Resources["BlueColor"];
        }

        private void FindTransaction_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FindTransactionView));
            
        }

        private void CopyTestButton_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(BtcTestAddress.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void CopyTestKeyButton_Click(object sender, RoutedEventArgs e)
        {
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(BtcTestKey.Text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
