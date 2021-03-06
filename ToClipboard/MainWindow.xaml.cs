﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using ToClipboard.Misc;
using ToClipboard.Model;
using System.Linq;

namespace ToClipboard
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        bool changed = false;

        public List<IItem> Items { get; set; }
        protected IData DB { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Title = App.TITLE + " v1.0.14";
            App.CURRENT.WindowPlace.Register(this); // save & restore window size and location

            DB = new Data.DataSQLite(true);
            DB.Bind_JumpLists_ItemsSource(cbJumpList);
            RefreshItems();
        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            DB.AddNew_Item(listView, DB.SelectedJumpList.JumpListId);
        }

        void RefreshItems()
        {
            DB.Bind_Items_ItemsSource(listView, jumpListId: DB.SelectedJumpList.JumpListId);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            CreateJumpList(DB);
            //DB.SaveChanges(); // this will save changes that don't show as needing saving
            DB.Dispose();
        }

        /// <summary>
        /// Create a JumpList, and add the items for the selected JumpList
        /// </summary>
        /// <param name="db"></param>
        public static void CreateJumpList(IData db = null)
        {
            if (null == db)
            {
                db = new Data.DataSQLite();
                db.SelectedJumpList = db.First_JumpList();
            }

            var jl = new JumpList { ShowFrequentCategory = false, ShowRecentCategory = false };

            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string category = db.SelectedJumpList.Name + " Jump List";

            // Apply sort
            IItem[] items = db.GetItems(db.SelectedJumpList.JumpListId);
            switch (db.SelectedJumpList.SortBy)
            {
                case SortType.Ascending:
                    items = items.OrderBy(i => i.Title).ToArray();
                    break;
                case SortType.Descending:
                    items = items.OrderByDescending(i => i.Title).ToArray();
                    break;
                case SortType.Percentage: // not implemented yet
                case SortType.LastUsed:
                    items = items.OrderByDescending(i => i.DateLastUsed).ToArray();
                    break;
                case SortType.MostUsed:
                    items = items.OrderByDescending(i => i.CountUsed)
                        .ThenByDescending(i => i.DateLastUsed)
                        .ToArray();
                    break;
                case SortType.OrderAdded: // no change
                default:
                    break;
            }

            foreach (IItem item in items)
            {
                string iconfile = item.LaunchApp;
                //
                // If the LaunchApp is an image file, use it for the Jump List Item Icon
                //
                if (App.Try_AppAndIcon_IsImage(iconfile, tempIconLocation =>
                {
                    //tempIconLocation.DeleteIfExists();

                    // If icon doesn't exist, create it
                    if (!tempIconLocation.Exists)
                        IconHelper.ImageToIcon(iconfile, tempIconLocation.FullName, 64);
                    if (tempIconLocation.Exists(true))
                        iconfile = tempIconLocation.FullName;
                })) { }

                // If is a steam app
                else if (App.Try_AppAndIcon_IsSteam(item, steamApp => iconfile = steamApp.FullName))
                { }

                /*
                 * If not an image, then see if LaunchApp is a website and download
                 * the website's icon
                 */
                else if (App.Try_AppAndIcon_IsHttp(iconfile, tempIconLocation =>
                {
                    //tempIconLocation.DeleteIfExists();
                    if (!tempIconLocation.Exists)
                    {
                        var original = tempIconLocation.AppendName("_original");
                        //original.DeleteIfExists();

                        // Download Icon
                        if (IconHelper.HttpToIcon(iconfile, original.FullName))
                        {
                            // Downloaded Icon may not actually be an icon file
                            IconHelper.ImageToIcon(original.FullName, tempIconLocation.FullName);
                            original.DeleteIfExists();
                        }
                    }
                    if (tempIconLocation.Exists(true))
                        iconfile = tempIconLocation.FullName;
                })) { }

                var task = new JumpTask
                {
                    ApplicationPath = appPath,
                    CustomCategory = category,
                    Arguments = item.ItemId.ToString(),
                    Title = item.Title,
                    IconResourcePath = iconfile
                };
                jl.JumpItems.Add(task);
            }
            JumpList.SetJumpList(Application.Current, jl);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (changed || DB.HasChanges)
            { // changed can be false, when it should be true, and DB.HasChanges is never true
                // if don't tab out of a text box (ie it loses focus, changed won't be true, a call to
                // DB.SaveChanges() won't catch it, until the app is closed. Not sure if its me clicking
                // close X in window corner, or what.

                // This popup shows in middle of application, but not necessarily on muli-monitor setups
                if (System.Windows.Forms.DialogResult.Yes == MessageBoxEx.Show(this,
                    "There are unsaved changes. Save?", "To Clipboard", System.Windows.Forms.MessageBoxButtons.YesNo))
                {
                    DB.SaveChanges();
                }
            }
            base.OnClosing(e);
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            DB.SaveChanges();
            changed = false;
        }

        private void LaunchApp_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            changed = true;
        }

        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            changed = true;
        }

        private void LaunchApp_Changed(object sender, TextChangedEventArgs e)
        {
            changed = true;

            TextBox textBox = (TextBox)sender;
            IItem item = (IItem)textBox.DataContext;

            // If (old) item.LaunchApp is an image, delete its Icon if it exists
            App.Try_AppAndIcon_IsImage(item.LaunchApp, tempIconLocation => tempIconLocation.DeleteIfExists());
            App.Try_AppAndIcon_IsHttp(item.LaunchApp, tempIconLocation => tempIconLocation.DeleteIfExists());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            IItem item = (IItem)button.DataContext;
            DB.Delete_Item(item);
            RefreshItems();
            changed = true;
        }

        protected bool SteamFolderExists { get { return App.SteamAppsDirectory.Exists; } }

        private void SteamLocation_Click(object sender, RoutedEventArgs e)
        {
            App.SteamAppsDirectory.OpenLocation();
        }

        private void DataDirectory_Click(object sender, RoutedEventArgs e)
        {
            App.UserDataDirectory.OpenLocation();
        }

        private void TempDirectory_Click(object sender, RoutedEventArgs e)
        {
            App.TempDirectory.OpenLocation();
        }

        protected SortType JumpListSort { get { return DB.SelectedJumpList.SortBy; } }

        private void Sort_Clicked(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            string name = (string)menu.Header;
            SortType sort = name.ToEnum<SortType>();
            DB.SelectedJumpList.SortBy = sort;
            changed = true;
        }
    }
}
