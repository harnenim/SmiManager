using System.Security.Permissions;
using Subtitle;

namespace SmiManager
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    public partial class MainForm : WebForm.WebForm
    {
        #region 초기화

        public MainForm()
        {
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Location = new System.Drawing.Point(554, 0);
            ClientSize = new System.Drawing.Size(1360, 749);
            Name = "MainForm";
            Text = "Sync Manager";
        }

        public override void InitAfterLoad()
        {
            // 파일 작업할 일이 있나?
            //SetDragEvent("originSmi", DragDropEffects.All, new DropActionDelegate(DropFile));
            //SetClickEvent("btnSplitOrigin", "SplitOrigin");
            //SetClickEvent("btnFrameAssToSmi", "FrameAssToSmi");
            //SetClickEvent("btnJoinFrame", "JoinFrame");

            Script("initMain");
        }

        #endregion


        public void FillSync(string id)
        {
            string input = Script("$val", new object[] { "#" + id + " > textarea.input" });
            SmiFile file = SmiFile.FromTxt(input);
            Smi.FillEmptySync(file.body);
            string output = file.ToTxt();
            Script("$val", new object[] { "#" + id + " > textarea.output", output });
        }

        public void Normalize(string id)
        {
            string input = Script("$val", new object[] { "#" + id + " > textarea.input" });
            SmiFile file = SmiFile.FromTxt(input);
            Smi.Normalize(file.body);
            string output = file.ToTxt();
            Script("$val", new object[] { "#" + id + " > textarea.output", output });
        }

        public void Fusion(string id)
        {
            string[] inputSync = Script("$val", new object[] { "#" + id + " textarea.inputSync" }).Replace("\r\n", "\n").Split('\n');
            string[] inputText = Script("$val", new object[] { "#" + id + " textarea.inputText" }).Replace("\r\n", "\n").Split('\n');
            
            string output = "";
            for (int i = 0; i * 2 < inputSync.Length && i < inputText.Length; i++)
                output += inputSync[i * 2] + "\n" + inputText[i] + "\n";

            Script("$val", new object[] { "#" + id + " textarea.output", output });
        }
    }
}
