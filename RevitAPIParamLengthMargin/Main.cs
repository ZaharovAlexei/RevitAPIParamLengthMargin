using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIParamLengthMargin
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите трубы");

            for (int i = 0; i < selectedElementRefList.Count(); i++)
            {
                Element element = doc.GetElement(selectedElementRefList[i]);
                if (element is Pipe)
                {
                    Parameter lengthParameter = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                    double lengthMargin = 0;
                    if (lengthParameter.StorageType == StorageType.Double)
                    {
                        lengthMargin = (lengthParameter.AsDouble()) * 1.1;
                    }

                    using (Transaction ts = new Transaction(doc, "Set parameters"))
                    {
                        ts.Start();
                        Parameter lengthMarginParameter = element.LookupParameter("Длина с запасом");
                        lengthMarginParameter.Set(lengthMargin);
                        ts.Commit();
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
