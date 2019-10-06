using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IEditorLocator
    {
        void GotoLine(int line);
        void GotoPos(int pos);
    }
    public interface IEditorUndoRedo
    {
        void Undo();
        void Redo();
    }
    public interface IEditorController
    {
        bool IsModified { get; }
        String FileName { get; }
        void Save();
        void Reload();
    }
    public interface IEditor
    {
        IEditorController Controller { get; }
        IEditorUndoRedo UndoRedo { get; }
        IEditorLocator Locator{ get; }
    }
}
