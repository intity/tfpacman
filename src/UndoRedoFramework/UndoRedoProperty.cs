//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
using System.Diagnostics;

namespace UndoRedoFramework
{
    [DebuggerDisplay("{Value}")]
    public class UndoRedo<TValue> : IUndoRedoMember
    {
        public UndoRedo()
        {
            tValue = default;
        }

        public UndoRedo(TValue defaultValue)
        {
            tValue = defaultValue;
        }

        TValue tValue;
        public TValue Value
        {
            get => tValue;
            set
            {
                UndoRedoManager.AssertCommand();
                if (!UndoRedoManager.CurrentCommand.ContainsKey(this))
                {
                    Change<TValue> change = new Change<TValue>
                    {
                        OldState = tValue
                    };
                    UndoRedoManager.CurrentCommand[this] = change;
                }
                tValue = value;
            }
        }

        #region IUndoRedoMember Members
        void IUndoRedoMember.OnCommit(object change)
        {
            Debug.Assert(change != null);
            ((Change<TValue>)change).NewState = tValue;
        }

        void IUndoRedoMember.OnUndo(object change)
        {
            Debug.Assert(change != null);
            tValue = ((Change<TValue>)change).OldState;
        }

        void IUndoRedoMember.OnRedo(object change)
        {
            Debug.Assert(change != null);
            tValue = ((Change<TValue>)change).NewState;
        }
        #endregion  
    }
}
