﻿//
// TasksViewModel.cs
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
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Acr.UserDialogs;
using CouchbaseLabs.MVVM.Services;
using Training.Core;
using Training.Models;

namespace Training.ViewModels
{
    /// <summary>
    /// The view model for the list of tasks page
    /// </summary>
    public class TasksViewModel : BaseNavigationViewModel<TasksModel>, IDisposable
    {//TasksModel

        #region Variables

        private readonly IUserDialogs _dialogs;
        private readonly ImageChooser _imageChooser;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently selected item in the table view
        /// </summary>
        /// <value>The selected item.</value>
        public TaskCellModel SelectedItem
        {
            get => _selectedItem;
            set {
                if(_selectedItem == value) {
                    return;
                }

                _selectedItem = value;
                value.IsChecked = !value.IsChecked;
                SetPropertyChanged(ref _selectedItem, null);
            }
        }
        private TaskCellModel _selectedItem;

        /// <summary>
        /// Gets the list of tasks for display in the list view
        /// </summary>
        /// <value>The list data.</value>
        public ObservableCollection<TaskCellModel> ListData;// => Model.ListData;

        /// <summary>
        /// Gets or sets the current text being searched for in the list
        /// </summary>
        public string SearchTerm
        {
            get {
                return _searchTerm;
            }
            set {
                if(SetPropertyChanged(ref _searchTerm, value)) {
                    //Model.Filter(value);
                }
            }
        }
        private string _searchTerm;

        /// <summary>
        /// Gets the command that is fired when the add button is pressed
        /// </summary>
        /// <value>The add command.</value>
        public ICommand AddCommand;// => new MvxCommand(AddNewItem);

        #endregion

        #region Constructors

        public TasksViewModel(INavigationService navigationService, IUserDialogs dialogs, ListDetailViewModel parent) 
            : base(navigationService, dialogs, new TasksModel(parent.CurrentListID))
        {
            _dialogs = dialogs;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The parent view model (this is a nested view model).</param>
        //public TasksViewModel(ListDetailViewModel parent) : base(new TasksModel(parent.CurrentListID))
        //{
        //    _dialogs = Mvx.Resolve<IUserDialogs>();
        //    _imageChooser = new ImageChooser(new ImageChooserConfig {
        //        Dialogs = _dialogs
        //    });

        //    ListData.CollectionChanged += (sender, e) => 
        //    {
        //        if(e.NewItems == null) {
        //            return;
        //        }

        //        UpdateButtons(e.NewItems);
        //    };

        //    UpdateButtons(ListData);
        //}

        #endregion

        #region Internal API

        internal async Task ShowOrChooseImage(TaskCellModel taskDocument)
        {
            if(!taskDocument.HasImage()) {
                await ChooseImage(taskDocument);
            } else {
                //ShowViewModel<TaskImageViewModel>(new { documentID = taskDocument.DocumentID });
            }
        }

        #endregion

        #region Private API

        private void UpdateButtons(IList newItems)
        {
            foreach (TaskCellModel item in newItems) {
                if (item.AddImageCommand == null) {
                    //item.AddImageCommand = new MvxAsyncCommand<TaskCellModel>(ShowOrChooseImage);
                }
            }
        }

        private async Task ChooseImage(TaskCellModel taskCellModel)
        {
            var result = await _imageChooser.GetPhotoAsync();
            if(result == null) {
                return;
            }

            if(result == Stream.Null) {
                result = null;
            }

            try {
                taskCellModel.SetImage(result);
            } catch(Exception e) {
                _dialogs.Toast(e.Message);
                return;
            }
        }
    
        private void AddNewItem()
        {
            _dialogs.Prompt(new PromptConfig {
                OnAction = CreateNewItem,
                Title = "New Task",
                Placeholder = "Task Name"
            });
        }

        private void CreateNewItem(PromptResult result)
        {
            if(!result.Ok) {
                return;
            }

            try {
                //Model.CreateNewTask(result.Text);
            } catch(Exception e) {
                _dialogs.Toast(e.Message);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //Model.Dispose();
        }

        #endregion
    }
}

