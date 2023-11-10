//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
namespace UndoRedoFramework
{
    class Change<TState>
    {
        public TState OldState;
        public TState NewState;
    }
}
