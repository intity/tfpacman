//
// Author: Siarhei Arkhipenka (c) 2006-2007
// E-mail: sbs-arhipenko@yandex.ru
//
using System;
using System.Collections.Generic;

namespace UndoRedoFramework
{
    /// <summary>
    /// The UndoRedoManager class.
    /// </summary>
    public static class UndoRedoManager
    {
        private static readonly List<Command> history = new List<Command>();
        private static int currentPosition = -1;

        internal static Command CurrentCommand { get; private set; } = null;

        /// <summary>
        /// Returns true if history has command that can be undone
        /// </summary>
        public static bool CanUndo => currentPosition >= 0;

        /// <summary>
        /// Returns true if history has command that can be redone
        /// </summary>
        public static bool CanRedo => currentPosition < history.Count - 1;

        /// <summary>
        /// Undo last command from history list
        /// </summary>
        public static void Undo()
        {
            AssertNoCommand();
            if (CanUndo)
            {
                Command command = history[currentPosition--];
                foreach (IUndoRedoMember member in command.Keys)
                {
                    member.OnUndo(command[member]);
                }
                OnCommandDone(CommandDoneType.Undo, command.Caption);
            }
        }

        /// <summary>
        /// Repeats command that was undone before
        /// </summary>
        public static void Redo()
        {
            AssertNoCommand();
            if (CanRedo)
            {
                Command command = history[++currentPosition];
                foreach (IUndoRedoMember member in command.Keys)
                {
                    member.OnRedo(command[member]);
                }
                OnCommandDone(CommandDoneType.Redo, command.Caption);
            }
        }

        /// <summary>
        /// Start command. Any data changes must be done within a command.
        /// </summary>
        /// <param name="commandCaption"></param>
        /// <returns></returns>
        public static IDisposable Start(string commandCaption)
        {
            AssertNoCommand();
            CurrentCommand = new Command(commandCaption);
            return CurrentCommand;
        }

        /// <summary>
        /// Commits current command and saves changes into history
        /// </summary>
        public static void Commit()
        {
            AssertCommand();
            foreach (IUndoRedoMember member in CurrentCommand.Keys)
                member.OnCommit(CurrentCommand[member]);

            // add command to history (all redo records will be removed)
            int count = history.Count - currentPosition - 1;
            history.RemoveRange(currentPosition + 1, count);

            history.Add(CurrentCommand);
            currentPosition++;
            TruncateHistory();
            var caption = CurrentCommand.Caption;
            CurrentCommand = null;
            OnCommandDone(CommandDoneType.Commit, caption);
        }

        /// <summary>
        /// Rollback current command. It does not saves any changes done in current command.
        /// </summary>
        public static void Cancel()
        {
            AssertCommand();
            foreach (IUndoRedoMember member in CurrentCommand.Keys)
                member.OnUndo(CurrentCommand[member]);
            CurrentCommand = null;
        }

        /// <summary>
        /// Clears all history. It does not affect current data but history only. 
        /// It is usefull after any data initialization if you want forbid user to undo this 
        /// initialization.
        /// </summary>
        public static void FlushHistory()
        {
            CurrentCommand = null;
            currentPosition = -1;
            history.Clear();
        }

        /// <summary>
        /// Checks there is no command started.
        /// </summary>
        internal static void AssertNoCommand()
        {
            if (CurrentCommand != null)
                throw new InvalidOperationException("Previous command is not completed. Use UndoRedoManager.Commit() to complete current command.");
        }

        /// <summary>
        /// Checks that command had been started.
        /// </summary>
        internal static void AssertCommand()
        {
            if (CurrentCommand == null)
                throw new InvalidOperationException("Command is not started. Use method UndoRedoManager.Start().");
        }

        public static bool IsCommandStarted => CurrentCommand != null;

        /// <summary>
        /// Gets an enumeration of commands captions that can be undone.
        /// </summary>
        /// <remarks>
        /// The first command in the enumeration will be undone first
        /// </remarks>
        public static IEnumerable<string> UndoCommands
        {
            get
            {
                for (int i = currentPosition; i >= 0; i--)
                    yield return history[i].Caption;
            }
        }

        /// <summary>
        /// Gets an enumeration of commands captions that can be redone.
        /// </summary>
        /// <remarks>
        /// The first command in the enumeration will be redone first
        /// </remarks>
        public static IEnumerable<string> RedoCommands
        {
            get
            {
                for (int i = currentPosition + 1; i < history.Count; i++)
                    yield return history[i].Caption;
            }
        }

        public static event EventHandler<CommandDoneEventArgs> CommandDone;
        static void OnCommandDone(CommandDoneType type, string caption)
        {
            CommandDone?.Invoke(null, new CommandDoneEventArgs(type, caption));
        }

        private static int maxHistorySize = 0;

        /// <summary>
        /// Gets/sets max commands stored in history. 
        /// Zero value (default) sets unlimited history size.
        /// </summary>
        public static int MaxHistorySize
        {
            get { return maxHistorySize; }
            set
            {
                if (IsCommandStarted)
                    throw new InvalidOperationException("Max size may not be set while command is run.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Value may not be less than 0");
                maxHistorySize = value;
                TruncateHistory();
            }
        }

        private static void TruncateHistory()
        {
            if (maxHistorySize > 0)
                if (history.Count > maxHistorySize)
                {
                    int count = history.Count - maxHistorySize;
                    history.RemoveRange(0, count);
                    currentPosition -= count;
                }
        }

    }

    public enum CommandDoneType
    {
        Commit, Undo, Redo
    }
}
