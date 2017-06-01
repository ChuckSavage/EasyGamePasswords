using ToClipboard.Data.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ToClipboard.Data
{
    class SQLiteDbContext : DbContext
    {
        //public static readonly StateListener _stateListener = new StateListener();
        //private static readonly IServiceProvider _serviceProvider
        //    = new ServiceCollection()
        //        .AddEntityFrameworkSqlite() // set this to your DB type
        //        .AddSingleton<IEntityStateListener>(_stateListener)
        //        .BuildServiceProvider();

        public SQLiteDbContext() : base() { }

        readonly string db_path;

        public virtual DbSet<JumpList> JumpLists { get; set; }
        public virtual DbSet<Category> Groups { get; set; }
        public virtual DbSet<Item> Items { get; set; }

        public SQLiteDbContext(string database) : base()
        {
            db_path = database;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                //.UseInternalServiceProvider(_serviceProvider)
                .UseSqlite(string.Format("Data Source={0}", db_path));
        }
    }

    // This will be obsolete in a future version of EF Core
    // Modified from: https://github.com/aspnet/EntityFramework/issues/5970
    /*
     * Not working as I'd hoped. I was hoping to get new value / old values, instead it's doing something else
     */
    //public class StateListener : IEntityStateListener
    //{
    //    public delegate void StateChangen(InternalEntityEntry entry, EntityState state, bool? fromQuery);
    //    public event StateChangen Changing;
    //    public event StateChangen Changed;

    //    public void StateChanging(InternalEntityEntry entry, EntityState newState)
    //    {
    //        Changing?.Invoke(entry, newState, null);
    //    }

    //    public void StateChanged(InternalEntityEntry entry, EntityState oldState, bool fromQuery)
    //    {
    //        Changed?.Invoke(entry, oldState, fromQuery);
    //    }
    //}
}
