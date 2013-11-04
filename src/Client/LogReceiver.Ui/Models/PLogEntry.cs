﻿/* -----------------------------------------------------------------------------
 * This source file is part of LogReceiver application
 * For the latest info, contact r@smus.nu
 *
 * Copyright (c) 2012 BLOCKS.DK
 * 
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with
 * this program; if not, write to the Free Software Foundation, Inc., 59 Temple
 * Place - Suite 330, Boston, MA 02111-1307, USA, or go to
 * http://www.gnu.org/copyleft/lesser.txt.
 * -----------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Blocks.Core.ObservableObjects;
using LogReceiver.Core.Models;

namespace LogReceiver.Ui.Models
{
    public class PLogEntry : ObservableObjectBase
    {
        private const string ImagePathNone = "/LogReceiver.Ui;component/Resources/Icons/none.png";
        private const string ImagePathDebug = "/LogReceiver.Ui;component/Resources/Icons/bug.png";
        private const string ImagePathInformation = "/LogReceiver.Ui;component/Resources/Icons/information.png";
        private const string ImagePathWarning = "/LogReceiver.Ui;component/Resources/Icons/error.png";
        private const string ImagePathError = "/LogReceiver.Ui;component/Resources/Icons/exclamation.png";
        private const string ImagePathFatal = "/LogReceiver.Ui;component/Resources/Icons/skull.png";

        private static readonly IDictionary<LogEntryLevelType, string> ImagePathMap = new Dictionary<LogEntryLevelType, string>
            {
                {LogEntryLevelType.None, ImagePathNone},
                {LogEntryLevelType.Debug, ImagePathDebug},
                {LogEntryLevelType.Information, ImagePathInformation},
                {LogEntryLevelType.Warning, ImagePathWarning},
                {LogEntryLevelType.Error, ImagePathError},
                {LogEntryLevelType.Fatal, ImagePathFatal},
            };

        #region Property - Id
        public const string PropertyId = "Id";
        private long _id;
        public long Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                RaisePropertyChanged(PropertyId);
            }
        }
        #endregion

        #region Property - Time
        public const string PropertyTime = "Time";
        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set
            {
                if (_time == value)
                {
                    return;
                }
                _time = value;
                RaisePropertyChanged(PropertyTime);
            }
        }
        #endregion

        #region Property - ThreadText
        public const string PropertyThreadText = "ThreadText";
        private string _threadText;
        public string ThreadText
        {
            get { return _threadText; }
            set
            {
                if (_threadText == value)
                {
                    return;
                }
                _threadText = value;
                RaisePropertyChanged(PropertyThreadText);
            }
        }
        #endregion

        #region Propery - Level
        public const string PropertyLevel = "Level";
        private LogEntryLevelType _level;
        public LogEntryLevelType Level
        {
            get { return _level; }
            set
            {
                if (_level == value)
                {
                    return;
                }
                _level = value;
                RaisePropertyChanged(PropertyLevel);
            }
        }
        #endregion

        #region Property - LoggerText
        public const string PropertyLoggerText = "LoggerText";
        private string _loggerText;
        public string LoggerText
        {
            get { return _loggerText; }
            set
            {
                if (_loggerText == value)
                {
                    return;
                }
                _loggerText = value;
                RaisePropertyChanged(PropertyLoggerText);
            }
        }
        #endregion

        #region Property - LoggerTextShort
        private string _loggerTextShort;
        public string LoggerTextShort
        {
            get { return _loggerTextShort; }
            set
            {
                if (_loggerTextShort == value)
                {
                    return;
                }
                _loggerTextShort = value;
                RaisePropertyChanged(() => LoggerTextShort);
            }
        }
        #endregion

        #region Property - MessageText
        public const string PropertyMessageText = "MessageText";
        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                if (_messageText == value)
                {
                    return;
                }
                _messageText = value;
                RaisePropertyChanged(PropertyMessageText);
            }
        }
        #endregion

        #region Property - MessageHeaderText
        public const string PropertyMessageHeaderText = "MessageHeaderText";
        private string _messageHeaderText;
        public string MessageHeaderText
        {
            get { return _messageHeaderText; }
            set
            {
                if (_messageHeaderText == value)
                {
                    return;
                }
                _messageHeaderText = value;
                RaisePropertyChanged(PropertyMessageHeaderText);
            }
        }
        #endregion

        #region Property - ExceptionText
        public const string PropertyExceptionText = "ExceptionText";
        private string _exceptionText;
        public string ExceptionText
        {
            get { return _exceptionText; }
            set
            {
                if (_exceptionText == value)
                {
                    return;
                }
                _exceptionText = value;
                RaisePropertyChanged(PropertyExceptionText);
            }
        }
        #endregion

        #region Property - ImageSource
        public const string PropertyImageSource = "ImageSource";
        private string _imageSource;
        public string ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource == value)
                {
                    return;
                }
                _imageSource = value;
                RaisePropertyChanged(PropertyImageSource);
            }
        }

        #endregion

        #region Property - ReceivedTime
        private DateTime _receivedTime;
        public DateTime ReceivedTime
        {
            get { return _receivedTime; }
            set
            {
                if (_receivedTime == value)
                {
                    return;
                }
                _receivedTime = value;
                RaisePropertyChanged(() => ReceivedTime);
            }
        }
        #endregion

        public PLogEntry()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PropertyLevel:
                    ImageSource = ImagePathMap[Level];
                    break;
                case PropertyMessageText:
                    MessageHeaderText = string.IsNullOrEmpty(MessageText) || MessageText.IndexOf('\n') < 0
                        ? MessageText
                        : MessageText.Substring(0, MessageText.IndexOf('\n'))
                            .Replace("\n", string.Empty)
                            .Replace("\r", string.Empty);
                    break;
                case "LoggerText":
                    LoggerTextShort = string.IsNullOrEmpty(LoggerText)
                        ? string.Empty
                        : LoggerText.Remove(0, LoggerText.LastIndexOf('.') + 1);
                    break;
            }
        }
    }
}
