﻿//
// TaskImageModel.cs
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
using System.IO;

using Couchbase.Lite;
using Training.Core;

namespace Training.Models
{
    /// <summary>
    /// The model for the page that displays a task's saved image
    /// </summary>
    public sealed class TaskImageModel
    {

        #region Variables

        private Document _taskDocument;

        #endregion

        #region Properties

        /// <summary>
        /// The image stored on the task
        /// </summary>
        /// <value>The image.</value>
        public Stream Image
        {
            get => _taskDocument.GetBlob("image")?.ContentStream;
            set {
                using(var mutableTask = _taskDocument.ToMutable()) {
                    if (value == null)
                    {
                        mutableTask.Remove("image");
                    }
                    else
                    {
                        mutableTask.SetBlob("image", new Blob("image/png", value));
                    }
                    
                    CoreApp.Database.Save(mutableTask);
                    _taskDocument = mutableTask;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="documentID">The id of the document to read from</param>
        public TaskImageModel(string documentID)
        {
            _taskDocument = CoreApp.Database.GetDocument(documentID);
        }

        #endregion
    }
}

