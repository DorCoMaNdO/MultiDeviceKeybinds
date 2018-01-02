using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    internal class CustomListView : ListView
    {
        private bool BlockCheck = false;

        public CustomListView() : base()
        {
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (BlockCheck)
            {
                ice.NewValue = ice.CurrentValue;

                BlockCheck = false;

                return;
            }

            base.OnItemCheck(ice);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks > 1) BlockCheck = true;

            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BlockCheck = false;

            base.OnKeyDown(e);
        }
    }
}
