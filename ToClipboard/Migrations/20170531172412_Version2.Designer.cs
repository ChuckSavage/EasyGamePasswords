using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ToClipboard.Data;

namespace ToClipboard.Migrations
{
    [DbContext(typeof(SQLiteDbContext))]
    [Migration("20170531172412_Version2")]
    partial class Version2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("ToClipboard.Data.Tables.Category", b =>
                {
                    b.Property<long>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("JumpListId");

                    b.Property<string>("Name");

                    b.HasKey("CategoryId");

                    b.HasIndex("JumpListId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("ToClipboard.Data.Tables.Item", b =>
                {
                    b.Property<long>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CategoryId");

                    b.Property<long>("CountUsed");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastUsed");

                    b.Property<bool>("DoLaunchApp");

                    b.Property<long?>("JumpListId");

                    b.Property<string>("LaunchApp");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("ItemId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("JumpListId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ToClipboard.Data.Tables.JumpList", b =>
                {
                    b.Property<long>("JumpListId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("JumpListId");

                    b.ToTable("JumpLists");
                });

            modelBuilder.Entity("ToClipboard.Data.Tables.Category", b =>
                {
                    b.HasOne("ToClipboard.Data.Tables.JumpList")
                        .WithMany("Groups")
                        .HasForeignKey("JumpListId");
                });

            modelBuilder.Entity("ToClipboard.Data.Tables.Item", b =>
                {
                    b.HasOne("ToClipboard.Data.Tables.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId");

                    b.HasOne("ToClipboard.Data.Tables.JumpList", "JumpList")
                        .WithMany("Items")
                        .HasForeignKey("JumpListId");
                });
        }
    }
}
