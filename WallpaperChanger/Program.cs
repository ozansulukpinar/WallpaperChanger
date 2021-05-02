/*
 * Filename: ..\WallpaperChanger\Program.cs
 * Path: ..\WallpaperChanger
 * Created Date: Sunday, May 2nd 2021, 3:19:25 pm
 * Author: Ozan Sulukpinar
 * 
 * Copyright (c) 2021
 */
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;

namespace WallpaperChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "wallpaper.jpg";

            Photo photo = SelectAPhoto();
            Uri address = new Uri(photo.Download_url);
            
            //Before changing the wallpaper, the photo should be downloaded to PC
            new WebClient().DownloadFile(address, fileName);
            //Now wallpaper can be changed
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, fileName, SPIF_UPDATEINIFILE);            
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        //This method selects a photo from JSON data
        private static Photo SelectAPhoto(){
            List<Photo> photoList = new List<Photo>();
            Photo photo = new Photo(){Download_url=null};

            int randomNumber = GetRandomNumber(11);
            string pageNumber = randomNumber.ToString();
            string address = String.Format("https://picsum.photos/v2/list?page={0}&limit=100", pageNumber);

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var json = webClient.DownloadString(address);
                    photoList = JsonConvert.DeserializeObject<List<Photo>>(json);
                }
                
                int number = GetRandomNumber(100);
                photo = photoList[number];
            }
            catch (Exception exception)
            {
                string explanation = String.Format("Exception occurred: exceptionMessage: {0}, exceptionHelpLink: {1}, exceptionSource: {2}, exceptionStackTrace: {3}, exceptionTargetSite: {4}", exception.Message, exception.HelpLink, exception.Source, exception.StackTrace, exception.TargetSite);
                Console.WriteLine(explanation);
            }

            string downloadUrl = photo.Download_url;

            //If downloadUrl value is null, this method is called until the opposite happens
            if (downloadUrl == null)
                SelectAPhoto();            

            bool isUrlValid = CheckUrl(downloadUrl);

            //If url of selected photo is not valid, this method is called until the opposite happens
            if (!isUrlValid)
                SelectAPhoto();                

            return photo;
        }

        //This method returns a random number
        private static int GetRandomNumber(int maxNumber)
        {
            Random random = new Random();
            int randomNumber = random.Next(maxNumber);

            return randomNumber;
        }

        //This method checks the url of selected photo
        private static bool CheckUrl(string url){
            bool result = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            int statusCode = (int)response.StatusCode;

            if (statusCode == 200)
                result = true;

            return result;
        }
    }
}