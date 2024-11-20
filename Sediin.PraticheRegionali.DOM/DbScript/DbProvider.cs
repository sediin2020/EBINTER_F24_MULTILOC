using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Sediin.PraticheRegionali.DOM.Data;
using Sediin.PraticheRegionali.DOM.Entitys;

namespace Sediin.PraticheRegionali.DOM.DbScript
{

    public class DbProvider : IDisposable
    {
        #region MyRegion

        public string Dbname
        {
            get
            {
                SediinPraticheRegionaliDbContext context = new SediinPraticheRegionaliDbContext();

                return context.Database.Connection.Database.ToString();
            }
        }

        public async Task BackupDatabase(string path)
        {
            var name = "";
            var backupName = "";
            var backupNameZip = "";
            try
            {
                var _connection = ConfigurationManager.ConnectionStrings[1].ConnectionString.Split(';');

                var _datasource = _connection.FirstOrDefault(x => x.TrimAll().StartsWith("data Source="))?.Replace("data Source=", "").TrimAll();

                var _username = _connection.FirstOrDefault(x => x.TrimAll().StartsWith("user="))?.Replace("user=", "").TrimAll();

                var _pwd = _connection.FirstOrDefault(x => x.TrimAll().StartsWith("pwd="))?.Replace("pwd=", "").TrimAll();

                name = Path.Combine(path, Dbname + "_" + DateTime.Now.Ticks);
                backupName = name + ".bak";
                backupNameZip = name + ".zip";

                // Connect to the local, default instance of SQL Server.   
                Server srv = new Server(_datasource);
                srv.ConnectionContext.LoginSecure = false;
                srv.ConnectionContext.Login = _username;
                srv.ConnectionContext.Password = _pwd;


                // Reference the database.   
                Microsoft.SqlServer.Management.Smo.Database db = default;
                // new Microsoft.SqlServer.Management.Smo.Database(srv, dbname);
                db = srv.Databases[Dbname];

                // Store the current recovery model in a variable.   
                int recoverymod;
                recoverymod = (int)db.DatabaseOptions.RecoveryModel;

                // Define a Backup object variable.   
                Backup bk = new Backup();

                // Specify the type of backup, the description, the name, and the database to be backed up.   
                bk.Action = BackupActionType.Database;
                bk.BackupSetDescription = "Full backup";
                bk.BackupSetName = "Backup";
                bk.Database = Dbname;

                // Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
                BackupDeviceItem bdi = default(BackupDeviceItem);
                bdi = new BackupDeviceItem(backupName, DeviceType.File);

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

                // Remove the backup device from the Backup object.   
                bk.Devices.Remove(bdi);

                await ZipDatabase(backupName, backupNameZip);

                try
                {
                    File.Delete(backupName);
                }
                catch
                {
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ZipDatabase(string backupName, string backupNameZip)
        {
            using (var archive = ZipFile.Open(backupNameZip, ZipArchiveMode.Create))
            {
                // we're getting a file stream
                // here but it could be any stream
                var file = File.OpenRead(backupName);
                var entry =
                    archive.CreateEntry(
                        Path.GetFileName(backupName),
                        CompressionLevel.Optimal
                    );

                await file.CopyToAsync(entry.Open());
                file.Dispose();
            }
        }

        #endregion
        public void RestoreDatabaseZip(string backupName)
        {
            var _isZip = false;
            var backup = backupName;

            var _tempfolder = Path.GetTempPath();

            if (Path.GetExtension(backupName) == ".zip")
            {
                _isZip = true;

                backup = Path.Combine(_tempfolder, Path.GetFileNameWithoutExtension(backupName) + ".bak");

                if (File.Exists(backup))
                {
                    File.Delete(backup);
                }

                ZipFile.ExtractToDirectory(backupName, _tempfolder);

                //backup = Directory.GetFiles(_tempfolder, Path.GetFileName(_bak), SearchOption.AllDirectories).FirstOrDefault();
            }

            RestoreDb(backup);

            if (_isZip)
            {
                if (File.Exists(backup))
                {
                    File.Delete(backup);
                }
            }

        }

        public void RestoreDatabase(string backupName)
        {
            Server srv;
            srv = new Server();
            var s = srv.MasterDBLogPath;
            Restore rs = new Restore();
            //string fileName = backupName;
            // string databaseName = Dbname;

            //// Define a Restore object variable.  
            //
            //rs.Database = databaseName;
            //rs.ReplaceDatabase = true;
            //rs.Action = RestoreActionType.Database;
            //rs.Devices.AddDevice(fileName, DeviceType.File);

            //rs.PercentCompleteNotification = 10;
            ////rs.ReplaceDatabase = true;
            ////rs.PercentComplete += new PercentCompleteEventHandler(ProgressEventHandler);
            //rs.SqlRestore(srv);


            // Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
            BackupDeviceItem bdi = default(BackupDeviceItem);
            bdi = new BackupDeviceItem(backupName, DeviceType.File);



            // Set the NoRecovery property to true, so the transactions are not recovered.   
            rs.NoRecovery = true;

            // Add the device that contains the full database backup to the Restore object.   
            rs.Devices.Add(bdi);

            // Specify the database name.   
            rs.Database = Dbname;

            // Restore the full database backup with no recovery.   
            rs.SqlRestore(srv);

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

        private void RestoreDb(string backupName)
        {
            Server srv;
            srv = new Server();

            Restore dbRestore = new Restore();
            dbRestore.Database = Dbname;
            dbRestore.Action = RestoreActionType.Database;
            dbRestore.ReplaceDatabase = true;

            string fileLocation = srv.MasterDBLogPath;

            try
            {
                BackupDeviceItem device = new BackupDeviceItem
                    ( backupName, DeviceType.File);
                dbRestore.Devices.Add(device);
            //    DataTable dtFiles = dbRestore.ReadFileList(srv);
            //    string backupDbLogicalName = dtFiles.Rows[0]["LogicalName"].ToString();

            //    RelocateFile dbRf = new RelocateFile
            //(backupDbLogicalName, string.Format("{0}\\{1}.mdf", fileLocation, dbRestore));
            //    RelocateFile logRf = new RelocateFile(string.Format("{0}_log",
            //backupDbLogicalName), string.Format("{0}\\{1}_Log.ldf",
            //fileLocation, Dbname));
            //    dbRestore.RelocateFiles.Add(dbRf);
            //    dbRestore.RelocateFiles.Add(logRf);


                //dbRestore.Complete += new ServerMessageEventHandler(dbRestore_Complete);
                //dbRestore.PercentComplete +=
                //new PercentCompleteEventHandler(PercentComplete);
                dbRestore.SqlRestore(srv);
            }
            catch (Exception exc)
            {
                dbRestore.Abort();
                MessageBox.Show(string.Format
                ("Exception occurred.\nMessage: {0}", exc.Message));
            }
            finally
            {
                //sqlConn.Close();
            }
        }


        public void Main()
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {
                // Connect to the local, default instance of SQL Server.   
                Server srv = new Server();

                var s = srv.Databases.Count;
                // Reference the  database.   
                Microsoft.SqlServer.Management.Smo.Database db = default;// new Microsoft.SqlServer.Management.Smo.Database(srv, "");
                db = srv.Databases["EBLACc"];

                // Store the current recovery model in a variable.   
                int recoverymod;
                recoverymod = (int)db.DatabaseOptions.RecoveryModel;

                // Define a Backup object variable.   
                Backup bk = new Backup();

                // Specify the type of backup, the description, the name, and the database to be backed up.   
                bk.Action = BackupActionType.Database;
                bk.BackupSetDescription = "Full backup of ";
                bk.BackupSetName = " Backup";
                bk.Database = "EBLACc";

                // Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
                BackupDeviceItem bdi = default(BackupDeviceItem);
                bdi = new BackupDeviceItem(@"c:\temp\bak\Test_Full_Backup1", DeviceType.File);

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

                //// Make a change to the database, in this case, add a table called test_table.   
                //Microsoft.SqlServer.Management.Smo.Table t = default(Microsoft.SqlServer.Management.Smo.Table);
                //t = new Microsoft.SqlServer.Management.Smo.Table(db, "test_table");
                //Column c = default(Column);
                //c = new Column(t, "col", Microsoft.SqlServer.Management.Smo.DataType.Int);
                //t.Columns.Add(c);
                //t.Create();

                //// Create another file device for the differential backup and add the Backup object.   
                //BackupDeviceItem bdid = default(BackupDeviceItem);
                //bdid = new BackupDeviceItem("Test_Differential_Backup1", DeviceType.File);

                //// Add the device to the Backup object.   
                //bk.Devices.Add(bdid);

                //// Set the Incremental property to True for a differential backup.   
                //bk.Incremental = true;

                //// Run SqlBackup to perform the incremental database backup on the instance of SQL Server.   
                //bk.SqlBackup(srv);

                //// Inform the user that the differential backup is complete.   
                //System.Console.WriteLine("Differential Backup complete.");

                //// Remove the device from the Backup object.   
                //bk.Devices.Remove(bdid);

                // Delete the  database before restoring it  
                // db.Drop();  

                // db.RecoveryModel = (RecoveryModel)RecoveryModel.Full;

                // Define a Restore object variable.  
                Restore rs = new Restore();
                rs.ReplaceDatabase = true;
                // Set the NoRecovery property to true, so the transactions are not recovered.   
                rs.NoRecovery = false;

                // Add the device that contains the full database backup to the Restore object.   
                rs.Devices.Add(bdi);

                // Specify the database name.   
                rs.Database = "EBLACc";

                // Restore the full database backup with no recovery.   
                rs.SqlRestore(srv);

                //// Inform the user that the Full Database Restore is complete.   
                //Console.WriteLine("Full Database Restore complete.");

                //// reacquire a reference to the database  
                //db = srv.Databases["EBLACc"];

                //// Remove the device from the Restore object.  
                //rs.Devices.Remove(bdi);

                //// Set the NoRecovery property to False.   
                //rs.NoRecovery = false;

                //// Add the device that contains the differential backup to the Restore object.   
                //rs.Devices.Add(bdid);

                //// Restore the differential database backup with recovery.   
                //rs.SqlRestore(srv);

                //// Inform the user that the differential database restore is complete.   
                //System.Console.WriteLine("Differential Database Restore complete.");

                //// Remove the device.   
                //rs.Devices.Remove(bdid);

                //// Set the database recovery mode back to its original value.  
                //db.RecoveryModel = (RecoveryModel)recoverymod;

                //// Drop the table that was added.   
                //db.Tables["test_table"].Drop();
                //db.Alter();

                // Remove the backup files from the hard disk.  
                // This location is dependent on the installation of SQL Server  
                System.IO.File.Delete("C:\\Program Files\\Microsoft SQL Server\\MSSQL12.MSSQLSERVER\\MSSQL\\Backup\\Test_Full_Backup1");
                System.IO.File.Delete("C:\\Program Files\\Microsoft SQL Server\\MSSQL12.MSSQLSERVER\\MSSQL\\Backup\\Test_Differential_Backup1");

            }
            catch (Exception ex)
            {

                throw;
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
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
