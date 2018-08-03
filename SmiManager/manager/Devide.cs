using System;
using System.Collections.Generic;
using Subtitle;

namespace SmiManager
{
    static class Devider
    {
        private class LinePadding
        {
            public string text = "";
            public int padding = 0;

            public LinePadding(string text)
            {
                if (text.Equals("<b>　</b>") || text.Equals("&nbsp;"))
                {
                    this.text = null;
                    return;
                }
                this.text = text;
                text = text.Replace("<b></b>", "").Replace("<B></B>", "").Replace("<i></i>", "").Replace("<I></I>", "");
                int l = 0, r = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                        l++;
                    else if (text[i] == '　')
                        l += 3;
                    else
                        break;
                }
                for (int i = text.Length - 1; i >= 0; i--)
                {
                    if (text[i] == ' ')
                        r++;
                    else if (text[i] == '　')
                        r += 3;
                    else
                        break;
                }
                padding = Math.Min(l, r);
                int j; l = 0; r = 0;
                {
                    for (j = 0; l < padding && l < padding; j++)
                    {
                        if (text[j] == ' ')
                            l++;
                        else if (text[j] == '　')
                            l += 3;
                    }
                    text = text.Substring(j);
                    while (l > padding)
                    {
                        text = " " + text;
                        l--;
                    }
                }
                {
                    for (j = text.Length - 1; j >= 0 && r < padding; j--)
                    {
                        if (text[j] == ' ')
                            r++;
                        else if (text[j] == '　')
                            r += 3;
                    }
                    text = text.Substring(0, j + 1);
                    while (r > padding)
                    {
                        text = text + " ";
                        r--;
                    }
                }
                this.text = text;
            }
        }
        private class SmiPadding
        {
            public int start = 0;
            public Smi.SyncType syncType = Smi.SyncType.normal;
            public LinePadding[] lines = new LinePadding[] { };

            public SmiPadding(Smi smi)
            {
                start = smi.start;
                syncType = smi.syncType;

                string[] lines = Combiner.Combined.ToLines(smi.text);
                this.lines = new LinePadding[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                {
                    this.lines[i] = new LinePadding(lines[i]);
                }
            }
        }

