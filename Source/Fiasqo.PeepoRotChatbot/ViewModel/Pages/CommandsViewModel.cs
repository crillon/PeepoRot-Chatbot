﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Fiasqo.PeepoRotChatbot.Common.Utilities;
using Fiasqo.PeepoRotChatbot.Domain;
using Fiasqo.PeepoRotChatbot.Domain.Commands;
using Fiasqo.PeepoRotChatbot.Model.Data;
using Fiasqo.PeepoRotChatbot.Model.Data.Collections;

namespace Fiasqo.PeepoRotChatbot.ViewModel.Pages {
public class CommandsViewModel : PropertyChangedNotifier, IPageViewModel {
#region Constructor

	public CommandsViewModel(bool IsLazyLoading = true) {
		EditRowCmd = new Command(sender => {
			for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
				if (!(vis is DataGridRow row)) continue;
				if (row.Item is ChatCommand editItem) {
					NewChatCommand.Command = editItem.Command;
					NewChatCommand.Reply = editItem.Reply;
					NewChatCommand.Permission = editItem.Permission;
					NewChatCommand.IsActivated = editItem.IsActivated;
				}

				DeleteRowCmd.Execute(sender);
				break;
			}
		});

		DeleteRowCmd = new Command(sender => {
			for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
				if (!(vis is DataGridRow row)) continue;
				ChatCommands.Remove(row.Item as ChatCommand);
				break;
			}
		});

		AddNewChatCommandCmd = new Command(_ => {
			if (ReferenceEquals(NewChatCommand, null)) throw new NullReferenceException(nameof(NewChatCommand));
			if (ReferenceEquals(ChatCommands, null)) throw new NullReferenceException(nameof(ChatCommands));
			if (NewChatCommand.GetHasError()) {
				MessageBox.Show("This Chat Command Contains Errors !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (ChatCommands.Any(chatCommand => chatCommand.Command.Equals(NewChatCommand.Command) || chatCommand.Equals(NewChatCommand))) {
				MessageBox.Show("This Chat Command Has Already Been Added To The Table !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			ChatCommands.Add(NewChatCommand);
			NewChatCommand = null;
			NewChatCommand = new ChatCommand();
			//TODO: Update bot
		});

		if (IsLazyLoading) LoadDefault();
		else LoadDataOrDefault();
	}

#endregion

#region Fields

	private ChatCommand _newChatCommand;
	private ChatCommandCollection _chatCommands;
	private bool _chatCommandsIsEnable;

#endregion

#region Commands

	public Command AddNewChatCommandCmd { get; }
	public Command EditRowCmd { get; }
	public Command DeleteRowCmd { get; }
	public Command ApplyChatCommandCmd { get; } = new(_ => { });
	public Command TurnOnOffChatCommandsCmd { get; } = new(_ => { });

#endregion

#region Properties

	public ChatCommand NewChatCommand { get => _newChatCommand; private set => SetField(ref _newChatCommand, value); }
	public ChatCommandCollection ChatCommands { get => _chatCommands; private set => SetField(ref _chatCommands, value); }
	public bool ChatCommandsIsEnabled { get => _chatCommandsIsEnable; set => SetField(ref _chatCommandsIsEnable, value); }

#endregion

#region IPageViewModel

	public void LoadDefault() {
		ChatCommands = new ChatCommandCollection();
		NewChatCommand = new ChatCommand();
	}

	/// <inheritdoc />
	public bool IsLoaded { get; private set; }

	/// <inheritdoc />
	public void LoadDataOrDefault() {
		Logger.Log.Info("Loading Data");

		IsLoaded = true;
	}

	/// <inheritdoc />
	public void SaveData() => Logger.Log.Info("Saving Data");

#endregion
}
}