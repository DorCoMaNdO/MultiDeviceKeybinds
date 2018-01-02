namespace MultiDeviceKeybinds
{
    class IConditionWrapper
    {
        public ICondition Condition { get; private set; }

        public IConditionWrapper(ICondition condition)
        {
            Condition = condition;
        }

        public override string ToString()
        {
            return Condition?.Name ?? "Unnamed macro";
        }
    }
}