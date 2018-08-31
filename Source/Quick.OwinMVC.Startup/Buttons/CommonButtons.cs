using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Buttons
{
    public class CommonButtons
    {
        public static CommonButtons Instance = new CommonButtons();

        private Label _VersionLabel;
        public object VersionLabel { get { return _VersionLabel; } }

        public Action Action_Exit { get; set; }
        public Action Action_Setting { get; set; }
        public Action Action_Debug { get; set; }

        public CommonButtons()
        {
            _VersionLabel = new Label() { Text = ProgramUtils.GetProgramVersion() };

            Action_Exit = () =>
            {
                foreach (var form in Application.OpenForms
                    .Cast<Form>()
                    .ToArray())
                {
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                }
                Environment.Exit(0);
            };
            Action_Setting = () => new Forms.SettingForm().ShowDialog();
            Action_Debug = () => ProgramUtils.StartSelfProcess("-debug", false, false);
        }
    }
}
