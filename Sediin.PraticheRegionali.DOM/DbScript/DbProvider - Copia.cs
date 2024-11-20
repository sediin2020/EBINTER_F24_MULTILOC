using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SqlServer.Management.Smo;
using Sediin.PraticheRegionali.DOM.Data;
using Sediin.PraticheRegionali.DOM.Entitys;

namespace EBLIG.DOM.DbScript
{
    public delegate void ReportProgressBackupDb(int percentuale, string id);

    public class _DbProvider : IDisposable
    {
        #region MyRegion

        public string Id { get; set; }

        public string BackupFilename { get; set; }

        public event ReportProgressBackupDb OnReportProgressBackupDb;
        public async Task BackupDatabase(string path)
        {

            SediinPraticheRegionaliDbContext context = new SediinPraticheRegionaliDbContext();

            var dbname = context.Database.Connection.Database.ToString();

            // Connect to the local, default instance of SQL Server.   
            Server srv = new Server();

            var s = srv.Databases.Count;
            // Reference the EBLACc database.   
            Microsoft.SqlServer.Management.Smo.Database db = default;// new Microsoft.SqlServer.Management.Smo.Database(srv, "EBLACc");
            db = srv.Databases[dbname];

            // Store the current recovery model in a variable.   
            int recoverymod;
            recoverymod = (int)db.DatabaseOptions.RecoveryModel;

            // Define a Backup object variable.   
            Backup bk = new Backup();

            // Specify the type of backup, the description, the name, and the database to be backed up.   
            bk.Action = BackupActionType.Database;
            bk.BackupSetDescription = "Full backup";
            bk.BackupSetName = "Backup";
            bk.Database = dbname;

            // Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
            BackupDeviceItem bdi = default(BackupDeviceItem);
            bdi = new BackupDeviceItem("Test_Full_Backup1", DeviceType.File);

            // Add the device to the Backup object.   
            bk.Devices.Add(bdi);
            // Set the Incremental property to False to specify that this is a full database backup.   
            bk.Incremental = false;

            // Set the expiration date.   
            //System.DateTime backupdate = new System.DateTime();
            //backupdate = new System.DateTime(2006, 10, 5);
            //bk.ExpirationDate = backupdate;

            // Specify that the log must be truncated after the backup is complete.   
            bk.LogTruncation = BackupTruncateLogType.Truncate;

            // Run SqlBackup to perform the full database backup on the instance of SQL Server.   
            bk.SqlBackup(srv);

            // Inform the user that the backup has been completed.   
            System.Console.WriteLine("Full Backup complete.");

            // Remove the backup device from the Backup object.   
            bk.Devices.Remove(bdi);



        }
        //public async Task BackupDatabase(string path)
        //{

        //    Id = Guid.NewGuid().ToString();

        //    var _database = "";
        //    var name = "";
        //    var backupName = "";
        //    var backupNameZIp = "";

        //    SediinPraticheRegionaliDbContext context = new SediinPraticheRegionaliDbContext();

        //    var _constr = context.Database.Connection.ConnectionString.ToString();

        //    var con = new SqlConnection(_constr);

        //    //  var con = new SqlConnection(_constr);

        //    _database = con.Database;
        //    name = Path.Combine(path, con.Database + "_" + DateTime.Now.Ticks);
        //    backupName = name + ".bak";
        //    backupNameZIp = name + ".zip";

        //    BackupFilename = backupName;

        //    var cmd = new SqlCommand(string.Format(
        //        "backup database {0} to disk = {1} with description = {2}, name = {3}, stats = 1",
        //        QuoteIdentifier(_database),
        //        QuoteString(backupName),
        //        QuoteString(DateTime.Now.ToString()),
        //        QuoteString(backupName)), con);
        //    {
        //        //The time in seconds to wait for the command to execute. The default is 30 seconds.
        //        cmd.CommandTimeout = 500;
        //        con.FireInfoMessageEventOnUserErrors = true;
        //        con.InfoMessage += OnInfoMessage;
        //        con.Open();
        //        cmd.ExecuteNonQuery();

        //        //con.Close();
        //        con.InfoMessage -= OnInfoMessage;
        //        con.FireInfoMessageEventOnUserErrors = false;

