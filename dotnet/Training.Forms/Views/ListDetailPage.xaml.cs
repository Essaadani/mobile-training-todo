﻿//
// ListDetailPage.xaml.cs
//
// Author:
// 	Jim Borden  <jim.borden@couchbase.com>
//
// Copyright (c) 2016 Couchbase, Inc All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System;
using System.ComponentModel;
using MvvmCross.Forms.Views;
using Training.Core;
using Training.Forms;
using Xamarin.Forms;

namespace Training
{
    /// <summary>
    /// The high level page containing a list of tasks and list of users
    /// </summary>
    public partial class ListDetailPage : MvxTabbedPage
    {

        #region Variables

        private NavigationLifecycleHelper _navHelper;
        private UsersPage _usersPage;
        private TasksPage _tasksPage;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ListDetailPage()
        {
            InitializeComponent();
            _navHelper =  new NavigationLifecycleHelper(this);
        }

        #endregion

        #region Private API

        private void AddUsersTab(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = ViewModel as ListDetailViewModel;
            if(viewModel == null) {
                return;
            }

            if(e.PropertyName == nameof(viewModel.HasModeratorStatus)) {
                if(viewModel.HasModeratorStatus && Children.Count < 2) {
                    Children.Add(_usersPage);
                }
            }
        }

        #endregion

        #region Overrides

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var viewModel = ViewModel as ListDetailViewModel;
            if(viewModel == null || Children.Count > 0) {
                return;
            }

            _tasksPage = new TasksPage();
            _tasksPage.ViewModel = new TasksViewModel(viewModel);
            Children.Add(_tasksPage);

            _usersPage = new UsersPage();
            _usersPage.ViewModel = new UsersViewModel(viewModel);

            if(!viewModel.HasModeratorStatus) {
                viewModel.PropertyChanged += AddUsersTab;
            } else {
                Children.Add(_usersPage);
            }
        }

        public void SelectUsersPage()
        {
            CurrentPage = _usersPage;
            this.ToolbarItems[0].Text = "Tasks";
        }

        public void SelectTasksPage()
        {
            CurrentPage = _tasksPage;
            this.ToolbarItems[0].Text = "Users";
        }

        protected override void OnDisappearing()
        {
            if(_navHelper.OnDisappearing(Navigation)) {
                ((Children[0] as MvxPage)?.ViewModel as IDisposable)?.Dispose();
                (_usersPage.ViewModel as IDisposable)?.Dispose();
            }
        }

        private void OnPageSelect_Clicked(object sender, System.EventArgs e)
        {
            if(this.ToolbarItems[0].Text == "Users") {
                SelectUsersPage();
            } else {
                SelectTasksPage();
            }
        }

        private void OnAdd_Clicked(object sender, System.EventArgs e)
        {
            if (this.ToolbarItems[0].Text == "Tasks") {
                ((UsersViewModel)_usersPage.ViewModel).AddCommand.Execute(new object());
            } else {
                ((TasksViewModel)_tasksPage.ViewModel).AddCommand.Execute(new object());
            }
        }

        #endregion

    }
}

