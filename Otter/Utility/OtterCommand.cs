using System;

namespace Otter.Utility
{
    public class OtterCommand : Attribute
    {
        /// <summary>
        /// The string that can be typed into the console to invoke this method.
        /// </summary>
        public string Alias;

        /// <summary>
        /// The text that will appear when the method is called with no parameters (note: will never show up if the method has no parameters by default.)
        /// </summary>
        public string UsageText;

        /// <summary>
        /// The text that will appear along with the method when the user invokes the help command.
        /// </summary>
        public string HelpText;

        /// <summary>
        /// The method group to associate this method with. Groups can be added or removed during runtime.
        /// </summary>
        public string Group;

        /// <summary>
        /// If true the method will not run until the next update.
        /// </summary>
        public bool IsBuffered;

        /// <summary>
        /// Use named parameters to define this to make your life way easier.
        /// </summary>
        /// <param name="alias">The string that can be typed into the console to invoke this method.</param>
        /// <param name="usageText">The text that will appear when the method is called with no parameters (note: will never show up if the method has no parameters by default.)</param>
        /// <param name="helpText">The text that will appear along with the method when the user invokes the help command.</param>
        /// <param name="group">The method group to associate this method with. Groups can be added or removed during runtime.</param>
        /// <param name="isBuffered">If true the method will not run until the next update.</param>
        public OtterCommand(string alias = "", string usageText = "", string helpText = "", string group = "", bool isBuffered = false)
        {
            Alias = alias;
            UsageText = usageText;
            HelpText = helpText;
            Group = group;
            IsBuffered = isBuffered;
        }
    }
}