        //        cmd.Dispose();
        //    }

        //    con.Close();

        //    using (var archive = ZipFile.Open(backupNameZIp, ZipArchiveMode.Create))
        //    {
        //        // we're getting a file stream
        //        // here but it could be any stream
        //        var file = File.OpenRead(backupName);
        //        var entry =
        //            archive.CreateEntry(
        //                Path.GetFileName(backupName),
        //                CompressionLevel.Optimal
        //            );

        //        await file.CopyToAsync(entry.Open());
        //        file.Dispose();

        //        try
        //        {
        //            File.Delete(BackupFilename);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            try
            {
                int.TryParse(e.Message.Split(' ')?.FirstOrDefault(), out int _percentuale);

                if (_percentuale == 0)
                {
                    _percentuale = 100;
                }
                OnReportProgressBackupDb?.Invoke(_percentuale, Id);
            }
            catch (Exception)
            {
            }

            //foreach (SqlError info in e.Errors)
            //{
            //    if (info.Class > 10)
            //    {
            //        // TODO: treat this as a genuine error
            //    }
            //    else
            //    {
            //        // TODO: treat this as a progress message
            //    }
            //}
        }

        private string QuoteIdentifier(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }

        private string QuoteString(string text)
        {
            return "'" + text.Replace("'", "''") + "'";
        }

        #endregion

        public void RestoreDatabase(string backup)
        {
            Server srv;
            srv = new Server();
            Console.WriteLine(srv.Databases.Count);

            //            string sql = "IF EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE name = 'test')";

            //DROP DATABASE test RESTORE DATABASE test FROM DISK = 'E:/test.bak'
        }

