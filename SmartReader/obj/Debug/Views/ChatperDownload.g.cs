﻿#pragma checksum "F:\Projects\SmartReader\SmartReader\Views\ChatperDownload.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "35F7A0D76EADD33F1092999C2B89AEA2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using SmartReader.Library.Converter;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SmartReader.Views {
    
    
    public partial class ChatperDownload : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal SmartReader.Library.Converter.BoolToVisibilityConverter ShowDownloadStatusConverter;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ListBox ChapterList;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SmartReader;component/Views/ChatperDownload.xaml", System.UriKind.Relative));
            this.ShowDownloadStatusConverter = ((SmartReader.Library.Converter.BoolToVisibilityConverter)(this.FindName("ShowDownloadStatusConverter")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ChapterList = ((System.Windows.Controls.ListBox)(this.FindName("ChapterList")));
        }
    }
}

