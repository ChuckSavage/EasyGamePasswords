using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ToClipboard.Model
{
    public interface IData : IDisposable, INotifyPropertyChanged
    {
        event Events.EntityChangen EntityChanging;
        event Events.EntityChangen EntityChanged;
        //new event PropertyChangedEventHandler PropertyChanged;

        void Bind_Items_ItemsSource(ItemsControl control, long? jumpListId = null, long? categoryId = null);
        IItem AddNew_Item(ItemsControl control, long? jumpListId = null, long? categoryId = null);
        void Delete_Item(IItem item);
        void Item_Clicked(IItem item);
        IItem Item_Clicked(long itemId);

        void Bind_JumpLists_ItemsSource(ItemsControl control);
        IJumpList First_JumpList();

        IJumpList SelectedJumpList { get; set; } // TwoWay Binding

        bool HasChanges { get; }
        void SaveChanges();
        long? JumpListId { get; }
        long? CategoryId { get; }

        IItem[] GetItems(long? jumpListId = null, long? categoryId = null);
    }
}
