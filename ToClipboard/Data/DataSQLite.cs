using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ToClipboard.Data.Tables;
using ToClipboard.Model;

namespace ToClipboard.Data
{
    public class DataSQLite : IData
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Events.EntityChangen EntityChanging;
        public event Events.EntityChangen EntityChanged;

        public const string DATABASE = "data.sqlite";
        readonly SQLiteDbContext db;

        public DataSQLite(bool seedData = false)
        {
            FileInfo database = new FileInfo(Path.Combine(App.UserDataDirectory.FullName, DATABASE));
            if (!database.Directory.Exists)
                Directory.CreateDirectory(database.DirectoryName);

            //if (database.Exists)
            //    database.Delete();

            db = new SQLiteDbContext(database.FullName);
            //SQLiteDbContext._stateListener.Changing += _stateListener_Changing;
            //SQLiteDbContext._stateListener.Changed += _stateListener_Changed;

            // If a change is about to be made to the structure of the database...
            if (database.Exists && db.Database.GetPendingMigrations().Any())
            {
                // Make backup of database
                database.CopyToUnique(App.TempDirectory);
            }
            db.Database.Migrate(); // Ensure database is up to date with all changes to tables applied
                                   // Microsoft.EntityFrameworkCore

            if (!db.JumpLists.Any())
            {
                JumpList d;
                db.JumpLists.Add(d = new JumpList { Name = "Default" });

                if (seedData)
                {
                    AddNew_Item(null, d.JumpListId);
                    AddNew_Item(null, d.JumpListId);
                    AddNew_Item(null, d.JumpListId);
                }
                db.SaveChanges();
            }
        }

        // will break on change to EF Core
        //private void _stateListener_Changing(Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry entry, EntityState state, bool? fromQuery)
        //{
        //    EntityChanging?.Invoke(entry.EntityType.Name, entry.EntityState, state);
        //}

        //// will break on change to EF Core
        //private void _stateListener_Changed(Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry entry, EntityState state, bool? fromQuery)
        //{
        //    EntityChanged?.Invoke(entry.EntityType.Name, entry.EntityState, state);
        //}

        #region Properties
        public long? CategoryId { get { return _CategoryId; } }
        long? _CategoryId;

        public bool HasChanges { get { return db.ChangeTracker.HasChanges(); } }

        public long? JumpListId { get { return _JumpListId; } }
        long? _JumpListId;

        public IJumpList SelectedJumpList
        {
            get { return _SelectedJumpList; }
            set
            {
                if (null != value)
                    Bind_Items_ItemsSource(null, value.JumpListId);
                //db.Items.Where(i => i.JumpListId == value.JumpListId).ToList();
                _SelectedJumpList = value;
                NotifyPropertyChanged();
            }
        }
        IJumpList _SelectedJumpList;


        #endregion

        #region Methods
        public IItem AddNew_Item(ItemsControl control, long? jumpListId = null, long? categoryId = null)
        {
            int count = GetLocalItems(jumpListId, categoryId).Length + 1;
            var item = new Item { Title = "{0} title".F(count), Text = "{0} text".F(count), CategoryId = categoryId, JumpListId = jumpListId };
            if (null != jumpListId || null != categoryId)
            {
                db.Items.Add(item);
                if (null != control)
                    Bind_Items_ItemsSource(control, jumpListId, categoryId);
            }
            //if (null != itemscontrol_tag)
            //{
            //    var local = (Microsoft.EntityFrameworkCore.ChangeTracking.LocalView<Item>)itemscontrol_tag;
            //    local.Add(item);
            //}
            return item;
        }

        public void Bind_Items_ItemsSource(ItemsControl control, long? jumpListId = null, long? categoryId = null)
        {
            if (null == control)
                control = _items_ItemsControl;
            else
                _items_ItemsControl = control;
            if (null == control) return;

            _JumpListId = jumpListId;
            _CategoryId = categoryId;

            control.ItemsSource = null;
            //control.DataContext = this;

            // Search database for these values & add them to Local
            GetItems(jumpListId, categoryId);

            // Have to filter Local, since it will contain all the searches to date
            control.ItemsSource = db.Items.Local
                .Where(i => i.JumpListId == jumpListId && i.CategoryId == categoryId)
                .ToList();
            control.Tag = db.Items.Local;
        }
        ItemsControl _items_ItemsControl;

        public void Bind_JumpLists_ItemsSource(ItemsControl control)
        {
            control.ItemsSource = null;
            control.DataContext = this; // needed to get SelectedItem in control PropertyChanged event working - see https://stackoverflow.com/a/44282261/353147

            db.JumpLists.ToList();
            control.ItemsSource = db.JumpLists.Local;
            control.Tag = db.JumpLists.Local;

            SelectedJumpList = db.JumpLists.FirstOrDefault();
        }

        public void Delete_Item(IItem IN)
        {
            Item item = (Item)IN;
            db.Items.Local.Remove(item);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IJumpList First_JumpList()
        {
            return db.JumpLists.FirstOrDefault();
        }

        /// <summary>
        /// Returns saved and unsaved items
        /// </summary>
        /// <param name="jumpListId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IItem[] GetItems(long? jumpListId = null, long? categoryId = null)
        {
            if (null != jumpListId || null != categoryId)
                return db.Items.Where(i => i.JumpListId == jumpListId && i.CategoryId == categoryId).ToArray();
            return db.Items.ToArray();
        }

        public IItem[] GetLocalItems(long? jumpListId = null, long? categoryId = null)
        {
            if (null != jumpListId || null != categoryId)
                return db.Items.Local.Where(i => i.JumpListId == jumpListId && i.CategoryId == categoryId).ToArray();
            return db.Items.Local.ToArray();
        }

        public void Item_Clicked(IItem item)
        {
            Item i = (Item)item;
            i.DateLastUsed = DateTime.Now;
            i.CountUsed++;
        }

        public IItem Item_Clicked(long itemId)
        {
            Item item = db.Items.Find(itemId);
            if (null != item)
                Item_Clicked(item);
            return item;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
        #endregion
    }
}
