using System;
using Subtitle;

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


        public int CalcWidth(string str)
        {
            return Convert.ToInt32(Script("calcWidth", new object[] { str }));
        }
        public int[] CalcWidth(string[] lines)
        {
            int[] widths = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                widths[i] = CalcWidth(lines[i]);
            return widths;
        }


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
    /*
    double subWidth(char c){
        if(c>='가' && c<='힣'
        || c>='ᄀ' && c<='ᇹ'
        || c>='⺀' && c<='龻'
        || c>='' && c<='頻')
            return 3;
        if(c=='i' || c=='j' || c=='l')
            return 6/7;
        if(c=='I')
            return 1;
        if(c=='f' || c=='r' || c=='t')
            return 8/7;
        if(c=='J')
            return 9/7;
        if(c=='c' || c=='s' || c=='z')
            return 10/7;
        if(c=='E' || c=='F' || c=='L'
             || c=='a' || c=='v' || c=='x' || c=='y')
            return 11/7;
        if(c=='T' || c=='e' || c=='k')
            return 12/7;
        if(c=='B' || c=='C' || c=='P' || c=='Y' || c=='Z'
             || c=='b' || c=='d' || c=='h' || c=='n' || c=='o'
             || c=='p' || c=='q' || c=='u')
            return 13/7;
        if(c=='K' || c=='R' || c=='V' || c=='X')
            return 2;
        if(c=='A' || c=='G' || c=='U')
            return 15/7;
        if(c=='D' || c=='H' || c=='O' || c=='Q')
            return 16/7;
        if(c=='N' || c=='w')
            return 17/7;
        if(c=='m')
            return 19/7;
        if(c=='M')
            return 20/7;
        if(c=='W')
            return 3;
        if(c=='S')
            return 27/16;
        return 1;
    }
    */
    /*
    double subWidth(String input){
        char c[] = input.toCharArray();
        int sum = 0;
        for(int i=0; i<c.length; i++)
            sum += subWidth(c[i]);
        return sum;
    }
    */
    /*
    String whiteSpace(double width){
        if(width<0.5)
            return "";

        String result = "";
        while(width>=2.5){
            result += "　";
            width -= 3;
        }
        while(width>=0.5){
            result = " " + result;
            width--;
        }
        return result + "&lt;b>&lt;/b>";
    }
*/
    /*
    interface nc{
        String fc(String result, char buff, double full_length);
    }
    interface nv{
        String fv(String result);
    }
    */
    /*
    public class cursor{
        nc f;
        public cursor(int m){
            if(m==1){
                f = new nc(){
                    public String fc(String result, char buff, double full_length){
                        if(buff>='ㄱ' && buff<='ㅎ'
                        || buff>='ㅏ' && buff<='ㅣ'
                        || buff>='가' && buff<='힣')
                            return result + "<u>"+buff+"</u>"
                                + whiteSpace(full_length-subWidth(result+buff));
                        else
                            return result + buff + whiteSpace(full_length-subWidth(result+buff));
                    }
                };
            }else if(m==2){
                f = new nc(){
                    public String fc(String result, char buff, double full_length){
                        return result + "<u>"+buff+"</u>"
                            + whiteSpace(full_length+1-subWidth(result+buff));
                    }
                };
            }else{
                f = new nc(){
                    public String fc(String result, char buff, double full_length){
                        return result + buff + whiteSpace(full_length-subWidth(result+buff));
                    }
                };
            }
        }
    }
    */
    /*
    public class viewHTML{
        nv f;
        public viewHTML(int m){
            if(m==0){
                f = new nv(){
                    public String fv(String result){
                        return result.replace("<", "&lt;");
                    }
                };
            }else{
                f = new nv(){
                    public String fv(String result){
                        return result;
                    }
                };
            }
        }
    }
    */
    /*

    %><!doctype html>
    <html>
    <head>
    <style>
    * { font-family:맑은 고딕; }
    </style>
    </head>
    <body><%

    */
    /*
    request.setCharacterEncoding("UTF-8");

    String text = "싫쇄롻튄즤륢 ㅁㄴㅇㄻㄴㅇㄹ";
    if(request.getParameter("text")!=null)
        text = request.getParameter("text").trim();

    int cs = 2;
    if(request.getParameter("cs")!=null)
        cs = Integer.parseInt(request.getParameter("cs"));
    cursor m = new cursor(cs);

    int view = 0;
    if(request.getParameter("view")!=null)
        view = Integer.parseInt(request.getParameter("view"));
    viewHTML v = new viewHTML(view);

    int typing = 0;
    if(request.getParameter("typing")!=null && request.getParameter("typing").equals("1"))
        typing = 1;

    int start = 1000;
    if(request.getParameter("start")!=null)
        start = Integer.parseInt(request.getParameter("start"));

    int end = 2500;
    if(request.getParameter("end")!=null)
        end = Integer.parseInt(request.getParameter("end"));
    %>
    <form action="typing.jsp" method="post">
        <select name="typing">
            <option value="0"<% if(typing==0) out.write(" selected"); %>>컴퓨터</option>
            <option value="1"<% if(typing==1) out.write(" selected"); %>>타자기</option>
        </select>
        <select name="view">
            <option value="0"<% if(view==0) out.write(" selected"); %>>태그 보이기</option>
            <option value="1"<% if(view==1) out.write(" selected"); %>>결과값 보기</option>
        </select>
        <select name="cs">
            <option value="0"<% if(cs==0) out.write(" selected"); %>>커서X</option>
            <option value="1"<% if(cs==1) out.write(" selected"); %>>한글에만 커서</option>
            <option value="2"<% if(cs==2) out.write(" selected"); %>>입력 커서 형태</option>
        </select>
        <br />
        시작 싱크 : <input type="text" name="start" value="<%=start%>" /><br />
        종료 싱크 : <input type="text" name="end"   value="<%=end  %>" /><br />
        <input type="text" name="text" style="width:200px;" value="<%=text%>" />
        <button type="submit">변환</button>
    </form>
    <pre><%
    double full_length = subWidth(text);
    String result = "";
    char buff = ' ';
    List<String> resultList = new ArrayList<String>();
    String buffList = "";

    */
    /*
    if(typing==1){
        char cho[] = {'ㄱ','ㄲ','ㄴ','ㄷ','ㄸ','ㄹ','ㅁ','ㅂ','ㅃ','ㅅ','ㅆ','ㅇ','ㅈ','ㅉ','ㅊ','ㅋ','ㅌ','ㅍ','ㅎ'};
        char type[] = text.toCharArray();
        for(int i=0; i<type.length; i++){
            result += (buff==' ')? "" : buff;
            char c[] = to_johap(type[i]+"").toCharArray();
            if(c[0]>='ᄀ' && c[0]<='ᄒ'){
                buff = cho[c[0]-'ᄀ'];

                if(c.length>1 && c[1]>='ᅡ' && c[1]<='ᅵ'){
                    resultList.add(result); buffList+=buff;
                    buff = (char)('가'+(c[0]-'ᄀ')*('까'-'가')+(c[1]-'ᅡ')*('개'-'가'));

                    if(c.length>2 && c[2]>='ᆨ' && c[2]<='ᇂ'){
                        resultList.add(result); buffList+=buff;
                        buff = (char)(buff+(c[2]-'ᆨ')+1);
                    }
                }

            }else if(c[0]>='ᅡ' && c[0]<='ᅵ' // 조합형 중성
                    || c[0]>='ᆨ' && c[0]<='ᇂ' // 조합형 종성
                    || c[0]>='ㄱ' && c[0]<='ㅎ' // 완성형 자모
                    || c[0]>='ㅏ' && c[0]<='ㅣ'){
                buff = c[0];

            }else{
                buff = ' ';
                result += c[0];
            }
            resultList.add(result); buffList+=buff;
        }

    */
    /*
    }else{
        char type[] = toType(text).toCharArray();
        for(int i=0; i<type.length; i++){
            while(true){
                if(type[i]>='ㄱ' && type[i]<='ㅎ'){ // 자음
                    // 합쳐질 수 있는 자음
                    if(buff=='ㄱ' && type[i]=='ㅅ'){
                        buff = 'ㄳ';
                        break;
                    }else if(buff=='ㄴ'){
                        if(type[i]=='ㅈ'){
                            buff = 'ㄵ';
                            break;
                        }else if(type[i]=='ㅎ'){
                            buff = 'ㄶ';
                            break;
                        }
                    }else if(buff=='ㄹ'){
                        if(type[i]=='ㄱ'){
                            buff = 'ㄺ';
                            break;
                        }else if(type[i]=='ㅁ'){
                            buff = 'ㄻ';
                            break;
                        }else if(type[i]=='ㅂ'){
                            buff = 'ㄼ';
                            break;
                        }else if(type[i]=='ㅅ'){
                            buff = 'ㄽ';
                            break;
                        }else if(type[i]=='ㅌ'){
                            buff = 'ㄾ';
                            break;
                        }else if(type[i]=='ㅍ'){
                            buff = 'ㄿ';
                            break;
                        }else if(type[i]=='ㅎ'){
                            buff = 'ㅀ';
                            break;
                        }
                    }else if(buff=='ㅂ' && type[i]=='ㅅ'){
                        buff = 'ㅄ';
                        break;

                    }else if(buff>='가' && buff<='힣'){ // 한글 글자가 있을 경우
                        if((buff-'가')%('개'-'가')==0){ // 받침 없는 글자가 있을 경우
                            if(nJong(type[i])>0){ // 받침이 가능한 경우
                                buff = (char)(buff + nJong(type[i]));
                            }else{ // 받침이 불가능한 경우
                                result += buff;
                                buff = type[i];
                            }
                            break;
                        }else if((buff-'가')%('개'-'가')==1 && type[i]=='ㅅ'){ // 받침 ㄱ+ㅅ
                            buff += 2;
                            break;
                        }else if((buff-'가')%('개'-'가')==4){ // 받침 ㄴ
                            if(type[i]=='ㅈ'){
                                buff += 1;
                                break;
                            }else if(type[i]=='ㅎ'){
                                buff += 2;
                                break;
                            }
                        }else if((buff-'가')%('개'-'가')==8){ // 받침 ㄹ
                            if(type[i]=='ㄱ'){
                                buff += 1;
                                break;
                            }else if(type[i]=='ㅁ'){
                                buff += 2;
                                break;
                            }else if(type[i]=='ㅂ'){
                                buff += 3;
                                break;
                            }else if(type[i]=='ㅅ'){
                                buff += 4;
                                break;
                            }else if(type[i]=='ㅌ'){
                                buff += 5;
                                break;
                            }else if(type[i]=='ㅍ'){
                                buff += 6;
                                break;
                            }else if(type[i]=='ㅎ'){
                                buff += 7;
                                break;
                            }
                        }else if((buff-'가')%('개'-'가')==17 && type[i]=='ㅅ'){ // 받침 ㅂ+ㅅ
                            buff += 1;
                            break;
                        }
                    }
                }else if(type[i]>='ㅏ' && type[i]<='ㅣ'){ // 모음
                    switch(buff){
                        // 버퍼에 합성 자음일 경우 미리 분리
                        case 'ㄳ' : result += 'ㄱ'; buff='ㅅ'; break;
                        case 'ㄵ' : result += 'ㄴ'; buff='ㅈ'; break;
                        case 'ㄶ' : result += 'ㄴ'; buff='ㅎ'; break;
                        case 'ㄺ' : result += 'ㄹ'; buff='ㄱ'; break;
                        case 'ㄻ' : result += 'ㄹ'; buff='ㅁ'; break;
                        case 'ㄼ' : result += 'ㄹ'; buff='ㅂ'; break;
                        case 'ㄽ' : result += 'ㄹ'; buff='ㅅ'; break;
                        case 'ㄾ' : result += 'ㄹ'; buff='ㅌ'; break;
                        case 'ㄿ' : result += 'ㄹ'; buff='ㅍ'; break;
                        case 'ㅀ' : result += 'ㄹ'; buff='ㅎ'; break;
                        case 'ㅄ' : result += 'ㅂ'; buff='ㅅ'; break;
                        default : {
                            // 버퍼에 받침 있는 글자일 경우 미리 분리
                            if((buff-'가')%('개'-'가')>0){
                                int jong = (buff-'가')%('개'-'가');
                                switch(jong){
                                    case  1 : // ㄱ
                                        result+=(char)(buff-1);  buff='ㄱ'; break;
                                    case  2 : // ㄲ
                                        result+=(char)(buff-2);  buff='ㄲ'; break;
                                    case  3 : // ㄳ
                                        result+=(char)(buff-2);  buff='ㅅ'; break;
                                    case  4 : // ㄴ
                                        result+=(char)(buff-4);  buff='ㄴ'; break;
                                    case  5 : // ㄵ
                                        result+=(char)(buff-1);  buff='ㅈ'; break;
                                    case  6 : // ㄶ
                                        result+=(char)(buff-2);  buff='ㅎ'; break;
                                    case  7 : // ㄷ
                                        result+=(char)(buff-7);  buff='ㄷ'; break;
                                    case  8 : // ㄹ
                                        result+=(char)(buff-8);  buff='ㄹ'; break;
                                    case  9 : // ㄺ
                                        result+=(char)(buff-1);  buff='ㄱ'; break;
                                    case 10 : // ㄻ
                                        result+=(char)(buff-2);  buff='ㅁ'; break;
                                    case 11 : // ㄼ
                                        result+=(char)(buff-3);  buff='ㅂ'; break;
                                    case 12 : // ㄽ
                                        result+=(char)(buff-4);  buff='ㅅ'; break;
                                    case 13 : // ㄾ
                                        result+=(char)(buff-5);  buff='ㅌ'; break;
                                    case 14 : // ㄿ
                                        result+=(char)(buff-6);  buff='ㅍ'; break;
                                    case 15 : // ㅀ
                                        result+=(char)(buff-7);  buff='ㅎ'; break;
                                    case 16 : // ㅁ
                                        result+=(char)(buff-16); buff='ㅁ'; break;
                                    case 17 : // ㅂ
                                        result+=(char)(buff-17); buff='ㅂ'; break;
                                    case 18 : // ㅄ
                                        result+=(char)(buff-1);  buff='ㅅ'; break;
                                    case 19 : // ㅅ
                                        result+=(char)(buff-19); buff='ㅅ'; break;
                                    case 20 : // ㅆ
                                        result+=(char)(buff-20); buff='ㅆ'; break;
                                    case 21 : // ㅇ
                                        result+=(char)(buff-21); buff='ㅇ'; break;
                                    case 22 : // ㅈ
                                        result+=(char)(buff-22); buff='ㅈ'; break;
                                    case 23 : // ㅊ
                                        result+=(char)(buff-23); buff='ㅊ'; break;
                                    case 24 : // ㅋ
                                        result+=(char)(buff-24); buff='ㅋ'; break;
                                    case 25 : // ㅌ
                                        result+=(char)(buff-25); buff='ㅌ'; break;
                                    case 26 : // ㅍ
                                        result+=(char)(buff-26); buff='ㅍ'; break;
                                    case 27 : // ㅎ
                                        result+=(char)(buff-27); buff='ㅎ'; break;
                                    default : // 존재해선 안 됨
                                        break;
                                }
                            }
                        }
                    }

                    if(buff>='ㄱ' && buff<='ㅎ'){ // 버퍼에 자음이 있을 경우
                        buff = (char)('가' + nCho(buff)*('까'-'가') + (type[i]-'ㅏ')*('개'-'가'));
                        break;

                    }else if(buff>='ㅏ' && buff<='ㅣ'){ // 버퍼에 모음이 있을 경우
                        if(buff=='ㅗ'){
                            if(type[i]=='ㅏ'){
                                buff = 'ㅘ';
                                break;
                            }else if(type[i]=='ㅐ'){
                                buff = 'ㅙ';
                                break;
                            }else if(type[i]=='ㅣ'){
                                buff = 'ㅚ';
                                break;
                            }
                        }else if(buff=='ㅜ'){
                            if(type[i]=='ㅓ'){
                                buff = 'ㅝ';
                                break;
                            }else if(type[i]=='ㅔ'){
                                buff = 'ㅞ';
                                break;
                            }else if(type[i]=='ㅣ'){
                                buff = 'ㅟ';
                                break;
                            }
                        }else if(buff=='ㅡ' && type[i]=='ㅣ'){
                            buff = 'ㅢ';
                            break;
                        }
                    }else if((buff-'가')%('개'-'가')==0){ // 받침이 없을 경우
                        if(((buff-'가')/('개'-'가'))%(('까'-'가')/('개'-'가'))
                                == ('고'-'가')/('개'-'가')){
                            if(type[i]=='ㅏ'){
                                buff += '과'-'고';
                                break;
                            }else if(type[i]=='ㅐ'){
                                buff += '괘'-'고';
                                break;
                            }else if(type[i]=='ㅣ'){
                                buff += '괴'-'고';
                                break;
                            }
                        }else if(((buff-'가')/('개'-'가'))%(('까'-'가')/('개'-'가'))
                                == ('구'-'가')/('개'-'가')){
                            if(type[i]=='ㅓ'){
                                buff += '궈'-'구';
                                break;
                            }else if(type[i]=='ㅔ'){
                                buff += '궤'-'구';
                                break;
                            }else if(type[i]=='ㅣ'){
                                buff += '귀'-'구';
                                break;
                            }
                        }else if(((buff-'가')/('개'-'가'))%(('까'-'가')/('개'-'가'))
                                == ('그'-'가')/('개'-'가') && type[i]=='ㅣ'){
                            buff += '긔'-'그';
                            break;
                        }
                    }
                }else{ // 한글이 아님
                    result += (buff==' ')? type[i] : buff+""+type[i];
                    buff = ' ';
                    break;
                }

                // 위에서 처리 안 된 경우
                result += (buff==' ')? "" : buff;
                buff = type[i];
                break;
            }
            resultList.add(result); buffList+=buff;
        }
    }
    */
    /*
    for(int i=0; i<resultList.size(); i++){
        out.write("&lt;Sync Start="+(start+(end-start)*i/(resultList.size()-1))+">&lt;P Class=KRCC>\n");
        out.write(v.f.fv(m.f.fc(resultList.get(i), buffList.toCharArray()[i], full_length)) + "\n");
    }


    %></pre>
    </body>
    </html>
            */
}
