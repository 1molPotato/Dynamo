using AnyCAD.Foundation;

namespace Dynamo.RapidAnyCAD.Trial
{
    internal class MainDocumentListener : DocumentListener
    {
        private MainViewModel mContext;

        public MainDocumentListener(MainViewModel context)
        {
            mContext = context;
        }

        public override void AfterDocumentChanged(Document document, DocumentEventArgs args)
        {
            if (mContext.Document.IsEqual(document))
            {
                mContext.OnDocumentChanged(args);
            }
        }

        public override void OnActiveDocument(Document pDocument)
        {
            Application.Instance().ShowDocument(pDocument);
        }
    }
}
