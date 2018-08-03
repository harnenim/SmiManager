using System;
using System.Collections.Generic;
using Subtitle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmiManager
{
    public partial class MainForm : WebForm.WebForm
    {
        #region 초기화

        public MainForm()
        {
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Location = new System.Drawing.Point(554, 0);
            ClientSize = new System.Drawing.Size(1360, 749);
            Name = Text = "Smi Manager";
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
            List<Smi> smis = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " > textarea.input" })).body;
            Smi.FillEmptySync(smis);
            SetPreview(id, smis, 100);
            string output = new SmiFile() { body = smis }.ToTxt();
            Script("$val", new object[] { "#" + id + " > textarea.output", output });
        }

        public void Normalize(string id)
        {
            List<Smi> smis = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " > textarea.input" })).body;
            Smi.Normalize(smis);
            SetPreview(id, smis, 100);
            string output = new SmiFile() { body = smis }.ToTxt();
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

        public void Combine(string id)
        {
            List<Smi> inputUpper = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " textarea.inputUpper" })).body;
            List<Smi> inputLower = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " textarea.inputLower" })).body;
            List<Smi> result = Combiner.Combine(inputUpper, inputLower);
            SetPreview(id, result, 100);
            string output = new SmiFile() { body = result }.ToTxt();
            Script("$val", new object[] { "#" + id + " > textarea.output", output });
        }

        public void Devide(string id)
        {
            List<Smi> input = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " textarea.input" })).body;
            Tuple<List<Smi>, List<Smi>> result = Devider.Devide(input);
            Script("$val", new object[] { "#" + id + " > textarea.outputUpper", Smi.Smi2txt(result.Item1) });
            Script("$val", new object[] { "#" + id + " > textarea.outputLower", Smi.Smi2txt(result.Item2) });
        }

        public void Shake(string id)
        {
            List<Smi> input = SmiFile.FromTxt(Script("$val", new object[] { "#" + id + " textarea.input" })).body;
            List<Smi> result = Shaker.Shake(input);
            SetPreview(id, result);
            Script("$val", new object[] { "#" + id + " > textarea.output", Smi.Smi2txt(result) });
        }

        public void Nico(string id)
        {
            JObject nicos = JObject.Parse(Script("getNicoValue", new object[] { id }));
            int width = Convert.ToInt32(nicos["width"].ToString());
            JArray values = (JArray)nicos["value"];

            List<NicoNico.NicoLine> nicoLines = new List<NicoNico.NicoLine>();
            foreach (JToken value in values)
            {
                int   line  = value["line" ].ToObject<int>();
                int   start = value["start"].ToObject<int>();
                int   end   = value["end"  ].ToObject<int>();
                string text = value["text" ].ToString();
                if (start >= end) continue;

                nicoLines.Add(new NicoNico.NicoLine()
                {
                    line  = line,
                    start = start,
                    end   = end,
                    text  = text,
                    width = Smi.GetLineWidth(text),
                });
            }
            nicoLines.Sort((a, b) => (a.start - b.start));

            List<Smi> result = NicoNico.Nico(nicoLines, width);
            SetPreview(id, result);
            Script("$val", new object[] { "#" + id + " > textarea.output", Smi.Smi2txt(result) });
        }

        // 어쩌다 보니 급조한 부분이긴 한데 음...
        private void SetPreview(string id, List<Smi> smis)
        {
            SetPreview(id, smis, 42);
        }
        private void SetPreview(string id, List<Smi> smis, int interval)
        {
            List<string> previews = new List<string>();
            foreach (Smi smi in smis)
            {
                List<Converter.Span> spans = Converter.Span.FromSmi(smi.text);
                string html = Converter.Span.ToHtml(spans);
                Console.WriteLine(html);
                previews.Add(html);
            }
            string json = JsonConvert.SerializeObject(previews);
            Script("setPreview", new object[] { id, json, interval });
        }
    }
}
