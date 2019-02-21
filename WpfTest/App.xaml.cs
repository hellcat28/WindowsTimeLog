﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //store api server
        public string api_server { get; set; }
        //store api mockup server
        public string api_mockup_server { get; set; }
        //store apikey
        public string api_key { get; set; }
        //store project id and name
        public string project_id { get; set; }
        public string project_name { get; set; }

        //store version id and name
        public string version_id { get; set; }
        public string version_name { get; set; }
        //store work package id and name
        public string workpackage_id { get; set; }
        public string workpackage_name { get; set; }
   
        //store activity id and name
        public string activity_id { get; set; }
        public string activity_name { get; set; }
        // store user id
        public string u_id { get; set; }
       
    }
    
}
