using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZennoLab.OwnCode
{
	
	public class Tech{
		
		public static IZennoPosterProjectModel project;
		public static Instance instance;
		
		public static string GetQueryHash(string username, string shortCode = "Bj0UusgB_og")
		{
			Tab tab = instance.ActiveTab;
			string targetUrl = "";
			project.Profile.Load(project.Directory+@"\profiles\"+username+".zpprofile");
			
			project.SendWarningToLog("1/2 переход в общем коде");
			tab.Navigate("https://www.instagram.com/");
			if(tab.IsBusy) tab.WaitDownloading();
			project.SendWarningToLog("1/2 переход в общем коде");
			tab.Navigate("https://www.instagram.com/p/"+shortCode+@"/?taken-by=buzova86");
			if(tab.IsBusy) tab.WaitDownloading();
			tab.FindElementByXPath(@"//div/a[@class='zV_Nj kCcVy']",0).Click();
			if(tab.IsBusy) tab.WaitDownloading();
			tab.MainDocument.EvaluateScript(@"javascript:scrollTo(0,500)");
			if(tab.IsBusy) tab.WaitDownloading();
			var traffic = tab.GetTraffic(new [] {"https://www.instagram.com/graphql/query/"});
			foreach(var t in traffic){
			    //project.SendInfoToLog(string.Format("Url: {0}\r\n Method: {1}\r\n Result: {2}", t.Url, t.Method, t.ResultCode));
				targetUrl = t.Url;
			}

			string queryHash = new Regex(@"(?<=query_hash=).*(?=&)").Match(targetUrl).ToString();
			return queryHash;
		}
	}
	
}
