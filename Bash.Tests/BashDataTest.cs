using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bash.App.Models;
using System.Linq;

namespace Bash.Tests
{
    [TestClass]
    public class BashDataTest
    {
        [TestMethod]
        public void BashDataSimple()
        {
            string text = "<i8b4uUnderground> d-_-b[newline]" + 
                          "<BonyNoMore> wie machst du das spiegelverkehrte b?[newline]" +
                          "<BonyNoMore> warte[newline]" +
                          "<BonyNoMore> vergiss es";
            PerformDataTest(text, 4, 2);
        }

        [TestMethod]
        public void BashDataComplexWithServerContentInBetween()
        {
            string text = "<Backpfeiffe> so mädels, jetz is schluss![newline]" +
                "<Backpfeiffe> da hier einige nicht ohne vulgärsprache auskommen, hab ich n bot geschrieben[newline]" +
                "<Tumanual> why? geht doch auch ohne :([newline]" +
                "<sec> freak![newline]" +
                "<Backpfeiffe> na wenn ich off bin, gibts kein ban[newline]" +
                "<Backpfeiffe> wenn ich euch nur die 1h kicke, dann seid ihr da meist eh afk[newline]" +
                "<Backpfeiffe> und ständig ne zeit in das feld tippen... hab ich auch kein bock![newline]" +
                "<Backpfeiffe> deswegen bot: 24h aktiv und arbeitet selbstständig :)[newline]" +
                "<Backpfeiffe> mein sklave^^[newline]" + 
                "<Backpfeiffe> für 1 \"böses wort\" werdet ihr absofort 1 tag gebannt![newline]" + 
                "<Backpfeiffe> 2 wörtchen = 1*2=2 usw.[newline]" +
                "<sec> und 3 mal in einem zug = 1*2*3 = 6 oder was?[newline]" +
                "jo[newline]<sec> und welche Wörter sind alle verboten? :>[newline]" +
                "<Backpfeiffe> ******, ****, ***********, *******, ********, ***, *****, *****, ***** und noch einige ähnliche mehr[newline]" +
                "*Backpfeiffe was banned from the server for 362880 days[newline]" + // <--
                "<Sechseckkugel> o.O[newline]" + 
                "<Tumanual> geil, selfowned by bot![newline]" +
                "<sec> o/ YES! We can!";
            PerformDataTest(text, 18, 4);
        }

        [TestMethod]
        public void BashDataSimpleWithServerContentStarting()
        {
            string text = "* ESL|luda is back from: kamelreiten (been away for 6h 54m) [newline]" + // <--
                "<LordofDemons> es ist nicht nett wie du deinen freund bezeichnest";
            PerformDataTest(text, 2, 1);
        }

        [TestMethod]
        public void BashDataSimpleWithCustom1hLater()
        {
            string text = "<iSi> also dann bis gleich ld[newline]" +
                "<shocker> jo, ich komm mit dem rad, bin dann in 10 minuten da. ida[newline]" +
                "---- 1 Stunde später ----[newline]" + // <--
                "<iSi> du willst mich verarschen oder? wo bleibst du denn?[newline]" +
                "<shocker> sorry, timo kam mit nem kasten vorbei[newline]" +
                "<iSi> ...arschloch[newline]" +
                "<shocker> cheers!";
            PerformDataTest(text, 7, 2);
        }

        [TestMethod]
        public void BashDataComplexWithCustomServerKickAndJoinInfo()
        {
            string text = "<Rigel> KingKashue dein pc ist nicht so 1337 wie meiner ich hab einen 122 ghz core32octa pc mit 5 617 terabyte festplatten ein dvd-bluray-hd-wr-rwr-drwxr-xr-x brenner mit 37 gigehurz L7 cache und 586 zoll lsd monitor mit 4325x2341 auflösung aber das beste ist das 400 gbps kabel, mit dem ich meine 25132 Mbit/s Internetleitung routen kann[newline]" +
                "<KingKashue> korreeekt...Aber kann dein PC das?[newline]" + // <--
                "*** Rigel was kicked by KingKashue (als Beweis, dass meiner es kann)[newline]" + // <--
                "*** Joins: Rigel [newline]" +
                "<Rigel> Nein.";
            PerformDataTest(text, 5, 2);
        }

        [TestMethod]
        public void BashDataComplexWithServerQuitInfo()
        {
            string text = "<CHRiS> lala? da?[newline]" +
                "<Lala> jaa :*[newline]" +
                "<CHRiS> 3 Worte.[newline]" +
                "<CHRiS> 12 Buchstaben.[newline]" +
                "<CHRiS> Leicht zu sagen.[newline]" +
                "<CHRiS> Schwer zu beweisen.[newline]" +
                "<Lala> Jaaaa ???? :-*** [newline]" +
                "<CHRiS> Ich bin Batman!!!!!11elf[newline]" +
                "<Lala> bloeder penner![newline]" +
                "Lala has quit IRC (******)"; // <--
            PerformDataTest(text, 10, 2);
        }

        [TestMethod]
        public void BashDataSimpleWithServerJoinInfoAtBeginning()
        {
            string text = "*** blazemore (empty3@dialup-209.245.205.138.Houston1.Level3.net) has joined #ramen[newline]" + // <--
                "<LkAway1> blazemore: was ist bei Dragonball Z heute passiert?[newline]" +
                "<HomerJ> die haben mit sonem typen gekämpft";
            PerformDataTest(text, 3, 2);
        }

