using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Globalization;
using Windows.Storage;

namespace KalturatorWalut
{
     
    public sealed partial class MainPage : Page
    {
        List<string> nazwy = new List<string>();
        List<string> kursy = new List<string>();
        const string daneNBP = "http://www.nbp.pl/kursy/xml/LastA.xml"; //tu adres pliku z danymi kursowymi
        List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>(); //lista pozycji z danymi dla kolejnych walut

       
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ApplicationData.Current.LocalSettings.Values["lbxZWaluty"] != null)
                {
                    var serwerNBP = new HttpClient();
                    string dane = "";
                    dane = await serwerNBP.GetStringAsync(new Uri(daneNBP));

                    if (dane != null)
                    {
                        kursyAktualne.Clear();
                    }
                    XDocument daneKursowe = XDocument.Parse(dane);

                    kursyAktualne = (from item in daneKursowe.Descendants("pozycja")
                                     select new PozycjaTabeliA()
                                     {
                                         przelicznik = (item.Element("przelicznik").Value),
                                         kod_waluty = item.Element("kod_waluty").Value,
                                         kurs_sredni = item.Element("kurs_sredni").Value
                                     }).ToList();

                    kursyAktualne.Insert(0, new PozycjaTabeliA()
                    {
                        kurs_sredni = "1,0000",
                        kod_waluty = "PLN",
                        przelicznik = "1"
                    });

                    lbxZWaluty.ItemsSource = kursyAktualne;
                    lbxNaWalute.ItemsSource = kursyAktualne;
                }
            }
            catch (Exception ex)
            {
                txtKwota.Text = "błąd";
            }
        }



        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            Przelicz();

        }
        private void lbxZWaluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Przelicz();
            /*var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["lbxZWaluty"] = lbxZWaluty.SelectedIndex;
            Object val = localSettings.Values["lbxZWaluty"];
            if (val == null)
            {
                txtKwota.Text = "null";
            }
            else
            {
                txtKwota.Text = "else";
            }*/


        }

        private void lbxNaWalute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Przelicz();
        }

        private void btnOProgramie_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OProgramie));
        }

        private void btnPomoc_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pomoc));
        }
        private void Przelicz()
        {
            try
            {
                if (lbxZWaluty.SelectedItem != null && lbxNaWalute.SelectedItem != null && txtKwota != null)
                {
                    int wybranaPozycja = lbxZWaluty.SelectedIndex;
                    PozycjaTabeliA zWaluty = kursyAktualne[wybranaPozycja];

                    double kurssredni;
                    kurssredni = double.Parse(zWaluty.kurs_sredni);

                    int wybranaPozycja2 = lbxNaWalute.SelectedIndex;
                    PozycjaTabeliA naWaluty = kursyAktualne[wybranaPozycja2];

                    double kurssredni2;
                    kurssredni2 = double.Parse(naWaluty.kurs_sredni);

                    //string wynik = (double.Parse(txtKwota.Text) * kurssredni).ToString();

                    tbPrzeliczona.Text = (double.Parse(txtKwota.Text) * kurssredni / kurssredni2).ToString();
                    tbPrzeliczona.Text = tbPrzeliczona.Text.Replace(",", ".");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public class PozycjaTabeliA
        {
            public string przelicznik { get; set; }
            public string kod_waluty { get; set; }
            public string kurs_sredni { get; set; }
        }

    }
}
