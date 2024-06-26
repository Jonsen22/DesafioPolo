﻿// <auto-generated />
using System;
using DesafioPolo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DesafioPolo.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DesafioPolo.Model.IndicadorModelDB", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("BaseCalculo")
                        .HasColumnType("int");

                    b.Property<DateOnly>("Data")
                        .HasColumnType("date");

                    b.Property<string>("DataReferencia")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("DesvioPadrao")
                        .HasColumnType("float");

                    b.Property<string>("Indicador")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Maximo")
                        .HasColumnType("float");

                    b.Property<double>("Media")
                        .HasColumnType("float");

                    b.Property<double>("Mediana")
                        .HasColumnType("float");

                    b.Property<double>("Minimo")
                        .HasColumnType("float");

                    b.Property<int?>("NumeroRespondentes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Indicadores");
                });
#pragma warning restore 612, 618
        }
    }
}