        [TestMethod]
        public void BashDataSimpleWithStrangeSpaces()
        {
            string text = " <Andi> BF3 > BO2[newline]" + // <--
                " <Tom> nenn mir eine Sache, die man in Battlefield kann, die man in Black opps 2 nicht kann[newline]" + // <<-
                " <Andi> Leitern hochklettern :D[newline]" + // <<-
                " <Tom> fuuuuuuu "; // <<-
            PerformDataTest(text, 4, 2);
        }

        [TestMethod]
        public void BashDataSimpleWithTwoServerInfos()
        {
            string text = "*** UndErX has joined #help[newline]" +
                "<UndErX> släuft ihr l4m3rz?[newline]" +
                "<UndErX> antwortet wenn ihr keine n00bz seid:[newline]" +
                "<FiEsTy-> sag bitte[newline]" +
                "<UndErX> nein n00by l4m3r[newline]" +
                "<UndErX> alles l4m3rz[newline]" +
                "<FiEsTy-> hey, tipp bitte nicht /quit fiesty- weil ich sonst ausm irc rausflieg, bitte tipp das nicht![newline]" +
                "*** Quits: UndErX (Quit: fiesty-)";
            PerformDataTest(text, 8, 2);
        }

        [TestMethod]
        public void BashDataLongDialogWithServerQuits()
        {
            string text = "<Matajui> Zwiebel, kann ich dich etwas fragen? als dein freund?[newline]" +
                "<[ekg]Zwiebel> do könntest mich auch etwas als komplett fremder fragen[newline]" +
                "<Matajui> Denkst du das Pia mit mir ausginge, wenn ich sie fragen würde?[newline]" +
                "<[ekg]Zwiebel> öhm...[newline]" +
                "<[ekg]Zwiebel> frag sie doch einfach?[newline]" +
                "<Matajui> nicht bevor ich eine 2te meinung gehört hab[newline]" +
                "<Wumm> alter, die ist im channel[newline]" +
                "<Matajui> nein ist sie nicht[newline]" +
                "<[ekg]Zwiebel> doch ist sie, guck doch nach[newline]" +
                "<Matajui> shit[newline]" +
                "<Matajui> PLAN B[newline]" +
                "<Matajui> spammt rum[newline]" +
                "<Matajui> schreibt einfach irgendnen scheiß[newline]" +
                "<Matajui> Dann verschwindets[newline]" +
                "<Princess> hi[newline]" +
                "<Matajui> von ihrem Bildschirm[newline]" +
                "<Matajui> FUCK[newline]" +
                "* Matajui has quit (PLAN C!!!)[newline]" + // <--
                "<Princess> Ich werd jetzt Windows 7 installieren[newline]" +
                "<Princess> also bin ich eben offline[newline]" +
                "<Princess> wenn er sich in der zwischenzeit wieder hier rein traut, sagt ihm dass ich Ja sage[newline]" +
                "<[ekg]Zwiebel> rofl[newline]" +
                "* Princess has quit (QUIT)[newline]" + // <--
                "<[ekg]Zwiebel> DAS waren 2 nerds unter sich [newline]" +
                "<Wumm> Alter, ich wünschte ich könnte mich auch so derbe bei nem Mädel blamieren und trotzdem Erfolg haben";
            PerformDataTest(text, 25, 4);
        }

        [TestMethod]
        public void BashDataSimpleWithArrowServerInfo()
        {
            string text = "<anamexis> oh mann[newline]" +
                "<anamexis> Ich hab mir ne Coke aufgemacht...[newline]" +
                "--> Beefpile (~mbeefpile@cloaked.wi.rr.com) has joined #themacmind[newline]" +
                "<anamexis> und es ist alles rausgespritzt[newline]" +
                "<anamexis> Fast direkt auf mein Keyboard[newline]" +
                "<anamexis> Aber ich konnts noch grad vorbeilenken[newline]" +
                "<-- Beefpile has quit (kranke wichser)[newline]" +
                "<anamexis> :<";
            PerformDataTest(text, 8, 1);
        }

        [TestMethod]
        public void BashDataSimpleServerChangeNickInfo()
        {
            string text = "<HellTheVoid> Boah meinen Nickname gibts nur einmal auf der welt[newline]" +
                "<G0nz0> ja toll der macht auch keinen sinn[newline]" +
                "*** G0nz0 changed nick to PumuckelAufAbwegen[newline]" +
                "<PumuckelAufAbwegen> so... wow jetzt bin ich auch ein individuum[newline]" +
                "*** Pumuck3lAufAbw3g3n has joined  [newline]" +
                "<Pumuck3lAufAbw3g3n> Morgeeeennn";
            PerformDataTest(text, 6, 4);
        }

        private static void PerformDataTest(string text, int quotesCount, int personsCount)
        {
            var bashData = CreateBashData(text);
            var parsedQuote = bashData.QuoteItems;
            Assert.AreEqual<int>(quotesCount, bashData.QuoteItems.Count);

            int max = 0;
            foreach (var item in bashData.QuoteItems)
            {
                max = Math.Max(max, item.PersonIndex);
            }
            Assert.AreEqual<int>(personsCount, max + 1);
        }

        private static BashData CreateBashData(string text)
        {
            var data = new BashData();
            data.Content = text;
            return data;
        }
    }
}
