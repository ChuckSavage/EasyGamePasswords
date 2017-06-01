using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToClipboard.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Format some text, shortcut for string.Format(text, args)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string F(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static ObservableCollection<TEntity> ToObservableCollection<TEntity>(this DbSet<TEntity> set, DbContext context)
            where TEntity : class
        {
            //var context = set.GetService<DbContext>();
            var data = context.ChangeTracker.Entries<TEntity>().Select(e => e.Entity);
            var collection = new ObservableCollection<TEntity>(data);

            collection.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    context.AddRange(e.NewItems.Cast<TEntity>());
                }

                if (e.OldItems != null)
                {
                    context.RemoveRange(e.OldItems.Cast<TEntity>());
                }
            };

            return collection;
        }

        //public static DbContext ToContext<TEntity>(this DbSet<TEntity> set)
        //{
        //    var context = ((IAccessor<IServiceProvider>)set).Service.GetService<DbContext>();
        //    return context;
        //}
    }
}
