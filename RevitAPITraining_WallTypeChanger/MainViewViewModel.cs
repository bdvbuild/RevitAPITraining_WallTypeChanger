using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using RevitApiTrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_WallTypeChanger
{
    internal class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public List<Element> PickedObjects { get; } = new List<Element>();
        public List<WallType> WallTypes { get; } = new List<WallType>();
        public DelegateCommand WallTypeChange { get; }
        public WallType SelectedWallType { get; set; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallTypes = WallTypesSelection.GetWallTypes(commandData);
            WallTypeChange = new DelegateCommand(OnWallTypeChange);
        }

        private void OnWallTypeChange()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallType == null)
                return;
            using (var ts = new Transaction(doc, "Set wall type"))
            {
                ts.Start();
                foreach (var pickedObject in PickedObjects) 
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;
                        oWall.ChangeTypeId(SelectedWallType.Id);
                    }
                }
                ts.Commit();
            }

            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
