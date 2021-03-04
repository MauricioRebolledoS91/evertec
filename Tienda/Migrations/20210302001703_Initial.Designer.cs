﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tienda;

namespace Tienda.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210302001703_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0-rc.1.20451.13");

            modelBuilder.Entity("Tienda.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("Created_at");

                    b.Property<string>("CustomerDocument")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Customer_Document");

                    b.Property<string>("CustomerEmail")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("Customer_email");

                    b.Property<string>("CustomerMobile")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("Customer_mobile");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("Customer_name");

                    b.Property<string>("Status")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("Updated_at");

                    b.Property<decimal>("ValorOrder")
                        .HasColumnType("money")
                        .HasColumnName("ValorOrder");

                    b.HasKey("Id");

                    b.ToTable("Orders", "dbo");
                });

            modelBuilder.Entity("Tienda.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OrderDetailsId")
                        .UseIdentityColumn();

                    b.Property<int>("Cantidad")
                        .HasColumnType("int")
                        .HasColumnName("Cantidad");

                    b.Property<string>("CodigoProducto")
                        .HasColumnType("varchar(10)")
                        .HasColumnName("CodigoProducto");

                    b.Property<string>("NombreProducto")
                        .HasColumnType("varchar(200)")
                        .HasColumnName("NombreProducto");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("OrderId");

                    b.Property<decimal>("Total")
                        .HasColumnType("money")
                        .HasColumnName("Total");

                    b.Property<decimal>("Valor")
                        .HasColumnType("money")
                        .HasColumnName("Valor");

                    b.HasKey("OrderDetailsId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails", "dbo");
                });

            modelBuilder.Entity("Tienda.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PaymentId")
                        .UseIdentityColumn();

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2")
                        .HasColumnName("Fecha");

                    b.Property<DateTime>("FechaUpdate")
                        .HasColumnType("datetime2")
                        .HasColumnName("FechaUpdate");

                    b.Property<string>("Message")
                        .HasColumnType("varchar(max)")
                        .HasColumnName("Message");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("OrderId");

                    b.Property<string>("Reason")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Reason");

                    b.Property<int>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("RequestId");

                    b.Property<string>("Status")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Status");

                    b.Property<string>("UrlPago")
                        .HasColumnType("varchar(max)")
                        .HasColumnName("UrlPago");

                    b.HasKey("PaymentId");

                    b.HasIndex("OrderId");

                    b.ToTable("Payment", "dbo");
                });

            modelBuilder.Entity("Tienda.Models.OrderDetail", b =>
                {
                    b.HasOne("Tienda.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Tienda.Models.Payment", b =>
                {
                    b.HasOne("Tienda.Models.Order", "Order")
                        .WithMany("Payments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Tienda.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
