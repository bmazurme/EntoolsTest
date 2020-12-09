using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Entools.Model
{
    class EntoolsTest
    {
        public void Main(ExternalCommandData revit)
        {
            UIDocument uidoc = revit.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            string newString = string.Empty;
            bool flag = true;

            // Skip checking for null.
            IList<Element> listAreas = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                                       .WhereElementIsNotElementType().Where(a => a.LookupParameter("ROM_Зона")
                                       .AsString().Contains("Квартира")).ToList();

            List<Element> listAreasGroupZone = listAreas.OrderBy(x => x.get_Parameter(BuiltInParameter.LEVEL_NAME).AsString())
                                                         .ThenBy(y => y.get_Parameter(BuiltInParameter.LEVEL_NAME).AsString())
                                                         .ThenBy(a => a.LookupParameter("BS_Блок").AsString())
                                                         .ThenBy(b => b.LookupParameter("ROM_Подзона").AsString())
                                                         .ThenBy(c => c.LookupParameter("ROM_Зона").AsString())
                                                         .ToList();

            using (Transaction t = new Transaction(doc, "Add value to parameter"))
            {
                t.Start();

                for (int i = 0; i < listAreasGroupZone.Count() - 1; i++)
                {
                    string levelFirst = listAreasGroupZone[i].get_Parameter(BuiltInParameter.LEVEL_NAME).AsString();
                    string levelSecond = listAreasGroupZone[i + 1].get_Parameter(BuiltInParameter.LEVEL_NAME).AsString();

                    string sectionFirst = listAreasGroupZone[i].LookupParameter("BS_Блок").AsString();
                    string sectionSecond = listAreasGroupZone[i + 1].LookupParameter("BS_Блок").AsString();

                    string typeFirst = listAreasGroupZone[i].LookupParameter("ROM_Подзона").AsString();
                    string typeSecond = listAreasGroupZone[i + 1].LookupParameter("ROM_Подзона").AsString();

                    string numberFirst = listAreasGroupZone[i].LookupParameter("ROM_Зона").AsString();
                    string numberSecond = listAreasGroupZone[i + 1].LookupParameter("ROM_Зона").AsString();

                    if (levelFirst == levelSecond && sectionFirst == sectionSecond && typeFirst == typeSecond)
                    {
                        if (numberFirst != numberSecond)                        
                            if (flag)
                            {
                                flag = false;
                                Parameter parameterId = listAreasGroupZone[i + 1].LookupParameter("ROM_Расчетная_подзона_ID");

                                if (parameterId != null) newString = parameterId.AsString() + ".Полутон";
                                else
                                {
                                    MessageBox.Show("ROM_Расчетная_подзона_ID == null");
                                    break;
                                }
                            }
                            else
                            {
                                flag = true;
                                newString = string.Empty;
                            }                        
                    }
                    else
                    {
                        flag = true;
                        newString = string.Empty;
                    }

                    Parameter parameterIndex = listAreasGroupZone[i + 1].LookupParameter("ROM_Подзона_Index");

                    if (parameterIndex != null) parameterIndex.Set(newString);
                    else
                    {
                        MessageBox.Show("ROM_Подзона_Index == null");
                        break;
                    }
                }
                doc.Regenerate();

                t.Commit();
            }
        }
    }
}