        public void SvuotaDatabase(List<string> table)
        {
            var _context = new SediinPraticheRegionaliDbContext();
            var _db = _context.Database.Connection.Database;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("use " + _db);
            try
            {
                if (table.FirstOrDefault(x => x == nameof(PraticheRegionaliImprese)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[PraticheRegionaliImpreseAllegati]");
                    sb.AppendLine("truncate table[dbo].[PraticheRegionaliImpreseDatiPratica]");
                    sb.AppendLine("truncate table[dbo].[PraticheRegionaliImpreseStatoPraticaStorico]");

                    sb.AppendLine("ALTER TABLE PraticheRegionaliImprese NOCHECK CONSTRAINT ALL");
                    sb.AppendLine("DELETE FROM[dbo].PraticheRegionaliImprese");
                    sb.AppendLine("DBCC CHECKIDENT('dbo.PraticheRegionaliImprese', RESEED, 0)");
                    sb.AppendLine("ALTER TABLE PraticheRegionaliImprese WITH CHECK CHECK CONSTRAINT ALL");
                }

                if (table.FirstOrDefault(x => x == nameof(Copertura)) != null)
                {
                    sb.AppendLine("truncate table [dbo].[Copertura]");
                }

                if (table.FirstOrDefault(x => x == nameof(Dipendente) || x == nameof(Azienda)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[DipendenteAzienda]");
                    sb.AppendLine("truncate table[dbo].[DelegheSportelloDipendente]");
                    sb.AppendLine("truncate table[dbo].[DelegheSportelloAzienda]");
                }

                if (table.FirstOrDefault(x => x == nameof(Dipendente) || x == nameof(Sportello)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[DelegheSportelloDipendente]");
                    sb.AppendLine("truncate table[dbo].[DelegheSportelloAzienda]");
                }

                if (table.FirstOrDefault(x => x == nameof(Azienda)) != null)
                {
                    sb.AppendLine("ALTER TABLE Azienda NOCHECK CONSTRAINT ALL");
                    sb.AppendLine("DELETE FROM[dbo].Azienda");
                    sb.AppendLine("DBCC CHECKIDENT('dbo.Azienda', RESEED, 0)");
                    sb.AppendLine("ALTER TABLE Azienda WITH CHECK CHECK CONSTRAINT ALL");
                }

                if (table.FirstOrDefault(x => x == nameof(Dipendente)) != null)
                {
                    sb.AppendLine("ALTER TABLE Dipendente NOCHECK CONSTRAINT ALL");
                    sb.AppendLine("DELETE FROM[dbo].Dipendente");
                    sb.AppendLine("DBCC CHECKIDENT('dbo.Dipendente', RESEED, 0)");
                    sb.AppendLine("ALTER TABLE Dipendente WITH CHECK CHECK CONSTRAINT ALL");
                }

                if (table.FirstOrDefault(x => x == nameof(Sportello)) != null)
                {
                    sb.AppendLine("ALTER TABLE[Sportello] NOCHECK CONSTRAINT ALL");
                    sb.AppendLine("DELETE FROM[dbo].[Sportello]");
                    sb.AppendLine("DBCC CHECKIDENT('dbo.Sportello',RESEED, 0)");
                    sb.AppendLine("ALTER TABLE[Sportello] WITH CHECK CHECK CONSTRAINT ALL");
                }

                if (table.FirstOrDefault(x => x == nameof(Liquidazione)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[LiquidazionePraticheRegionali]");

                    sb.AppendLine("ALTER TABLE Liquidazione NOCHECK CONSTRAINT ALL");
                    sb.AppendLine("DELETE FROM[dbo].Liquidazione");
                    sb.AppendLine("DBCC CHECKIDENT('dbo.Liquidazione', RESEED, 0)");
                    sb.AppendLine("ALTER TABLE Liquidazione WITH CHECK CHECK CONSTRAINT ALL");

                }

                if (table.FirstOrDefault(x => x == nameof(NavigatioHistory)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[NavigatioHistory]");
                }

                if (table.FirstOrDefault(x => x == nameof(Logs)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[Logs]");
                }

                if (table.FirstOrDefault(x => x == nameof(Uniemens)) != null)
                {
                    sb.AppendLine("truncate table[dbo].[Uniemens]");
                }

                if (table.FirstOrDefault(x => x == "Users") != null)
                {
                    sb.AppendLine("delete from[dbo].[AspNetUserRoles] where roleid != (SELECT TOP (1) AspNetUserRoles.RoleId");
                    sb.AppendLine("FROM AspNetUsers INNER JOIN");
                    sb.AppendLine("AspNetUserRoles ON AspNetUsers.Id = AspNetUserRoles.UserId INNER JOIN");
                    sb.AppendLine("AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id");
                    sb.AppendLine("WHERE  (AspNetUsers.UserName = N'Admin'))");
                    sb.AppendLine("delete from[dbo].[AspNetUsers] where UserName != 'Admin'");
                }

                SediinPraticheRegionaliDbContext context = new SediinPraticheRegionaliDbContext();

                var _constr = context.Database.Connection.ConnectionString.ToString();

                var con = new SqlConnection(_constr);

                var cmd = new SqlCommand(sb.ToString(), con);
                {
                    //The time in seconds to wait for the command to execute. The default is 30 seconds.
                    cmd.CommandTimeout = 500;
                    con.Open();
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                }

                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Main()
        {
            try
            {
                // Connect to the local, default instance of SQL Server.   
                Server srv = new Server();

                var s = srv.Databases.Count;
                // Reference the EBLACc database.   
                Microsoft.SqlServer.Management.Smo.Database db = default;// new Microsoft.SqlServer.Management.Smo.Database(srv, "EBLACc");
                db = srv.Databases["EBLACc"];

                // Store the current recovery model in a variable.   
                int recoverymod;
                recoverymod = (int)db.DatabaseOptions.RecoveryModel;

                // Define a Backup object variable.   
                Backup bk = new Backup();

                // Specify the type of backup, the description, the name, and the database to be backed up.   
                bk.Action = BackupActionType.Database;
                bk.BackupSetDescription = "Full backup of EBLACc";
                bk.BackupSetName = "EBLACc Backup";
                bk.Database = "EBLACc";

                // Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
                BackupDeviceItem bdi = default(BackupDeviceItem);
                bdi = new BackupDeviceItem("Test_Full_Backup1", DeviceType.File);

                // Add the device to the Backup object.   
                bk.Devices.Add(bdi);
                // Set the Incremental property to False to specify that this is a full database backup.   
                bk.Incremental = false;

                // Set the expiration date.   
                System.DateTime backupdate = new System.DateTime();
                backupdate = new System.DateTime(2006, 10, 5);
                bk.ExpirationDate = backupdate;

                // Specify that the log must be truncated after the backup is complete.   
                bk.LogTruncation = BackupTruncateLogType.Truncate;

                // Run SqlBackup to perform the full database backup on the instance of SQL Server.   
                bk.SqlBackup(srv);

                // Inform the user that the backup has been completed.   
                System.Console.WriteLine("Full Backup complete.");

                // Remove the backup device from the Backup object.   
                bk.Devices.Remove(bdi);

                // Make a change to the database, in this case, add a table called test_table.   
                Microsoft.SqlServer.Management.Smo.Table t = default(Microsoft.SqlServer.Management.Smo.Table);
                t = new Microsoft.SqlServer.Management.Smo.Table(db, "test_table");
                Column c = default(Column);
                c = new Column(t, "col", Microsoft.SqlServer.Management.Smo.DataType.Int);
                t.Columns.Add(c);
                t.Create();

                // Create another file device for the differential backup and add the Backup object.   
                BackupDeviceItem bdid = default(BackupDeviceItem);
                bdid = new BackupDeviceItem("Test_Differential_Backup1", DeviceType.File);

                // Add the device to the Backup object.   
                bk.Devices.Add(bdid);

                // Set the Incremental property to True for a differential backup.   
                bk.Incremental = true;

                // Run SqlBackup to perform the incremental database backup on the instance of SQL Server.   
                bk.SqlBackup(srv);

                // Inform the user that the differential backup is complete.   
                System.Console.WriteLine("Differential Backup complete.");

                // Remove the device from the Backup object.   
                bk.Devices.Remove(bdid);

                // Delete the EBLACc database before restoring it  
                // db.Drop();  

                // Define a Restore object variable.  
                Restore rs = new Restore();

                // Set the NoRecovery property to true, so the transactions are not recovered.   
                rs.NoRecovery = true;

                // Add the device that contains the full database backup to the Restore object.   
                rs.Devices.Add(bdi);

                // Specify the database name.   
                rs.Database = "EBLACc";

                // Restore the full database backup with no recovery.   
                rs.SqlRestore(srv);

                // Inform the user that the Full Database Restore is complete.   
                Console.WriteLine("Full Database Restore complete.");

                // reacquire a reference to the database  
                db = srv.Databases["EBLACc"];

                // Remove the device from the Restore object.  
                rs.Devices.Remove(bdi);

                // Set the NoRecovery property to False.   
                rs.NoRecovery = false;

                // Add the device that contains the differential backup to the Restore object.   
                rs.Devices.Add(bdid);

                // Restore the differential database backup with recovery.   
                rs.SqlRestore(srv);

                // Inform the user that the differential database restore is complete.   
                System.Console.WriteLine("Differential Database Restore complete.");

                // Remove the device.   
                rs.Devices.Remove(bdid);

                // Set the database recovery mode back to its original value.  
                db.RecoveryModel = (RecoveryModel)recoverymod;

                // Drop the table that was added.   
                db.Tables["test_table"].Drop();
                db.Alter();

                // Remove the backup files from the hard disk.  
                // This location is dependent on the installation of SQL Server  
                System.IO.File.Delete("C:\\Program Files\\Microsoft SQL Server\\MSSQL12.MSSQLSERVER\\MSSQL\\Backup\\Test_Full_Backup1");
                System.IO.File.Delete("C:\\Program Files\\Microsoft SQL Server\\MSSQL12.MSSQLSERVER\\MSSQL\\Backup\\Test_Differential_Backup1");

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        ~DbProvider()
        {
            Dispose(false);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            // check if already disposed
            if (!disposedValue)
            {
                try
                {
                    File.Delete(BackupFilename);
                }
                catch
                {
                }
                // set the bool value to true
                disposedValue = true;
            }
        }

        // The consumer object can call
        // the below dispose method
        public void Dispose()
        {
            // Invoke the above virtual
            // dispose(bool disposing) method
            Dispose(disposing: true);

            // Notify the garbage collector
            // about the cleaning event
            GC.SuppressFinalize(this);
        }
    }
}
