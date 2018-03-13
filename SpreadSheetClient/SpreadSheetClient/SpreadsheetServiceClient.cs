using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;

namespace SpreadSheetClient
{
    internal sealed class SpreadsheetServiceClient : TextTcpClient
    {
        private const string NameParameterName = "name";

        private const string UnableToSendCommandErrorMessage = "unable to send command.";
        private const string InvalidResponseErrorMessage = "the server returned an invalid response.";

        private TaskCompletionSource<CreateSessionResponse> myCreateSessionTaskCompletionSource;
        private TaskCompletionSource<JoinSessionResponse> myJoinSessionTaskCompletionSource;
        private TaskCompletionSource<ChangeCellResponse> myChangeCellTaskCompletionSource;
        private TaskCompletionSource<UndoLastChangeResponse> myUndoLastChangeTaskCompletionSource;
        private TaskCompletionSource<SaveResponse> mySaveTaskCompletionSource;

        public event CellUpdatedEventHandler CellUpdated;

        private static string GetSetting(string settingName)
        {
            var setting = ConfigurationManager.AppSettings[settingName];

            if (string.IsNullOrWhiteSpace(setting))
            {
                throw new ConfigurationErrorsException("the \"" + settingName + "\" setting can't be empty in the application configuration file.");
            }

            return setting;
        }

        private static bool ValidateParameters(SortedDictionary<string, string> commandParameters, params string[] parameterNames)
        {
            foreach (var name in commandParameters.Keys)
            {
                if (Array.IndexOf(parameterNames, name) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TrySetResponse<T>(TaskCompletionSource<T> taskCompletionSource, T response)
        {
            if (taskCompletionSource != null)
            {
                taskCompletionSource.TrySetResult(response);
            }
            return true;
        }

        private static bool CommandFailedValidateParameters<T>(SortedDictionary<string, string> commandParameters, TaskCompletionSource<T> taskCompletionSource)
            where T : FailureResponse, new()
        {
            if (ValidateParameters(commandParameters, NameParameterName))
            {
                return false;
            }

            var response = new T
            {
                ErrorMessage = InvalidResponseErrorMessage
            };

            return TrySetResponse(taskCompletionSource, response);
        }

        private static bool CommandFailed<T>(SortedDictionary<string, string> commandParameters, T response, string commandName, TaskCompletionSource<T> taskCompletionSource)
            where T : FailureResponse
        {
            string name;

            if (ValidateParameters(commandParameters, NameParameterName) && commandParameters.TryGetValue(NameParameterName, out name))
            {
                response.Name = name;
                response.ErrorMessage = commandName;
            }
            else
            {
                response.ErrorMessage = InvalidResponseErrorMessage;
            }

            return TrySetResponse(taskCompletionSource, response);
        }

        protected override bool TryProcessCommand(string previousCommandName, string commandName, SortedDictionary<string, string> commandParameters)
        {
            const string createSessionSucceededCommandName = "create ok";
            const string createSessionFailedCommandName = "create fail";
            const string joinSessionSucceededCommandName = "join ok";
            const string joinSessionFailedCommandName = "join fail";
            const string changeCellSucceededCommandName = "change ok";
            const string changeCellVersionOutOfDateCommandName = "change wait";
            const string changeCellFailedCommandName = "change fail";
            const string undoSucceededCommandName = "undo ok";
            const string undoNoUnsavedChangesCommandName = "undo end";
            const string undoVersionOutOfDateCommandName = "undo wait";
            const string undoFailedCommandName = "undo fail";
            const string updateCommandName = "update";
            const string saveSucceededCommandName = "save ok";
            const string saveFailedCommandName = "save fail";

            const string passwordParameterName = "password";
            const string versionParameterName = "version";
            const string cellParameterName = "cell";
            const string lengthParameterName = "length";

            string name;
            string password;
            string version;
            string cell;
            string length;

            switch (commandName)
            {
                case createSessionSucceededCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, passwordParameterName))
                    {
                        var response = new CreateSessionResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myCreateSessionTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name) && commandParameters.TryGetValue(passwordParameterName, out password))
                    {
                        var response = new CreateSessionResponse
                        {
                            Name = name,
                            Password = password
                        };
                        return TrySetResponse(myCreateSessionTaskCompletionSource, response);
                    }

                    return false;

                case createSessionFailedCommandName:
                    return CommandFailedValidateParameters(commandParameters, myCreateSessionTaskCompletionSource);

                case joinSessionSucceededCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, lengthParameterName))
                    {
                        var response = new JoinSessionResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myJoinSessionTaskCompletionSource, response);
                    }
                    return false;

                case joinSessionFailedCommandName:
                    return CommandFailedValidateParameters(commandParameters, myJoinSessionTaskCompletionSource);

