﻿using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using flint;

namespace Windows.Pebble.ViewModels
{
    public class PebbleAppsViewModel : ViewModelBase
    {
        private readonly flint.Pebble _pebble;
        private readonly BindingList<AppBank.App> _apps = new BindingList<AppBank.App>();

        private readonly RelayCommand<AppBank.App> _removeAppCommand;
        private readonly RelayCommand _installAppCommand;

        public PebbleAppsViewModel( flint.Pebble pebble )
        {
            _pebble = pebble;

            _removeAppCommand = new RelayCommand<AppBank.App>( OnRemoveApp );
            _installAppCommand = new RelayCommand( OnInstallApp );
        }

        public ICollectionView Apps
        {
            get { return CollectionViewSource.GetDefaultView( _apps ); }
        }

        public ICommand RemoveAppCommand
        {
            get { return _removeAppCommand; }
        }

        public ICommand InstallAppCommand
        {
            get { return _installAppCommand; }
        }


        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if ( Set( () => IsSelected, ref _IsSelected, value ) )
                {
                    LoadValuesAsync();
                }
            }
        }

        private bool _Loading;
        public bool Loading
        {
            get { return _Loading; }
            set { Set( () => Loading, ref _Loading, value ); }
        }

        public async Task OnConnectedAsync()
        {
            await LoadValuesAsync();
        }

        public async Task OnDisconnectedAsync()
        {
            await LoadValuesAsync();
        }

        private async Task LoadValuesAsync()
        {
            if (IsSelected && _pebble.Alive)
                await LoadAppsAsync();
        }

        private async Task LoadAppsAsync()
        {
            if ( _pebble.Alive == false )
                return;

            Loading = true;
            var appBankContents = await _pebble.GetAppbankContentsAsync();
            _apps.Clear();
            if ( appBankContents.Success )
                foreach ( var app in appBankContents.AppBank.Apps )
                    _apps.Add( app );
            Loading = false;
        }

        private async void OnRemoveApp( AppBank.App app )
        {
            if ( _pebble.Alive == false )
                _pebble.Connect();

            await _pebble.RemoveAppAsync( app );
            await LoadAppsAsync();
        }

        private async void OnInstallApp()
        {
            var openDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "*.pbw",
                Filter = "Pebble Apps|*.pbw",
                RestoreDirectory = true,
                Title = "Pebble App"
            };
            if ( openDialog.ShowDialog() == true )
            {
                var bundle = new PebbleBundle( openDialog.FileName );

                if ( _pebble.Alive == false )
                    _pebble.Connect();
                await _pebble.InstallAppAsync( bundle );
                await LoadAppsAsync();
            }
        }
    }
}