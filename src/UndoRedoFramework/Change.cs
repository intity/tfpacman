// Siarhei Arkhipenka (c) 2006-2007. email: sbs-arhipenko@yandex.ru
using System;
using System.Collections.Generic;
using System.Text;

namespace UndoRedoFramework
{
    class Change<TState>
    {
        public TState OldState;
        public TState NewState;
    } 
}
