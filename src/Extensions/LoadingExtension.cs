﻿using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CSM.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private UIButton _multiplayerButton;

        public override void OnReleased()
        {
            // Stop everything
            MultiplayerManager.Instance.StopEverything();

            base.OnReleased();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                Command.SendToServer(new ClientLevelLoadedCommand());

                // Hide join panel (from resyncing)
                ClientJoinPanel clientJoinPanel = UIView.GetAView().FindUIComponent<ClientJoinPanel>("MPClientJoinPanel");
                if (clientJoinPanel != null)
                {
                    clientJoinPanel.isVisible = false;
                }
                MultiplayerManager.Instance.GameBlocked = false;
            }

            UIView uiView = UIView.GetAView();

            // Add the chat log
            uiView.AddUIComponent(typeof(ChatLogPanel));

            _multiplayerButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));

            _multiplayerButton.text = "Multiplayer";
            _multiplayerButton.width = 150;
            _multiplayerButton.height = 40;

            _multiplayerButton.normalBgSprite = "ButtonMenu";
            _multiplayerButton.disabledBgSprite = "ButtonMenuDisabled";
            _multiplayerButton.hoveredBgSprite = "ButtonMenuHovered";
            _multiplayerButton.focusedBgSprite = "ButtonMenuFocused";
            _multiplayerButton.pressedBgSprite = "ButtonMenuPressed";
            _multiplayerButton.textColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.disabledTextColor = new Color32(7, 7, 7, 255);
            _multiplayerButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            _multiplayerButton.focusedTextColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            _multiplayerButton.playAudioEvents = true;

            // Place the button.
            _multiplayerButton.transformPosition = new Vector3(-1.45f, 0.97f);

            // Respond to button click.
            _multiplayerButton.eventClick += (component, param) =>
            {
                ConnectionPanel panel = uiView.FindUIComponent<ConnectionPanel>("MPConnectionPanel");

                if (panel != null)
                {
                    panel.isVisible = !panel.isVisible;
                }
                else
                {
                    uiView.AddUIComponent(typeof(ConnectionPanel));
                }

                _multiplayerButton.Unfocus();
            };
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            //Code below destroys any created UI from the screen.
            try
            {
                UIComponent _getui = UIView.GetAView().FindUIComponent<UIComponent>("ChatLogPanel");
                UIComponent[] children = _getui.GetComponentsInChildren<UIComponent>();

                foreach (UIComponent child in children)
                {
                    Object.Destroy(child);
                }

                // Destroy duplicated multiplayer button
                UIComponent temp = UIView.GetAView().FindUIComponent("MPConnectionPanel");
                Object.Destroy(temp);
            }
            catch (NullReferenceException)
            {
                // Ignore, because it sometimes throws them... (Not caused by us)
                // TODO: Rework this to be more stable
            }
        }
    }
}