                case changeCellSucceededCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName))
                    {
                        var response = new ChangeCellResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myChangeCellTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name) && commandParameters.TryGetValue(versionParameterName, out version))
                    {
                        var response = new ChangeCellResponse
                        {
                            Name = name,
                            Version = version
                        };
                        return TrySetResponse(myChangeCellTaskCompletionSource, response);
                    }

                    return false;

                case changeCellVersionOutOfDateCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName))
                    {
                        var response = new ChangeCellResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myChangeCellTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name) && commandParameters.TryGetValue(versionParameterName, out version))
                    {
                        var response = new ChangeCellResponse
                        {
                            Name = name,
                            Version = version,
                            VersionOutOfDate = true
                        };
                        return TrySetResponse(myChangeCellTaskCompletionSource, response);
                    }

                    return false;

                case changeCellFailedCommandName:
                    return CommandFailedValidateParameters(commandParameters, myChangeCellTaskCompletionSource);

                case undoSucceededCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, cellParameterName, lengthParameterName))
                    {
                        var response = new UndoLastChangeResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                    }
                    return false;

                case undoNoUnsavedChangesCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName))
                    {
                        var response = new UndoLastChangeResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name) && commandParameters.TryGetValue(versionParameterName, out version))
                    {
                        var response = new UndoLastChangeResponse
                        {
                            Name = name,
                            Version = version,
                            NoUnsavedChanges = true
                        };
                        return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                    }

                    return false;

                case undoVersionOutOfDateCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName))
                    {
                        var response = new UndoLastChangeResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name) && commandParameters.TryGetValue(versionParameterName, out version))
                    {
                        var response = new UndoLastChangeResponse
                        {
                            Name = name,
                            Version = version,
                            VersionOutOfDate = true
                        };
                        return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                    }

                    return false;

                case undoFailedCommandName:
                    return CommandFailedValidateParameters(commandParameters, myUndoLastChangeTaskCompletionSource);

                case updateCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, cellParameterName, lengthParameterName))
                    {
                        return true;
                    }
                    return false;

                case saveSucceededCommandName:
                    if (!ValidateParameters(commandParameters, NameParameterName))
                    {
                        var response = new SaveResponse
                        {
                            ErrorMessage = InvalidResponseErrorMessage
                        };
                        return TrySetResponse(mySaveTaskCompletionSource, response);
                    }

                    if (commandParameters.TryGetValue(NameParameterName, out name))
                    {
                        var response = new SaveResponse
                        {
                            Name = name
                        };
                        return TrySetResponse(mySaveTaskCompletionSource, response);
                    }

                    return false;

                case saveFailedCommandName:
                    return CommandFailedValidateParameters(commandParameters, mySaveTaskCompletionSource);

                default:
                    switch (previousCommandName)
                    {
                        case createSessionFailedCommandName:
                            return CommandFailed(commandParameters, new CreateSessionResponse(), commandName, myCreateSessionTaskCompletionSource);

                        case joinSessionSucceededCommandName:
                            if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, lengthParameterName))
                            {
                                var response = new CreateSessionResponse
                                {
                                    ErrorMessage = InvalidResponseErrorMessage
                                };
                                return TrySetResponse(myCreateSessionTaskCompletionSource, response);
                            }

                            if
                            (
                                commandParameters.TryGetValue(NameParameterName, out name) &&
                                commandParameters.TryGetValue(versionParameterName, out version) &&
                                commandParameters.TryGetValue(lengthParameterName, out length)
                            )
                            {
                                JoinSessionResponse response;
                                int lengthValue;

                                if (!int.TryParse(length, out lengthValue) || commandName.Length != lengthValue)
                                {
                                    response = new JoinSessionResponse
                                    {
                                        ErrorMessage = InvalidResponseErrorMessage
                                    };
                                }
                                else
                                {
                                    response = new JoinSessionResponse
                                    {
                                        Name = name,
                                        Version = version,
                                        Xml = commandName
                                    };
                                }

                                return TrySetResponse(myJoinSessionTaskCompletionSource, response);
                            }

                            return false;

                        case joinSessionFailedCommandName:
                            return CommandFailed(commandParameters, new JoinSessionResponse(), commandName, myJoinSessionTaskCompletionSource);

                        case changeCellFailedCommandName:
                            return CommandFailed(commandParameters, new ChangeCellResponse(), commandName, myChangeCellTaskCompletionSource);

                        case undoSucceededCommandName:
                            if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, cellParameterName, lengthParameterName))
                            {
                                var response = new UndoLastChangeResponse
                                {
                                    ErrorMessage = InvalidResponseErrorMessage
                                };
                                return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                            }

                            if
                            (
                                commandParameters.TryGetValue(NameParameterName, out name) &&
                                commandParameters.TryGetValue(versionParameterName, out version) &&
                                commandParameters.TryGetValue(cellParameterName, out cell) &&
                                commandParameters.TryGetValue(lengthParameterName, out length)
                            )
                            {
                                UndoLastChangeResponse response;
                                int lengthValue;

                                if (!int.TryParse(length, out lengthValue) || commandName.Length != lengthValue)
                                {
                                    response = new UndoLastChangeResponse
                                    {
                                        ErrorMessage = InvalidResponseErrorMessage
                                    };
                                }
                                else
                                {
                                    response = new UndoLastChangeResponse
                                    {
                                        Name = name,
                                        Version = version,
                                        Cell = cell,
                                        Content = commandName
                                    };
                                }

                                return TrySetResponse(myUndoLastChangeTaskCompletionSource, response);
                            }

                            return false;

                        case undoFailedCommandName:
                            return CommandFailed(commandParameters, new UndoLastChangeResponse(), commandName, myUndoLastChangeTaskCompletionSource);

                        case updateCommandName:
                            if (!ValidateParameters(commandParameters, NameParameterName, versionParameterName, cellParameterName, lengthParameterName))
                            {
                                return true;
                            }

                            if
                            (
                                commandParameters.TryGetValue(NameParameterName, out name) &&
                                commandParameters.TryGetValue(versionParameterName, out version) &&
                                commandParameters.TryGetValue(cellParameterName, out cell) &&
                                commandParameters.TryGetValue(lengthParameterName, out length)
                            )
                            {
                                int lengthValue;

                                if (!int.TryParse(length, out lengthValue) || commandName.Length != lengthValue)
                                {
                                    return true;
                                }

                                var cellUpdated = CellUpdated;

                                if (cellUpdated == null)
                                {
                                    return true;
                                }

                                var updateEventArgs = new CellUpdatedEventArgs
                                {
                                    Name = name,
                                    Version = version,
                                    Cell = cell,
                                    Content = commandName
                                };

                                cellUpdated(this, updateEventArgs);

                                return true;
                            }

                            return false;

                        case saveFailedCommandName:
                            return CommandFailed(commandParameters, new SaveResponse(), commandName, mySaveTaskCompletionSource);

                        default:
                            return false;
                    }
            }
        }

        public async Task Connect(string host)
        {
            var portString = GetSetting("Port");
            int port;

            if (!int.TryParse(portString, out port) || port < 0)
            {
                throw new ConfigurationErrorsException("\"" + portString + "\" isn't a valid port.");
            }

            await Connect(host, port);

            return;
        }

        public async Task<CreateSessionResponse> CreateSession(string name, string password)
        {
            myCreateSessionTaskCompletionSource = new TaskCompletionSource<CreateSessionResponse>();

            var ok = await SendText("CREATE\nName:" + name + "\nPassword:" + password + "\n");
            var taskCompletionSource = myCreateSessionTaskCompletionSource;

            if (!ok || taskCompletionSource == null)
            {
                var response = new CreateSessionResponse
                {
                    ErrorMessage = UnableToSendCommandErrorMessage
                };
                return response;
            }

            return await taskCompletionSource.Task;
        }

        public async Task<JoinSessionResponse> JoinSession(string name, string password)
        {
            myJoinSessionTaskCompletionSource = new TaskCompletionSource<JoinSessionResponse>();

            var ok = await SendText("JOIN\nName:" + name + "\nPassword:" + password + "\n");
            var taskCompletionSource = myJoinSessionTaskCompletionSource;

            if (!ok || taskCompletionSource == null)
            {
                var response = new JoinSessionResponse
                {
                    ErrorMessage = UnableToSendCommandErrorMessage
                };
                return response;
            }

            return await taskCompletionSource.Task;
        }

        public Task<bool> LeaveSession(string name)
        {
            return SendText("LEAVE\nName:" + name + "\n");
        }

        public async Task<ChangeCellResponse> ChangeCell(string name, string version, string cell, string content)
        {
            myChangeCellTaskCompletionSource = new TaskCompletionSource<ChangeCellResponse>();

            var ok = await SendText("CHANGE\nName:" + name + "\nVersion:" + version + "\nCell:" + cell + "\nLength:" + content.Length.ToString(NumberFormatInfo.InvariantInfo) + "\n" + content + "\n");
            var taskCompletionSource = myChangeCellTaskCompletionSource;

            if (!ok || taskCompletionSource == null)
            {
                var response = new ChangeCellResponse
                {
                    ErrorMessage = UnableToSendCommandErrorMessage
                };
                return response;
            }

            return await taskCompletionSource.Task;
        }

        public async Task<UndoLastChangeResponse> UndoLastChange(string name, string version)
        {
            myUndoLastChangeTaskCompletionSource = new TaskCompletionSource<UndoLastChangeResponse>();

            var ok = await SendText("UNDO\nName:" + name + "\nVersion:" + version + "\n");
            var taskCompletionSource = myUndoLastChangeTaskCompletionSource;

            if (!ok || taskCompletionSource == null)
            {
                var response = new UndoLastChangeResponse
                {
                    ErrorMessage = UnableToSendCommandErrorMessage
                };
                return response;
            }

            return await taskCompletionSource.Task;
        }

        public async Task<SaveResponse> Save(string name)
        {
            mySaveTaskCompletionSource = new TaskCompletionSource<SaveResponse>();

            var ok = await SendText("SAVE\nName:" + name + "\n");
            var taskCompletionSource = mySaveTaskCompletionSource;

            if (!ok || taskCompletionSource == null)
            {
                var response = new SaveResponse
                {
                    ErrorMessage = UnableToSendCommandErrorMessage
                };
                return response;
            }

            return await taskCompletionSource.Task;
        }
    }
}