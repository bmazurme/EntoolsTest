using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Entools.Model
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class ClassAddPar : IExternalCommand
    {
        #region IExternalCommand Members Implementation
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message,
                                                                        ElementSet elements)
        {
            try
            {
                EntoolsTest test = new EntoolsTest();
                test.Main(revit);

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception ff)
            {
                return Autodesk.Revit.UI.Result.Failed;
            }

            throw new NotImplementedException();
        }
        #endregion IExternalCommand Members Implementation
    }
}