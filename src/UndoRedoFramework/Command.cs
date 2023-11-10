//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UndoRedoFramework
{
    internal class Command : Dictionary<IUndoRedoMember, object>, IDisposable
    {
        public readonly string Caption;
        public Command(string caption)
        {
            Caption = caption;
        }

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            if (UndoRedoManager.CurrentCommand != null)
            {
                if (UndoRedoManager.CurrentCommand == this)
                    UndoRedoManager.Cancel();
                else
                    Debug.Fail("There was another command within disposed command");
            }
        }
        #endregion
    }
}