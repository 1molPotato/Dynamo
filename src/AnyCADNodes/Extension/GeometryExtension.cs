using AnyCAD.Foundation;
using Autodesk.DesignScript.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCADNodes.Extension
{
    internal static class GeometryExtension
    {
        public static GPnt To(this Point pt)
        {
            return new GPnt(pt.X, pt.Y, pt.Z);
        }

        public static TopoShape To(this Geometry geometry)
        {
            // TODO
            return null;
        }
    }
}
