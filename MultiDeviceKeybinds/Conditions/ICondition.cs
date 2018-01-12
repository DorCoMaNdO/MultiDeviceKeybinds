using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    /// <summary>
    /// The condition interface.
    /// 
    /// <para>All types inheriting this interface will be loaded if placed in the same folder as the hook.</para>
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// The condition's name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// A description of the condition's actions.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The instructions of arguments taken by the condition.
        /// </summary>
        string ArgumentsTaken { get; }

        /// <summary>
        /// Passes information about the device and the key that was pressed along with user-set arguments.
        /// </summary>
        /// <param name="device">The device that the key(s) were pressed on</param>
        /// <param name="key">The key that was pressed (corrected for left/right for modifiers)</param>
        /// <param name="state">The key's state</param>
        /// <param name="lastState">The key's last state</param>
        /// <param name="guid">The GUID of the keybind that activated this condition, the arguments passed in <paramref name="args"/> won't differ unless the keybind was edited, a new GUID is generated then.
        /// <para>To be used to reduce processing after the first execution by a specific keybind, if the processing of arguments takes a considerable duration.</para></param>
        /// <param name="args">The arguments set by the user</param>
        /// <returns>Whether the device, the key, the key state and the arguments satisfy the condition</returns>
        bool Test(KeybindDevice device, Keys key, KeyState state, KeyState lastState, string guid, params object[] args);
    }
}