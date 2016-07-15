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

        public Action Action_Exit { get; set; }
        public Action Action_Setting { get; set; }
        public Action Action_Debug { get; set; }

        public CommonButtons()
        {
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