        private static Tuple<List<Smi>, List<Smi>> DevideGroup(List<SmiPadding> smis, int lineCount)
        {
            List<Smi> smiUpper = new List<Smi>();
            List<Smi> smiLower = new List<Smi>();

            // 모든 줄 내용이 바뀐 경우 그룹 잘라서 처리
            SmiPadding bfr = smis[0];
            List<SmiPadding> splitted = new List<SmiPadding>() { bfr };
            for (int i = 1; i < smis.Count; i++)
            {
                SmiPadding smi = smis[i];
                bool isSplit = true;
                for (int j = 0; j < lineCount; j++)
                {
                    if (smi.lines[j] != null && !smi.lines[j].Equals(bfr.lines[j]))
                    {
                        isSplit = false;
                        break;
                    }
                }
                if (isSplit)
                {
                    Tuple<List<Smi>, List<Smi>> devided = DevideLine(splitted, lineCount);
                    smiUpper.AddRange(devided.Item1);
                    smiLower.AddRange(devided.Item2);
                    splitted = new List<SmiPadding> { smi };
                }
                else
                {
                    splitted.Add(smi);
                }
                bfr = smi;
            }
            {
                Tuple<List<Smi>, List<Smi>> devided = DevideLine(splitted, lineCount);
                smiUpper.AddRange(devided.Item1);
                smiLower.AddRange(devided.Item2);
            }

            return new Tuple<List<Smi>, List<Smi>>(smiUpper, smiLower);
        }
        private static Tuple<List<Smi>, List<Smi>> DevideLine(List<SmiPadding> smis, int lineCount)
        {
            List<Smi> outputUpper = new List<Smi>();
            List<Smi> outputLower = new List<Smi>();

            if (lineCount == 1)
            {
                foreach (SmiPadding smi in smis)
                {
                    outputLower.Add(new Smi()
                    {
                        start = smi.start,
                        syncType = smi.syncType,
                        text = smi.lines[0].text ?? "&nbsp;"
                    });
                }
            }
            else
            {
                int upperLineCount = 0;
                foreach (SmiPadding smi in smis)
                {
                    int padding = smi.lines[lineCount - 1].padding;
                    for (int i = lineCount - 2; i >= upperLineCount; i--)
                    {
                        LinePadding line = smi.lines[i];
                        if (line.text != null && line.padding != padding)
                        {
                            upperLineCount = i + 1;
                            break;
                        }
                    }
                }
                foreach (SmiPadding smi in smis)
                {
                    string upperText = null, lowerText = null;
                    for (int i = 0; i < upperLineCount; i++)
                    {
                        if (smi.lines[i].text != null)
                            upperText = (upperText == null ? "" : upperText + "<br>") + smi.lines[i].text;
                    }
                    for (int i = upperLineCount; i < lineCount; i++)
                    {
                        if (smi.lines[i].text != null)
                            lowerText = (lowerText == null ? "" : lowerText + "<br>") + smi.lines[i].text;
                    }

                    if (outputUpper.Count > 0)
                    {
                        if (upperText == null) upperText = "&nbsp;";
                        if (upperText.StartsWith(" ") || upperText.StartsWith("　"))
                            upperText = "​" + upperText;
                        if (upperText.EndsWith(" ") || upperText.EndsWith("　"))
                            upperText += "​";
                        if (!upperText.Equals(outputUpper[outputUpper.Count - 1].text))
                        {
                            outputUpper.Add(new Smi()
                            {
                                start = smi.start,
                                syncType = smi.syncType,
                                text = upperText
                            });
                        }
                    }
                    else
                    {
                        if (upperText != null)
                        {
                            if (upperText.StartsWith(" ") || upperText.StartsWith("　"))
                                upperText = "​" + upperText;
                            if (upperText.EndsWith(" ") || upperText.EndsWith("　"))
                                upperText += "​";
                            outputUpper.Add(new Smi()
                            {
                                start = smi.start,
                                syncType = smi.syncType,
                                text = upperText
                            });
                        }
                    }
                    if (outputLower.Count > 0)
                    {
                        if (lowerText == null) lowerText = "&nbsp;";
                        if (lowerText.StartsWith(" ") || lowerText.StartsWith("　"))
                            upperText = "​" + upperText;
                        if (lowerText.EndsWith(" ") || lowerText.EndsWith("　"))
                            upperText += "​";
                        if (!lowerText.Equals(outputLower[outputLower.Count - 1].text))
                        {
                            outputLower.Add(new Smi()
                            {
                                start = smi.start,
                                syncType = smi.syncType,
                                text = lowerText
                            });
                        }

                    }
                    else
                    {
                        if (lowerText != null)
                        {
                            if (lowerText.StartsWith(" ") || lowerText.StartsWith("　"))
                                upperText = "​" + upperText;
                            if (lowerText.EndsWith(" ") || lowerText.EndsWith("　"))
                                upperText += "​";
                            outputLower.Add(new Smi()
                            {
                                start = smi.start,
                                syncType = smi.syncType,
                                text = lowerText
                            });
                        }
                    }
                }
            }

            return new Tuple<List<Smi>, List<Smi>>(outputUpper, outputLower);
        }

        public static Tuple<List<Smi>, List<Smi>> Devide(List<Smi> input)
        {
            List<SmiPadding> smis = new List<SmiPadding>();
            foreach (Smi smi in input)
                smis.Add(new SmiPadding(smi));

            List<Smi> smiUpper = new List<Smi>();
            List<Smi> smiLower = new List<Smi>();

            // 줄 수가 같은 것 단위로 나눠서 처리
            SmiPadding bfr = smis[0];
            List<SmiPadding> splitted = new List<SmiPadding>() { bfr };
            for (int i = 1; i < smis.Count; i++)
            {
                SmiPadding smi = smis[i];
                if (smi.lines.Length == bfr.lines.Length)
                {
                    splitted.Add(smi);
                }
                else
                {
                    Tuple<List<Smi>, List<Smi>> devided = DevideGroup(splitted, bfr.lines.Length);
                    smiUpper.AddRange(devided.Item1);
                    smiLower.AddRange(devided.Item2);
                    splitted = new List<SmiPadding> { smi };
                }
                bfr = smi;
            }
            {
                Tuple<List<Smi>, List<Smi>> devided = DevideGroup(splitted, bfr.lines.Length);
                smiUpper.AddRange(devided.Item1);
                smiLower.AddRange(devided.Item2);
            }

            return new Tuple<List<Smi>, List<Smi>>(smiUpper, smiLower);
        }
    }
}
