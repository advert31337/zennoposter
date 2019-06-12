//Придумать постере в инсту


int intPicSideSize = 0;
List<string> lstFotoFiles = new List<string>();
//подключить библиотеку imageprocessor
string strVideosDirectoryPath = project.Directory + @"\Unique\in\";
lstFotoFiles.AddRange(Directory.EnumerateFiles(strVideosDirectoryPath, "*.jpg", SearchOption.AllDirectories));

for(int i = 0; i<lstFotoFiles.Count;i++){
	var path = lstFotoFiles[i]; // Наш исходный файл
	var pathrez = project.Directory + String.Format(@"\Unique\out\{0}.jpg",i+1); // Файл куда будем сохранять
	//Допилить выходное именование файлов и сортировку по папкам
	FileStream fs = File.OpenWrite(pathrez); //  для записи
	byte[] photoBytes = File.ReadAllBytes(path); // для чтения
	
	ISupportedImageFormat format = new JpegFormat { Quality = 100 }; // Устанавливаем качество фото на выходе
	
	Image image = new Bitmap(path);
	int intItemHeight =image.Height;
	int intItemWidth =image.Width;

	if(intItemHeight>intItemWidth){
		intPicSideSize = intItemHeight;
	}else{
		intPicSideSize = intItemWidth;
	}
	image.Dispose();
	//Если наибольшая сторона картинки меньше 500, закрываем fs, удаляем созданный файл файл, пропускаем цикл
	if(intPicSideSize<500){
		fs.Dispose();
		File.Delete(pathrez);
		continue;
	}

	Size size = new Size(intPicSideSize, intPicSideSize); // Это размер фото на выходе
	         
	TextLayer text = new TextLayer(); // Создаем экземпляр класса
	 
	text.FontColor = Color.White; // Цвет шрифта белый
	//text.FontColor = Color.FromArgb(100, 100, 100); // Это тоже цвет, только в RGB
	         
	text.FontFamily = new FontFamily("Arial"); // Тип шрифта
	text.FontSize = 17; // Размер шрифта
	text.DropShadow = true; // Тень
	text.Opacity = 60; // Непрозрачность
	text.Style = FontStyle.Bold; //Жирный шрифт
	text.Text = "@cozi_generator"; // Ну и сам текст "@autofuckers" "@ru_gf_instockings" "@ru_sexy_legs"
	text.Position = new Point(5,5); // Координаты вставки водяного знака
	         
	using (MemoryStream inStream = new MemoryStream(photoBytes))
	            {
	                using (MemoryStream outStream = new MemoryStream())
	                {
	                 
	                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData:true))
	                    {
							// Load, resize, set the format and quality and save an image.
	                        imageFactory.Load(inStream)// грузим картинку
										.Watermark(text)// добавляем водяной знак
	                                    .Resize(size)
										.BackgroundColor(Color.White)// меняем размер (500 на 500 см. выше)
	                                    .Format(format)  // выбираем формат картинки, т.е jpeg(jpg)
	                                    .Save(outStream);// сохраняем в поток
	                        //outStream.CopyTo(inStream);
	                        outStream.WriteTo(fs); // записываем в файл
	                        outStream.Close();     // не забываем закрывать потоки ввода-вывода
	                    }
	                    inStream.Close(); // не забываем закрывать потоки ввода-вывода
	                    fs.Close();
	               }
	            }
	Thread.Sleep(500);
}
project.SendInfoToLog("Готово");