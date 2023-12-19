using System;

namespace UndoRedoFramework
{
    public class CommandDoneEventArgs : EventArgs
    {
        public readonly CommandDoneType CommandDoneType;
        public readonly string Caption;
        public CommandDoneEventArgs(CommandDoneType type, string caption)
        {
            CommandDoneType = type;
            Caption = caption;
        }
    }
}
