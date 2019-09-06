// Siarhei Arkhipenka (c) 2006-2007. email: sbs-arhipenko@yandex.ru
using System;
using System.Collections.Generic;
using System.Text;

namespace UndoRedoFramework
{
    public interface IUndoRedoMember
    {
        void OnCommit(object change);
        void OnUndo(object change);
        void OnRedo(object change);
    }
}
