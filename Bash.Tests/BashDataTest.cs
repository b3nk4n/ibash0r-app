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
            string text = "<i8b4uUnderground> d-_-b[newline]<BonyNoMore> wie machst du das spiegelverkehrte b?[newline]<BonyNoMore> warte[newline]<BonyNoMore> vergiss es";
            PerformDataTest(text, 4, 2);
        }

        [TestMethod]
        public void BashDataComplexWithServerContentInBetween()
        {
            string text = "<Backpfeiffe> so mädels, jetz is schluss![newline]<Backpfeiffe> da hier einige nicht ohne vulgärsprache auskommen, hab ich n bot geschrieben[newline]<Tumanual> why? geht doch auch ohne :([newline]<sec> freak![newline]<Backpfeiffe> na wenn ich off bin, gibts kein ban[newline]<Backpfeiffe> wenn ich euch nur die 1h kicke, dann seid ihr da meist eh afk[newline]<Backpfeiffe> und st�ndig ne zeit in das feld tippen... hab ich auch kein bock![newline]<Backpfeiffe> deswegen bot: 24h aktiv und arbeitet selbstständig :)[newline]<Backpfeiffe> mein sklave^^[newline]<Backpfeiffe> für 1 \"böses wort\" werdet ihr absofort 1 tag gebannt![newline]<Backpfeiffe> 2 w�rtchen = 1*2=2 usw.[newline]<sec> und 3 mal in einem zug = 1*2*3 = 6 oder was?[newline]jo[newline]<sec> und welche W�rter sind alle verboten? :>[newline]<Backpfeiffe> ******, ****, ***********, *******, ********, ***, *****, *****, ***** und noch einige �hnliche mehr[newline]*Backpfeiffe was banned from the server for 362880 days[newline]<Sechseckkugel> o.O[newline]<Tumanual> geil, selfowned by bot![newline]<sec> o/ YES! We can!";
            PerformDataTest(text, 18, 5);
        }

        [TestMethod]
        public void BashDataSimpleWithServerContentStarting()
        {
            string text = "* ESL|luda is back from: kamelreiten (been away for 6h 54m) [newline]<LordofDemons> es ist nicht nett wie du deinen freund bezeichnest";
            PerformDataTest(text, 2, 2);
        }

        public void BashDataSimpleWithCustom1hLater()
        {
            string text = "<iSi> also dann bis gleich ld[newline]<shocker> jo, ich komm mit dem rad, bin dann in 10 minuten da. ida[newline]---- 1 Stunde später ----[newline]<iSi> du willst mich verarschen oder? wo bleibst du denn?[newline]<shocker> sorry, timo kam mit nem kasten vorbei[newline]<iSi> ...arschloch[newline]<shocker> cheers!";
            PerformDataTest(text, 7, 2);
        }

        public void BashDataComplexWithCustomServerKickAndJoinInfo()
        {
            string text = "<Rigel> KingKashue dein pc ist nicht so 1337 wie meiner ich hab einen 122 ghz core32octa pc mit 5 617 terabyte festplatten ein dvd-bluray-hd-wr-rwr-drwxr-xr-x brenner mit 37 gigehurz L7 cache und 586 zoll lsd monitor mit 4325x2341 auflösung aber das beste ist das 400 gbps kabel, mit dem ich meine 25132 Mbit/s Internetleitung routen kann[newline]<KingKashue> korreeekt...Aber kann dein PC das?[newline]*** Rigel was kicked by KingKashue (als Beweis, dass meiner es kann)[newline]*** Joins: Rigel [newline]<Rigel> Nein.";
            PerformDataTest(text, 5, 2);
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
