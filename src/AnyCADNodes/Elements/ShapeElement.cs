using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Geometry;
using AnyCADNodes.Extension;
using AElement = AnyCAD.Foundation.Element;
using AShapeElement = AnyCAD.Foundation.ShapeElement;
using DynamoServices;
using AnyCADServices.Persistence;

namespace AnyCADNodes.Elements
{
    [RegisterForTrace]
    public class ShapeElement : Element
    {
        internal AShapeElement InternalShape { get; private set; }
        public override AElement InternalElement => InternalShape;

        private ShapeElement(TopoShape topoShape)
        {
            SafeInit(() => Init(topoShape));
        }

        public static ShapeElement ByGeometry(Geometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            var topoShape = geometry.To();
            if (topoShape == null)
            {
                throw new ArgumentNullException(nameof(topoShape));
            }

            return new ShapeElement(topoShape);
        }

        public static ShapeElement BySphereCenterRadius(Point center, double radius = 1.0)
        {
            var topoShape = ShapeBuilder.MakeSphere(center.To(), radius);
            return new ShapeElement(topoShape);
        }

        private void Init(TopoShape topoShape)
        {
            // try to reuse from trace
            var shapeElement = AShapeElement.Cast(ElementBinder.GetElementFromTrace(Document));

            UndoTransaction undo = new(Document);
            undo.Start("shape");
            if (shapeElement == null)
            {
                shapeElement = AShapeElement.Create(Document);
            }
            shapeElement.SetShape(topoShape);
            undo.Commit();

            InternalInit(shapeElement);

            // TODO: clear and set?
            ElementBinder.SetElementForTrace(shapeElement.GetId());
        }

        private void InternalInit(AShapeElement shapeElement)
        {
            InternalShape = shapeElement;
            InternalId = shapeElement.GetId();
        }
    }
}
