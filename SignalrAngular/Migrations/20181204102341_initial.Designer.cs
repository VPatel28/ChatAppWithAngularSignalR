﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SignalrAngular.Models;

namespace SignalrAngular.Migrations
{
    [DbContext(typeof(Signalr))]
    [Migration("20181204102341_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SignalRWithAngular.SignalrAngular.ChatHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChatData");

                    b.Property<string>("GrpName");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ChatHistory");
                });

            modelBuilder.Entity("SignalRWithAngular.SignalrAngular.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConnectionID");

                    b.Property<int>("IsLoggedIn");

                    b.Property<string>("NickName");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("ChatMessage");
                });
#pragma warning restore 612, 618
        }
    }
}