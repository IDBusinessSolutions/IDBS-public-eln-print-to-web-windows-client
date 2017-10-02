// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ID Business Solutions Ltd.">
//
//    Copyright (C) 2014
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Idbs.Ewb.Printing.Api;
using log4net;
using log4net.Config;
using PrintToEwbInbox.Properties;

namespace PrintToEwbInbox
{
    /// <summary>
    /// A simple command line application used for starting and monitoring the EWB printer.
    /// Note that the printer is only available when this application is alive.
    /// </summary>
    public class Program
    {
        /// <summary> The configured logger instance. </summary>
        private static ILog logger;

        /// <summary> Indicates the background thread is running </summary>
        private static volatile bool working = true;

        /// <summary> The background thread. </summary>
        private static Thread backgroundThread;

        /// <summary>
        /// The main application entry point.
        /// </summary>
        public static void Main()
        {
            IPrinterService service = null;

            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                FileInfo assemblyFile = new FileInfo(location);
                if (assemblyFile.Directory == null)
                    throw new ApplicationException("Unable to determine the location of the running application");

                Directory.SetCurrentDirectory(assemblyFile.Directory.FullName);
                string logFileDir = Path.Combine(Application.UserAppDataPath, "Logs");
                if (!Directory.Exists(logFileDir)) Directory.CreateDirectory(logFileDir);

                GlobalContext.Properties["LogDir"] = logFileDir;

                XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                logger = LogManager.GetLogger(typeof(Program));

                /* Initialize and load the composition container for dependency injection */
                var aggregateCatalog = new AggregateCatalog();

                /* Just look in the executable directory for implementations */
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory));

                var container = new CompositionContainer(aggregateCatalog);

                /* Create the printer inside of the using block (will delete the printer when this block
                 * exits as long as the printer did not already exist). */
                service = container.GetExportedValue<IPrinterService>();

                RunLoop(service);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                try
                {
                    if (service != null)
                        service.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                }
            }
        }

        /// <summary>
        /// Called to signal that the printer service should be shut down
        /// </summary>
        public static void Shutdown()
        {
            logger.Info("Shutdown request received");

            working = false;
            backgroundThread.Join();
        }

        /// <summary>
        /// Runs the printer loop on a separate thread
        /// </summary>
        /// <param name="service"> The service to run. </param>
        private static void RunLoop(IPrinterService service)
        {
            try
            {
                // Set up a tray icon to enable this application to be shut down by the user
                NotifyIcon trayIcon = new NotifyIcon();
                trayIcon.Text = Resources.TrayText;
                trayIcon.Icon = Resources.ewb16x16;

                ContextMenu trayMenu = new ContextMenu();

                EventHandler exitHandler = (s, e) => Shutdown();
                trayMenu.MenuItems.Add(Resources.ExitMenuText, exitHandler);

                trayIcon.ContextMenu = trayMenu;
                trayIcon.Visible = true;

                logger.Info("Starting background thread");

                backgroundThread = new Thread(() =>
                {
                    try
                    {
                        logger.Info("Starting printer service");
                        service.Start();

                        while (working)
                        {
                            Thread.Sleep(250);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal(ex);
                    }
                    finally
                    {
                        logger.Info("Stopping printer service");
                        try
                        {
                            service.Stop();
                            service.Dispose();
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex);
                        }
                    }

                    Application.Exit();
                });

                backgroundThread.IsBackground = false;
                backgroundThread.Start();

                Application.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
