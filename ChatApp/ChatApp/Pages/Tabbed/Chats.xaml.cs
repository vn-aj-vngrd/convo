﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChatApp.TempData;

namespace ChatApp.Pages.Tabbed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chats : ContentView
    {
        public Chats()
        {
            InitializeComponent();
            fetchContacts();
            
            MessagingCenter.Subscribe<MainPage>(this, "RefreshMainPage", (sender) => {
                fetchContacts();
            });
        }

        private async void Frame_GoToConvo(object sender, EventArgs e)
        {

            var user = new UserModel
            {
                email = (string)((TappedEventArgs)e).Parameter,
            };

            var conversation = new Conversation
            {
                BindingContext = user
            };
            await Navigation.PushAsync(conversation, true);
        }

        private void fetchContacts()
        {

            string id = (string)Application.Current.Properties["id"];
            var contactList = GloblalData.contactList.Where(x => x.contactID[0] == id).FirstOrDefault();

            if (contactList == null)
            {
                ContactListGrid.IsVisible = false;
                return;

            }

            ObservableCollection<UserModel> userContacts = new ObservableCollection<UserModel>();
            for (int i = 1; i < contactList.contactID.Count(); i++)
            {
                userContacts.Add(new UserModel() { uid = contactList.contactID[i], username = contactList.contactName[i], email = contactList.contactEmail[i] });
            }

            userListView.ItemsSource = userContacts;
        }
    }
}