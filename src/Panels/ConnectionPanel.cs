﻿using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using UnityEngine;

namespace CSM.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _clientConnectButton;
        private UIButton _serverConnectButton;

        // These buttons are displayed when the server is running
        private UIButton _disconnectButton;

        private UIButton _serverManageButton;
        private UIButton _playerListButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPConnectionPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 100.0f);

            width = 360;
            height = 200;

            // Handle visible change events
            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    if (MultiplayerManager.Instance.CurrentServer.Status == ServerStatus.Running)
                    {
                        Hide(_clientConnectButton);
                        Hide(_serverConnectButton);
                        Show(_disconnectButton);
                        Show(_serverManageButton);
                        Hide(_playerListButton);

                        _disconnectButton.text = "Stop server";
                    }
                    else
                    {
                        Show(_clientConnectButton);
                        Show(_serverConnectButton);
                        Hide(_disconnectButton);
                        Hide(_serverManageButton);
                        Hide(_playerListButton);
                    }
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    Hide(_clientConnectButton);
                    Hide(_serverConnectButton);
                    Show(_disconnectButton);
                    Hide(_serverManageButton);
                    Show(_playerListButton);

                    _disconnectButton.text = "Disconnect";
                }
                else
                {
                    Show(_clientConnectButton);
                    Show(_serverConnectButton);
                    Hide(_disconnectButton);
                    Hide(_serverManageButton);
                    Hide(_playerListButton);
                }
            };

            this.CreateTitleLabel("Multiplayer Menu", new Vector3(80, -20, 0));

            // Join game button
            _clientConnectButton = this.CreateButton("Join Game", new Vector2(10, -60));

            // Manage server button
            _serverManageButton = this.CreateButton("Manage Server", new Vector2(10, -60));
            _serverManageButton.isEnabled = false;
            _serverManageButton.isVisible = false;

            // Host game button
            _serverConnectButton = this.CreateButton("Host Game", new Vector2(10, -130));

            // Close server button
            _disconnectButton = this.CreateButton("Stop Server", new Vector2(10, -130));
            _disconnectButton.isEnabled = false;
            _disconnectButton.isVisible = false;

            _playerListButton = this.CreateButton("Player list", new Vector2(10, -60));
            _playerListButton.isEnabled = false;
            _playerListButton.isVisible = false;

            _clientConnectButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<JoinGamePanel>("MPJoinGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    var joinGamePanel = (JoinGamePanel)view.AddUIComponent(typeof(JoinGamePanel));
                    joinGamePanel.Focus();
                }

                isVisible = false;
            };

            // Host a game panel
            _serverConnectButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<HostGamePanel>("MPHostGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    var hostGamePanel = (HostGamePanel)view.AddUIComponent(typeof(HostGamePanel));
                    hostGamePanel.Focus();
                }

                isVisible = false;
            };

            _disconnectButton.eventClick += (component, param) =>
            {
                isVisible = false;

                MultiplayerManager.Instance.StopEverything(false);
            };

            _serverManageButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<ManageGamePanel>("MPManageGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                }
                else
                {
                    panel = (ManageGamePanel)view.AddUIComponent(typeof(ManageGamePanel));
                }

                panel.Focus();

                isVisible = false;
            };

            _playerListButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<PlayerListPanel>("MPPlayerListPanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                }
                else
                {
                    panel = (PlayerListPanel)view.AddUIComponent(typeof(PlayerListPanel));
                }

                panel.Focus();

                isVisible = false;
            };

            base.Start();
        }

        private void Show(UIButton button)
        {
            button.isVisible = true;
            button.isEnabled = true;
        }

        private void Hide(UIButton button)
        {
            button.isVisible = false;
            button.isEnabled = false;
        }
    }
}