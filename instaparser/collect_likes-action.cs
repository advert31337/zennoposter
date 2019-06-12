//добавить анонимайзер
//Вставить случайность в паузы между запросами
//Браузерные дела
Tab tab = instance.ActiveTab;
//для общего кода
Tech.project = project;
Tech.instance = instance;
//Вставить случайность в паузы между запросами
var rnd = new Random();
int min = 60*1000;
int hour = 60*60*1000;
//====Счетчики
int rCount = 356000;
int offsetStep = 50;
int i=1;
int a=1;
//Вынести в настройки
string postShortCode = "Bj4TL39AwWI";
var username = "andrea_rossi_oboistore";
string outFileName = "natali_iashchuk_likers";

project.Profile.Load(project.Directory+@"\profiles\"+username+".zpprofile");
tab.Navigate("https://www.instagram.com/");
if(tab.IsBusy) tab.WaitDownloading();

string nextPage = "True";

//Получаем uery hash
string queryHash = "1cb6ec562846122743b61e492c85999f";//Tech.GetQueryHash(username,postShortCode);// надо выцепить последний запрос после прокрутки
project.SendWarningToLog("ПРОВЕРКА================="+ Environment.NewLine
							+ "Взяли хэш"+ Environment.NewLine 
							+ queryHash);

//Для формирования ссылки
string endCursor = "";
string url = @"https://www.instagram.com/graphql/query/?query_hash="+queryHash+@"&variables=";
// или попробовать Uri.EscapeDataString(keyword)
string url2 = Uri.EscapeDataString(@"{""shortcode"":"""+postShortCode+@""",""first"":12}");
string urlResult = url + url2;
project.SendWarningToLog("ПРОВЕРКА================="+ Environment.NewLine
							+ "итоговая ссылка"+ Environment.NewLine 
							+ url);
//==========Пути к выходным файлам

string idsFilePath = project.Directory + @"\out\"+outFileName + "__ids.txt";
string usersFilePath = project.Directory + @"\out\"+outFileName + "__users.txt";

//Списки для выходных файлов
var idsList = new List<string>();
var usersList = new List<string>();
//=========================================================
while(nextPage == "True"){
	tab.Navigate(urlResult);
	if(tab.IsBusy) tab.WaitDownloading();
	//Забираем результат поиска в переменную для распарса в json
	var elResp = tab.FindElementByXPath(@"//pre",0);
	//Проверка наличия элемента
	for(int q = 0;q<5;q++){
		if(elResp.IsVoid){ 
			tab.Navigate(urlResult);
			if(tab.IsBusy) tab.WaitDownloading();
			elResp = tab.FindElementByXPath(@"//pre",0);
			if(!elResp.IsVoid) break;
		}
		//сюда вставить всплывающее окно для подождать
		//System.Windows.Forms.MessageBox.Show("ПРоверь интернет");
		if(q==5) throw new Exception("ЧТО-ТО не так с путями xPath");
	}
	var strResp = elResp.GetAttribute("InnerHtml").ToString();
	
	project.SendWarningToLog("ПРОВЕРКА================="+ Environment.NewLine 
								+ "взяли json"+ Environment.NewLine 
								+ strResp);
	JObject json = JObject.Parse(strResp);
	//===подготавливаем следующую ссылку
	//Есть еще пользователи?
	nextPage = json.SelectToken("data.shortcode_media.edge_liked_by.page_info.has_next_page").ToString();
	//Конец блока с пользователями, его надо подменить в ссылке
	endCursor = json.SelectToken("data.shortcode_media.edge_liked_by.page_info.end_cursor").ToString();
	try{
		string limitRequest = json.SelectToken("message").ToString();
		if(limitRequest=="rate limited" ) Thread.Sleep(2*60*60*1000);
	}catch{
		
	}
	if(endCursor!=""){
		url2 = TextProcessing.UrlEncode(@"{""shortcode"":""" + postShortCode + @""","+
			@"""first"":"+ offsetStep +","+
			@"""after"":""" + endCursor + @"""}");
		urlResult = url + url2;
	}
	

	IEnumerable<JToken> likers = json.SelectTokens("data.shortcode_media.edge_liked_by.edges[*]", false);
	foreach(JToken liker in likers){
		string likerID = liker.SelectToken("node.id").ToString();
		string likerName = liker.SelectToken("node.username").ToString();
		project.SendInfoToLog(String.Format("{0}--{1}:{2}",i,likerName,likerID));
		idsList.Add(likerID);
		usersList.Add(likerName);
		i++;
		
	}
	if(!File.Exists(idsFilePath)) File.Create(idsFilePath).Close();
	if(!File.Exists(usersFilePath)) File.Create(usersFilePath).Close();
	
	File.WriteAllLines(idsFilePath, idsList);
	File.WriteAllLines(usersFilePath, usersList);
	
	project.SendInfoToLog("Проход: "+ a);
	if(a>=356000/12){
		project.SendInfoToLog("Достигнут лимит сбора: " + rCount);
		break;
	}
	//Задержки
	if(a%30==0) {
		project.SendInfoToLog("Пауза "+ 0.5 +" минут");
		Thread.Sleep(60*1000);
	}
	if(a%200==0){
		project.SendInfoToLog("Пауза "+ 2*hour +" минуты");
		Thread.Sleep(2*min);
	}
	if(a%1000==0){
		project.SendInfoToLog("Пауза "+ 10*hour +" минут");
		Thread.Sleep(10*min);
	}
	if(DateTime.Now.Hour>=23){ 
		Thread.Sleep(5*hour);
		project.SendInfoToLog("Пауза "+ 5*hour +" часов");
	}
	a++;
}
System.Windows.Forms.MessageBox.Show("Все вроде спарсил. Проверяй");
Console.Beep(659, 300);
Console.Beep(659, 300);
Console.Beep(659, 300);