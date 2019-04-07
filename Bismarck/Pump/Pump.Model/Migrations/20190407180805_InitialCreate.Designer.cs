﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pump.Model;

namespace Pump.Model.Migrations
{
    [DbContext(typeof(PumpDbContext))]
    [Migration("20190407180805_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Pump")
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Pump.Model.Entity.CalculationRequestInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HasError");

                    b.Property<string>("Request");

                    b.Property<string>("Response");

                    b.HasKey("Id");

                    b.ToTable("CalculationRequestInfo");
                });

            modelBuilder.Entity("Pump.Model.Entity.PumpCalculation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CalcRequestInfoId");

                    b.Property<int>("CalcState");

                    b.Property<Guid?>("CalcUuid")
                        .IsRequired();

                    b.Property<int>("ClientId");

                    b.Property<decimal?>("Pressure")
                        .IsRequired();

                    b.Property<decimal?>("Temperature")
                        .IsRequired();

                    b.Property<decimal?>("WallTemperature")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CalcRequestInfoId");

                    b.ToTable("PumpCalculation");
                });

            modelBuilder.Entity("Pump.Model.Entity.PumpCalculation", b =>
                {
                    b.HasOne("Pump.Model.Entity.CalculationRequestInfo", "CalcRequestInfo")
                        .WithMany()
                        .HasForeignKey("CalcRequestInfoId");
                });
#pragma warning restore 612, 618
        }
    }
}
