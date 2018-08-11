using System.Collections.Generic;
using Subtitle;

namespace SmiManager
{
    static class Combiner
    {
        public class Combined
        {
            public int start = 0;
            public SyncType syncType = SyncType.normal;
            public string[] upper = new string[] { };
            public string[] lower = new string[] { };
            public static string[] ToLines(string smi)
            {
                return smi.Replace("​", "")
                    .Replace("\r\n", "")
                    .Replace("\n", "")
                    .Replace("<BR>", "\n")
                    .Replace("<Br>", "\n")
                    .Replace("<bR>", "\n")
                    .Replace("<br>", "\n")
                    .Split('\n');
            }
        }
        private class CombinedGroup
        {
            public int upperLineCount = 0;
            public int lowerLineCount = 0;
            public List<Combined> combineds = new List<Combined>();
        }

        public static List<Smi> Combine(List<Smi> inputUpper, List<Smi> inputLower)
        {
            List<CombinedGroup> groups = new List<CombinedGroup>();
            CombinedGroup lastGroup = new CombinedGroup();
            Combined last = new Combined();
            int ui = 0, li = 0;
            while (ui < inputUpper.Count && li < inputLower.Count)
            {
                Smi uLine = inputUpper[ui];
                Smi lLine = inputLower[li];

                int comp = uLine.start - lLine.start;
                if (comp < 0)
                {
                    // 위 바뀜

                    if (last.lower.Length == 0)
                        // 아래가 빈 싱크인 상태로 위가 바뀜 -> 새 그룹
                        groups.Add((lastGroup = new CombinedGroup()));

                    lastGroup.combineds.Add((last = new Combined()
                    {
                        start = uLine.start,
                        syncType = uLine.syncType,
                        upper = uLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(uLine.text),
                        lower = (string[])last.lower.Clone()
                    }));
                    if (last.upper.Length > lastGroup.upperLineCount)
                        lastGroup.upperLineCount = last.upper.Length;

                    ui++;
                }
                else if (comp > 0)
                {
                    // 아래 바뀜

                    if (last.upper.Length == 0)
                        // 위가 빈 싱크인 상태로 위가 바뀜 -> 새 그룹
                        groups.Add((lastGroup = new CombinedGroup()));

                    lastGroup.combineds.Add((last = new Combined()
                    {
                        start = lLine.start,
                        syncType = lLine.syncType,
                        upper = (string[])last.upper.Clone(),
                        lower = lLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(lLine.text)
                    }));
                    if (last.lower.Length > lastGroup.lowerLineCount)
                        lastGroup.lowerLineCount = last.lower.Length;

                    li++;
                }
                else
                {
                    // 동시

                    // 새 그룹
                    groups.Add((lastGroup = new CombinedGroup()));

                    lastGroup.combineds.Add((last = new Combined()
                    {
                        start = uLine.start,
                        syncType = (uLine.syncType == SyncType.frame || lLine.syncType == SyncType.frame) ? SyncType.frame : SyncType.normal,
                        upper = uLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(uLine.text),
                        lower = lLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(lLine.text)
                    }));
                    lastGroup.upperLineCount = last.upper.Length;
                    lastGroup.lowerLineCount = last.lower.Length;

                    ui++; li++;
                }
            }
            for (; ui < inputUpper.Count; ui++)
            {
                Smi uLine = inputUpper[ui];

                if (last.lower.Length == 0)
                    // 아래가 빈 싱크인 상태로 위가 바뀜 -> 새 그룹
                    groups.Add((lastGroup = new CombinedGroup()));

                lastGroup.combineds.Add((last = new Combined()
                {
                    start = uLine.start,
                    syncType = uLine.syncType,
                    upper = uLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(uLine.text),
                    lower = (string[])last.lower.Clone()
                }));
                if (last.upper.Length > lastGroup.upperLineCount)
                    lastGroup.upperLineCount = last.upper.Length;
            }
            for (; li < inputLower.Count; li++)
            {
                Smi lLine = inputLower[li];

                if (last.upper.Length == 0)
                    // 위가 빈 싱크인 상태로 위가 바뀜 -> 새 그룹
                    groups.Add((lastGroup = new CombinedGroup()));

                lastGroup.combineds.Add((last = new Combined()
                {
                    start = lLine.start,
                    syncType = lLine.syncType,
                    upper = (string[])last.upper.Clone(),
                    lower = lLine.text.Replace("&nbsp;", "").Length == 0 ? new string[] { } : Combined.ToLines(lLine.text)
                }));
                if (last.lower.Length > lastGroup.lowerLineCount)
                    lastGroup.lowerLineCount = last.lower.Length;
            }

            List<Smi> result = new List<Smi>();
            foreach (CombinedGroup group in groups)
            {
                foreach (Combined combined in group.combineds)
                {
                    int maxUpperIndex = 0, maxLowerIndex = 0;
                    float maxUpperWidth = 0, maxLowerWidth = 0;
                    for (int i = 0; i < combined.upper.Length; i++)
                    {
                        float width = Smi.GetLineWidth(combined.upper[i]);
                        if (width > maxUpperWidth)
                        {
                            maxUpperIndex = i;
                            maxUpperWidth = width;
                        }
                    }
                    for (int i = 0; i < combined.lower.Length; i++)
                    {
                        float width = Smi.GetLineWidth(combined.lower[i]);
                        if (width > maxLowerWidth)
                        {
                            maxLowerIndex = i;
                            maxLowerWidth = width;
                        }
                    }

                    // 정렬용 글자 수 맞춰주기
                    if (combined.upper.Length > 0 && combined.lower.Length > 0)
                    {
                        if (maxUpperWidth > maxLowerWidth)
                        {
                            //string left = Smi.GetLineAppend(combined.lower[maxLowerIndex], maxUpperWidth, true);
                            string left = Width.GetAppend(Smi.GetLineWidth(combined.lower[maxLowerIndex]), maxUpperWidth, true);
                            if (left.Length > 0)
                            {
                                string right = Width.AppendToRight(left);
                                for (int i = 0; i < combined.lower.Length; i++)
                                    //combined.lower[i] = "<b></b>" + left + combined.lower[i] + right + "<b></b>";
                                    combined.lower[i] = left + combined.lower[i] + right;
                            }
                        }
                        else
                        {
                            //string left = Smi.GetLineAppend(combined.upper[maxUpperIndex], maxLowerWidth, true);
                            string left = Width.GetAppend(Smi.GetLineWidth(combined.upper[maxUpperIndex]), maxLowerWidth, true);
                            if (left.Length > 0)
                            {
                                string right = Width.AppendToRight(left);
                                for (int i = 0; i < combined.upper.Length; i++)
                                    //combined.upper[i] = "<b></b>" + left + combined.upper[i] + right + "<b></b>";
                                    combined.upper[i] = left + combined.upper[i] + right;
                            }
                        }
                    }

                    Smi smi = new Smi()
                    {
                        start = combined.start,
                        syncType = combined.syncType,
                        text = ""
                    };
                    for (int i = 0; i < group.upperLineCount - combined.upper.Length; i++)
                        smi.text += "<b>　</b>" + (i < group.upperLineCount - 1 ? "<br>" : "");
                    for (int i = 0; i < combined.upper.Length; i++)
                        smi.text += combined.upper[i] + (i < combined.upper.Length - 1 ? "<br>" : "");
                    if (group.upperLineCount > 0 && group.lowerLineCount > 0)
                        smi.text += "<br>";
                    for (int i = 0; i < group.lowerLineCount - combined.lower.Length; i++)
                        smi.text += "<b>　</b>" + (i < (group.lowerLineCount - 1) ? "<br>" : "");
                    for (int i = 0; i < combined.lower.Length; i++)
                        smi.text += combined.lower[i] + (i < (combined.lower.Length - 1) ? "<br>" : "");

                    if (smi.text.Length == 0)
                        smi.text = "&nbsp;";
                    else
                    {
                        if (smi.text.StartsWith("　") || smi.text.StartsWith(" "))
                            smi.text = "​" + smi.text;
                        if (smi.text.EndsWith("　") || smi.text.EndsWith(" "))
                            smi.text = smi.text + "​";
                    }

                    result.Add(smi);
                }
            }
            return result;
        }
    }
}
