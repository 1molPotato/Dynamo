using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyCADServices;

using AElement = AnyCAD.Foundation.Element;

namespace AnyCADNodes.Elements
{
    /// <summary>
    /// ShapeElement
    /// </summary>
    public abstract class Element : IDisposable
    {
        protected void SafeInit(Action init)
        {
            try
            {
                init();
            }
            catch (Exception e)
            {
                // TODO
                throw e;
            }
        }

        internal static Document Document => Application.Instance().GetActiveDocument();

        /// <summary>
        /// 若当前对象已经被anycad所拥有，那么在Dispose中不应被删除
        /// </summary>
        internal bool _isOwned = false;

        [SupressImportIntoVM]
        public abstract AElement InternalElement { get; }

        private ObjectId _internalId;

        protected ObjectId InternalId
        {
            get
            {
                if (_internalId == null || _internalId.IsInvalid())
                    return InternalElement != null ? InternalElement.GetId() : ObjectId.InvalidId;
                return _internalId;
            }
            set
            {
                _internalId = value;

                // TODO: id lifecycle manager
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public virtual void Dispose()
        {
            if (DisposeLogic.IsShuttingDown || DisposeLogic.IsClosingHomeworkspace)
                return;

            if (!_isOwned && InternalId.IsValid())
            {
                UndoTransaction undo = new(Document);
                undo.Start("erase");
                Document.RemoveElement(InternalId);
                undo.Commit();
            }

            InternalId = ObjectId.InvalidId;
        }
    }
}
