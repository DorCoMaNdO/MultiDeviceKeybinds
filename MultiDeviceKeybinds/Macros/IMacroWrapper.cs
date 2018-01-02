namespace MultiDeviceKeybinds
{
    class IMacroWrapper
    {
        public IMacro Macro { get; private set; }

        public IMacroWrapper(IMacro macro)
        {
            Macro = macro;
        }

        public override string ToString()
        {
            return Macro?.Name ?? "Unnamed macro";
        }
    }
}