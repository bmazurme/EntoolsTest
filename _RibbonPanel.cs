using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace Entools.Model
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class Entools : IExternalApplication
    {
        static readonly string AddInPath = typeof(Entools).Assembly.Location;
        static readonly string ButtonIconsFolder = Path.GetDirectoryName(AddInPath);

        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {            
            try
            {
                CreateRibbonEntoolsPanel(application);
                application.ControlledApplication.DocumentOpened += OnDocOpened;

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("EnToolsLt Sample", ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
        }


        public Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= OnDocOpened;

            return Autodesk.Revit.UI.Result.Succeeded;
        }


        private void OnDocOpened(object sender, DocumentOpenedEventArgs args)
        {
            Autodesk.Revit.ApplicationServices.Application app
               = (Autodesk.Revit.ApplicationServices.Application)sender;
            Document doc = args.Document;           
        }


        private void CreateRibbonEntoolsPanel(UIControlledApplication application)
        {
            String tabName = "EnToolsTest";        
            string settingsTabPart = "Test";
            application.CreateRibbonTab(tabName);            

            RibbonPanel ribbonPlumbingPanel = application.CreateRibbonPanel(tabName, settingsTabPart);
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.ChmFile, path + "/EntoolsRibbonHelp.htm");         

            PushButtonData pushButtonData = new PushButtonData("AddPar", "AddPar", AddInPath,
            "Entools.Model.ClassAddPar") { ToolTip = "Help" };
            PushButton pushButton = ribbonPlumbingPanel.AddItem(pushButtonData) as PushButton;
            //pushButton.Image = new Converter().Convert(Properties.Resources.settings_20);
            //pushButton.LargeImage = new Converter().Convert(Properties.Resources.settings_24);

            pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                                                 "_img\\insert_24.png"), UriKind.Absolute));
            pushButton.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                                                "_img\\insert_20.png"), UriKind.Absolute));

            pushButton.LongDescription = "Help";
            //pushButton.SetContextualHelp(contextHelp);

            ribbonPlumbingPanel.AddSeparator();
        }
    }
}