using System.Collections.Generic;
using System.Linq;
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

            int flag = 1;
            string deltaNumber = string.Empty;
            string newString = string.Empty;
            string message = string.Empty; 

            IList<Element> listAreas = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType().Where(a => a.LookupParameter("ROM_Зона").AsString().Contains("Квартира")).ToList();

            List<Element> listAreasGroupZone = listAreas.OrderBy(x => x.get_Parameter(BuiltInParameter.LEVEL_NAME).AsString())
                                                            .ThenBy(y => y.get_Parameter(BuiltInParameter.LEVEL_NAME).AsString())
                                                            .ThenBy(a => a.LookupParameter("BS_Блок").AsString())
                                                            .ThenBy(b => b.LookupParameter("ROM_Подзона").AsString())
                                                            .ThenBy(c => c.LookupParameter("ROM_Зона").AsString())
                                                            .ToList();

            int length = listAreasGroupZone.Count();

            using (Transaction t = new Transaction(doc, "add val par"))
            {
                t.Start();

                for (int i = 0; i < length - 1; i++)
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
                        {
                            if (flag == 1)
                            {
                                flag = -1;
                                newString = listAreasGroupZone[i + 1].LookupParameter("ROM_Расчетная_подзона_ID").AsString() + ".Полутон";
                            }
                            else
                            {
                                flag = 1;
                                newString = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        flag = 1;
                        newString = string.Empty;
                    }
                    listAreasGroupZone[i + 1].LookupParameter("ROM_Подзона_Index").Set(newString);
                }
                doc.Regenerate();

                t.Commit();
            }
        }
    }
}