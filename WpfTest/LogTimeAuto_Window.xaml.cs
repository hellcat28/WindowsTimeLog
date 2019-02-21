﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using DataFormat = RestSharp.DataFormat;
using System.Net;
using static WpfTest.LogTimeManual_Window;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for LogTimeAuto_Window.xaml
    /// </summary>
    public partial class LogTimeAuto_Window : MetroWindow
    {
        public class LinksProperty
        {
            public string href { get; set; }
        }
        // create a class of combobox activity type
        public class ComboBoxActivity
        {
            public string key { get; set; }
            public string value { get; set; }
            public ComboBoxActivity(string _key, string _value)
            {
                key = _key;
                value = _value;
            }
        }
        public class Links
        {
            public LinksProperty project { get; set; }
            public LinksProperty activity { get; set; }
            public LinksProperty workPackage { get; set; }
            public LinksProperty customField4 { get; set; }
        }
        public class RootObject
        {
            public Links _links { get; set; }
            public string hours { get; set; }
            public string comment { get; set; }
            public string spentOn { get; set; }
        }

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        public LogTimeAuto_Window()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 1);

        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",ts.Hours, ts.Minutes, ts.Seconds);
                clocktxtblock.Text = currentTime;
            }
        }
        private void startbtn_Click(object sender, RoutedEventArgs e)
        {
            sw.Start();
            dt.Start();
        }
  
        private void stopbtn_Click(object sender, RoutedEventArgs e)
        {
          
            if (sw.IsRunning)
            {
                sw.Stop();
                //format HH:MM:SS to decimal   
                decimal dec = Convert.ToDecimal(TimeSpan.Parse(currentTime).TotalHours);
                //roundup to 2 decimal place
                dec = Math.Round(dec, 2);

                elapsedtimeitem.Text = dec.ToString(new CultureInfo("en-US"));
            }
            else
            {
                ;
            }
        }
        private void resetbtn_Click(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            clocktxtblock.Text = "00:00:00";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string project_id = (App.Current as App).project_id;
            string workpackage_id = (App.Current as App).workpackage_id;
            string project_name = (App.Current as App).project_name;
            string workpackage_name = (App.Current as App).workpackage_name;
            Project.Text = project_name;
            WorkPackage.Text = workpackage_name;
            //Add activity type to combo box
            List<ComboBoxActivity> cbA = new List<ComboBoxActivity>();
            string api_mockup_server = (App.Current as App).api_mockup_server;
            var client = new RestClient(api_mockup_server);
            var request = new RestSharp.RestRequest("activities", Method.GET);
            IRestResponse response = client.Execute(request);
            RootObjectActivity te_activity = JsonConvert.DeserializeObject<RootObjectActivity>(response.Content);
            var tea_id = 0;
            while (tea_id < te_activity.activities.Count)
            {
                cbA.Add(new ComboBoxActivity(te_activity.activities[tea_id].id, te_activity.activities[tea_id].name));
                tea_id++;
            }
            //bind to the Activity combobox in xaml
            Activity.DisplayMemberPath = "value";
            Activity.SelectedValuePath = "key";
            Activity.ItemsSource = cbA;
        }

        private void LogTime_Click(object sender, RoutedEventArgs e)
        {
            string project_id = (App.Current as App).project_id;
            string workpackage_id = (App.Current as App).workpackage_id;
            // get access to cbox key
            ComboBoxActivity cbA = (ComboBoxActivity)Activity.SelectedItem;
            string activity_type = cbA.key;

            string log_hour = elapsedtimeitem.Text;
            string comment = tb_Comment.Text;
            DateTime date = (DateTime)datePicker.SelectedDate;
            string result = date.ToString("yyyy-MM-dd");
            string user_id = (App.Current as App).u_id;

            MessageBox.Show(user_id);
            RootObject time_entry = new RootObject()
            {
                _links = new Links
                {
                    project = new LinksProperty
                    {
                        href = "/api/v3/projects/" + project_id
                    },
                    activity = new LinksProperty
                    {
                        href = "/api/v3/time_entries/activities/" + activity_type
                    },
                    workPackage = new LinksProperty
                    {
                        href = "/api/v3/work_packages/" + workpackage_id
                    },
                    customField4 = new LinksProperty
                    {
                        href = "/api/v3/users/" + user_id
                    }
                },
                hours = "PT" + log_hour + "H",
                comment = comment,
                spentOn = result
            };

            var json = JsonConvert.SerializeObject(time_entry);
            string api_server = (App.Current as App).api_server;
            var client = new RestClient(api_server);
            var request = new RestRequest("/time_entries", Method.POST);
            string password = (App.Current as App).api_key;
            client.Authenticator = new HttpBasicAuthenticator("apikey", password);

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;

            MessageBox.Show(statusCode.ToString());
        }
    }
}
