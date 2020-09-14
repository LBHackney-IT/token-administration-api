﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Infrastructure.Migrations
{
    [DbContext(typeof(TokenDatabaseContext))]
    [Migration("20200907091805_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TokenAdministrationApi.V1.Infrastructure.ApiEndpointNameLookup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApiEndpointName")
                        .IsRequired()
                        .HasColumnName("endpoint_name")
                        .HasColumnType("text");

                    b.Property<int>("ApiLookupId")
                        .HasColumnName("api_lookup_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("api_endpoint_lookup");
                });

            modelBuilder.Entity("TokenAdministrationApi.V1.Infrastructure.ApiNameLookup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApiName")
                        .IsRequired()
                        .HasColumnName("api_name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("api_lookup");
                });

            modelBuilder.Entity("TokenAdministrationApi.V1.Infrastructure.AuthTokens", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ApiEndpointNameLookupId")
                        .HasColumnName("api_endpoint_lookup_id")
                        .HasColumnType("integer");

                    b.Property<int>("ApiLookupId")
                        .HasColumnName("api_lookup_id")
                        .HasColumnType("integer");

                    b.Property<string>("AuthorizedBy")
                        .IsRequired()
                        .HasColumnName("authorized_by")
                        .HasColumnType("text");

                    b.Property<string>("ConsumerName")
                        .IsRequired()
                        .HasColumnName("consumer_name")
                        .HasColumnType("text");

                    b.Property<int>("ConsumerTypeLookupId")
                        .HasColumnName("consumer_type_lookup")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnName("date_created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Enabled")
                        .HasColumnName("enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Environment")
                        .IsRequired()
                        .HasColumnName("environment")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnName("expiration_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("HttpMethodType")
                        .IsRequired()
                        .HasColumnName("http_method_type")
                        .HasColumnType("character varying(6)")
                        .HasMaxLength(6);

                    b.Property<string>("RequestedBy")
                        .IsRequired()
                        .HasColumnName("requested_by")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("tokens");
                });

            modelBuilder.Entity("TokenAdministrationApi.V1.Infrastructure.ConsumerTypeLookup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnName("consumer_name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("consumer_type_lookup");
                });
#pragma warning restore 612, 618
        }
    }
}
