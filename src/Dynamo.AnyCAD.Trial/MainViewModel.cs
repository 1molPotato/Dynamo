using Dynamo.Applications;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyCAD.WPF;
using AnyCAD.Foundation;
using System.Security.Cryptography;
using Dynamo.RapidAnyCAD.Trial.ViewModel;
using Dynamo.RapidAnyCAD.Trial.Model;
using Dynamo.Wpf.ViewModels.Watch3D;

namespace Dynamo.RapidAnyCAD.Trial
{
    public partial class MainViewModel
    {
        private RenderControl _renderControl;
        private MainDocumentListener _DocumentListener;
        public MainViewModel(RenderControl mRenderControl)
        {
            _renderControl = mRenderControl;
        }

        public Document Document => Application.Instance().GetActiveDocument();

        public bool Initialize()
        {
            if (_renderControl == null)
                return false;
            Application.Instance().SetActiveViewer(_renderControl.Viewer);
            _DocumentListener = new MainDocumentListener(this);
            DocumentEvent.Instance().AddListener(_DocumentListener);
            Document document = Application.Instance().CreateDocument(".acad");
            document.SetModified(false);
            Application.Instance().SetActiveDocument(document);
            return true;
        }

        public void OnDocumentChanged(DocumentEventArgs args)
        {
            ViewContext viewContext = _renderControl.ViewContext;
            if (viewContext == null || args.GetPreviewing())
            {
                return;
            }

            Scene scene = viewContext.GetScene();
            foreach (ObjectId addedId in args.GetAddedIds())
            {
                ulong integer = addedId.GetInteger();
                if (scene.FindNodeByUserId(integer) == null)
                {
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            foreach (ObjectId removedId in args.GetRemovedIds())
            {
                ulong integer2 = removedId.GetInteger();
                SceneNode sceneNode = scene.FindNodeByUserId(integer2);
                if (sceneNode != null)
                {
                    scene.RemoveNode(sceneNode.GetUuid());
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            Document document = Document;
            foreach (ObjectId changedId in args.GetChangedIds())
            {
                ObjectId objectId = changedId;
                ulong value = changedId.Value;
                Component component = Component.Cast(document.FindElement(changedId));
                if (component != null)
                {
                    objectId = component.GetEntityId();
                    value = objectId.Value;
                }

                if (scene.FindNodeByUserId(value) != null)
                {
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }
        }

        private DynamoViewModel? _dynamoViewModel;
        private DynamoView? _dynamoView;
        [RelayCommand]
        void OnOpenDynamo()
        {
            // init Dynamo View
            DynamoModel model;
            model = RapidDynamoModel.Start();

            _dynamoViewModel = RapidDynamoViewModel.Start(
                   new DynamoViewModel.StartConfiguration()
                   {
                       CommandFilePath = string.Empty,
                       DynamoModel = model,
                       ShowLogin = false
                   });

            _dynamoView = new DynamoView(_dynamoViewModel);
            _dynamoView.Show();
            _dynamoView.Activate();
        }

        [RelayCommand]
        void OnCreateSphere()
        {
            var doc = Application.Instance().GetActiveDocument();
            UndoTransaction undo = new(doc);
            undo.Start("sphere");
            var sphere = ShapeElement.Create(doc);
            sphere.SetShape(ShapeBuilder.MakeSphere(new GPnt(), 1.0));
            undo.Commit();
        }
    }
}
