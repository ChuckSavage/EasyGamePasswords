using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using ToClipboard.Misc;
using ToClipboard.Model;

namespace ToClipboard
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        bool changed = false;
        bool loading = true;

        public List<IItem> Items { get; set; }

        protected IData DB { get; private set; }
        public MainWindow()
        {
            InitializeComponent();

            Title = App.TITLE + " v1.0.3";

            DB = new Data.DataSQLite(true);
            //DB.EntityChanged += (e, a, b) => //changed = true;
            //{
            //    changed = true;
            //};
            DB.Bind_JumpLists_ItemsSource(cbJumpList);
            //IJumpList jumpList = DB.SelectedJumpList;//.First_JumpList();
            //cbJumpList.SelectedItem = jumpList = db.First_JumpList();
            //cbJumpList.Text = jumpList.Name;
            RefreshItems();

            //this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DB.SelectedJumpList = DB.First_JumpList();
            loading = false;
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var listView = sender as ListView;
            ////IItem item = listView.SelectedItems[0] as IItem;
            //IItem item = listView.SelectedItem as IItem;
            //Clipboard.SetText(item.Text);
            ////var a = listView;

            //DB.Item_Clicked(item);
            //changed = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            /*
             * THE REASON FOR THE PROGRAM
             * 
             * Create a JumpList, and add the items for the selected JumpList
             */

            var jl = new JumpList { ShowFrequentCategory = false, ShowRecentCategory = false };

            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string category = DB.SelectedJumpList.Name + " Jump List";

            //App.TempDirectory.Open();

            foreach (IItem item in DB.GetItems(DB.SelectedJumpList.JumpListId))
            {
                string iconfile = item.LaunchApp;
                //
                // If the LaunchApp is an image file, use it for the Jump List Item Icon
                //
                App.AppAndIcon_IsImage(iconfile, tempIconLocation =>
                {
                    //if (tempIconLocation.Exists)
                    //    tempIconLocation.Delete();

                    // If icon doesn't exist, create it
                    if (!tempIconLocation.Exists)
                        IconHelper.ConvertToIcon(iconfile, tempIconLocation.FullName, 64);
                    iconfile = tempIconLocation.FullName;
                });

                var task = new JumpTask
                {
                    ApplicationPath = appPath,
                    CustomCategory = category,
                    Arguments = string.Format("{0} {1}", item.ItemId, item.Text),
                    Title = item.Title,
                    IconResourcePath = iconfile
                };
                jl.JumpItems.Add(task);
            }
            JumpList.SetJumpList(Application.Current, jl);
            //DB.SaveChanges(); // this will save changes that don't show as needing saving
            DB.Dispose();
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

        //private void cbJumpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (loading) return;
        //    IJumpList jumpList = cbJumpList.SelectedItem as IJumpList;
        //    long id = (long)cbJumpList.SelectedValue;
        //    if (jumpList.JumpListId != DB.JumpListId)
        //        Bind_ListView_Items(jumpList?.JumpListId ?? 1);
        //}

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
            App.AppAndIcon_IsImage(item.LaunchApp, tempIconLocation =>
            {
                if (tempIconLocation.Exists)
                    tempIconLocation.Delete();
            });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            IItem item = (IItem)button.DataContext;
            DB.Delete_Item(item);
            RefreshItems();
            changed = true;
        }
    }
}
