//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
namespace UndoRedoFramework
{
    public interface IUndoRedoMember
    {
        void OnCommit(object change);
        void OnUndo(object change);
        void OnRedo(object change);
    }
}
