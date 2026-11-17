using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrijavaIspita.Migrations
{
    /// <inheritdoc />
    public partial class DodajIspitTermin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Smer",
                columns: table => new
                {
                    SmerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smer", x => x.SmerId);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    KorisnikID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GodinaUpisa = table.Column<short>(type: "smallint", nullable: false),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Skolarina = table.Column<double>(type: "float", nullable: false),
                    Prosek = table.Column<double>(type: "float", nullable: true),
                    SmerId = table.Column<int>(type: "int", nullable: false),
                    Budzet = table.Column<double>(type: "float", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.KorisnikID);
                    table.ForeignKey(
                        name: "FK_Korisnici_Smer_SmerId",
                        column: x => x.SmerId,
                        principalTable: "Smer",
                        principalColumn: "SmerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Budzet",
                columns: table => new
                {
                    BudzetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    Iznos = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Na cekanju")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budzet", x => x.BudzetID);
                    table.ForeignKey(
                        name: "FK__Budzet__KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                });

            migrationBuilder.CreateTable(
                name: "Obavestenja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obavestenja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obavestenja_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                });

            migrationBuilder.CreateTable(
                name: "Predmet",
                columns: table => new
                {
                    PredmetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfesorID = table.Column<int>(type: "int", nullable: true),
                    SmerId = table.Column<int>(type: "int", nullable: false),
                    ESPB = table.Column<int>(type: "int", nullable: true),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predmet", x => x.PredmetId);
                    table.ForeignKey(
                        name: "FK_Predmet_Korisnici_ProfesorID",
                        column: x => x.ProfesorID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                    table.ForeignKey(
                        name: "FK_Predmet_Smer_SmerId",
                        column: x => x.SmerId,
                        principalTable: "Smer",
                        principalColumn: "SmerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transakcija",
                columns: table => new
                {
                    TransakcijaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Iznos = table.Column<double>(type: "float", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transakcija", x => x.TransakcijaID);
                    table.ForeignKey(
                        name: "FK_Transakcija_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRole_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "ZahtevBudzet",
                columns: table => new
                {
                    ZahtevBudzetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    Kolicina = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZahtevBudzet", x => x.ZahtevBudzetId);
                    table.ForeignKey(
                        name: "FK_ZahtevBudzet_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IspitTermini",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PredmetId = table.Column<int>(type: "int", nullable: false),
                    Rok = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IspitTermini", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IspitTermini_Predmet_PredmetId",
                        column: x => x.PredmetId,
                        principalTable: "Predmet",
                        principalColumn: "PredmetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prijava",
                columns: table => new
                {
                    PredmetID = table.Column<int>(type: "int", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    Ocena = table.Column<int>(type: "int", nullable: false),
                    Rok = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumPrijave = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prijava", x => new { x.PredmetID, x.KorisnikID });
                    table.ForeignKey(
                        name: "FK_Prijava_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikID");
                    table.ForeignKey(
                        name: "FK_Prijava_Predmet_PredmetID",
                        column: x => x.PredmetID,
                        principalTable: "Predmet",
                        principalColumn: "PredmetId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budzet_KorisnikID",
                table: "Budzet",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_IspitTermini_PredmetId",
                table: "IspitTermini",
                column: "PredmetId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_SmerId",
                table: "Korisnici",
                column: "SmerId");

            migrationBuilder.CreateIndex(
                name: "IX_Obavestenja_KorisnikID",
                table: "Obavestenja",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Predmet_ProfesorID",
                table: "Predmet",
                column: "ProfesorID");

            migrationBuilder.CreateIndex(
                name: "IX_Predmet_SmerId",
                table: "Predmet",
                column: "SmerId");

            migrationBuilder.CreateIndex(
                name: "IX_Prijava_KorisnikID",
                table: "Prijava",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Transakcija_KorisnikID",
                table: "Transakcija",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_KorisnikID",
                table: "UserRole",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleID",
                table: "UserRole",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_ZahtevBudzet_KorisnikId",
                table: "ZahtevBudzet",
                column: "KorisnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budzet");

            migrationBuilder.DropTable(
                name: "IspitTermini");

            migrationBuilder.DropTable(
                name: "Obavestenja");

            migrationBuilder.DropTable(
                name: "Prijava");

            migrationBuilder.DropTable(
                name: "Transakcija");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "ZahtevBudzet");

            migrationBuilder.DropTable(
                name: "Predmet");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "Smer");
        }
    }
}
