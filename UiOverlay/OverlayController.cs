using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Internal.Enums;
using KamiToolKit.Internal.Extensions;

namespace KamiToolKit.UiOverlay;

/// <summary>
/// Overlay controller for interacting with various overlay addons for displaying ui elements as part of the HUD.
/// </summary>
public unsafe class OverlayController : IDisposable {

    /// <summary>
    /// Adds a node to the overlay.
    /// </summary>
    /// <remarks>
    /// This must be done from the main game thread.
    /// The added node is then owned by the overlay.
    /// </remarks>
    public void AddNode(OverlayNode node) {
        ThreadSafety.AssertMainThread();

        overlayNodes.TryAdd(node.OverlayLayer, []);

        if (overlayNodes[node.OverlayLayer].Contains(node)) return;

        overlayNodes[node.OverlayLayer].Add(node);

        if (addonState[node.OverlayLayer] is not OverlayAddonState.Ready) return;

        var overlayAddon = RaptureAtkUnitManager.Instance()->GetAddonByName(node.OverlayLayer.Description);
        if (overlayAddon is not null) {
            node.AttachNode(overlayAddon);
        }

        UpdateNodeListsForOverlay(node.OverlayLayer);
    }

    /// <summary>
    /// Removes and disposes the specified node from the overlay.
    /// </summary>
    /// <remarks>
    /// This must be done from the main game thread.
    /// </remarks>
    public void RemoveNode(OverlayNode node) {
        ThreadSafety.AssertMainThread();

        if (!overlayNodes.TryGetValue(node.OverlayLayer, out var list)) return;

        if (list.Remove(node)) {
            node.Dispose();
        }

        UpdateNodeListsForOverlay(node.OverlayLayer);
    }

    /// <summary>
    /// Removes and disposes all attached nodes.
    /// </summary>
    /// <remarks>
    /// Must be done from the main game thread.
    /// </remarks>
    public void RemoveAllNodes() {
        ThreadSafety.AssertMainThread();

        foreach (var node in overlayNodes.SelectMany(set => set.Value).ToList()) {
            RemoveNode(node);
        }
    }

    /// <remarks>
    /// Must be constructed from the main game thread
    /// </remarks>
    public OverlayController() {
        ThreadSafety.AssertMainThread();

        ClearState();

        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlatePreFinalize);

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addonName = overlayLayer.Description;

            Services.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, addonName, OnOverlayAddonUpdate);
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, addonName, OnOverlayAddonFinalize);
        }

        BeginStateCheck();
    }

    /// <inheritdoc />
    public void Dispose() {
        ThreadSafety.AssertMainThread();

        Services.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        Services.AddonLifecycle.UnregisterListener(OnOverlayAddonFinalize, OnOverlayAddonUpdate);

        foreach (var (overlayLayer, nodes) in overlayNodes) {
            Services.Log.Debug($"Disposing overlay nodes for layer {overlayLayer}");
            foreach (var node in nodes) {
                node.Dispose();
            }

            UpdateNodeListsForOverlay(overlayLayer);
        }

        overlayNodes.Clear();
    }

    //
    // State management (framework thread)
    //

    private void ClearState() {
        controllerState = ControllerState.WaitForNameplate;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            addonState[overlayLayer] = OverlayAddonState.None;
        }
    }

    private void BeginStateCheck() {
        Services.Framework.Update -= CheckOverlayState;
        Services.Framework.Update += CheckOverlayState;
    }

    private void CheckOverlayState(IFramework framework) {
        switch (controllerState) {
            case ControllerState.WaitForNameplate:
                CheckNameplateReady();
                break;

            case ControllerState.WaitForReady:
                CheckOverlayAddonsReady();
                break;

            case ControllerState.Ready:
                Services.Framework.Update -= CheckOverlayState;
                break;
        }
    }

    private void CheckNameplateReady() {
        var nameplate = RaptureAtkUnitManager.Instance()->GetAddonByName("NamePlate");
        if (nameplate is null) return;
        if (!nameplate->IsReady) return;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(overlayLayer.Description);

            if (addon is null) {
                if (addonState[overlayLayer] == OverlayAddonState.None) {
                    addonState[overlayLayer] = OverlayAddonState.WaitForReady;
                    CreateOverlayAddon(overlayLayer).Open();
                }
            }
            else {
                addonState[overlayLayer] = OverlayAddonState.WaitForReady;
            }
        }

        controllerState = ControllerState.WaitForReady;
    }

    private void CheckOverlayAddonsReady() {
        var totalAddons = Enum.GetValues<OverlayLayer>().Length;
        var totalAddonsReady = 0;

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(overlayLayer.Description);
            if (addon is null) continue;
            if (!addon->IsReady) continue;

            if (addonState[overlayLayer] is OverlayAddonState.WaitForReady) {
                AttachAllNodes(overlayLayer);
                addonState[overlayLayer] = OverlayAddonState.Ready;
            }
            totalAddonsReady++;
        }

        if (totalAddonsReady == totalAddons) {
            controllerState = ControllerState.Ready;
        }
    }

    private void AttachAllNodes(OverlayLayer layer) {
        if (!overlayNodes.TryGetValue(layer, out var list)) return;

        var overlayAddon = RaptureAtkUnitManager.Instance()->GetAddonByName(layer.Description);
        if (overlayAddon is not null) {
            foreach (var node in list) {
                node.AttachNode(overlayAddon);
            }
        }

        UpdateNodeListsForOverlay(layer);
    }

    //
    // Events
    //

    private void OnNamePlatePreFinalize(AddonEvent type, AddonArgs args) {
        ClearState();

        foreach (var overlayLayer in Enum.GetValues<OverlayLayer>()) {
            if (!overlayNodes.TryGetValue(overlayLayer, out var list)) continue;

            foreach (var node in list) {
                node.DetachNode();
            }

            UpdateNodeListsForOverlay(overlayLayer);
        }

        BeginStateCheck();
    }

    private void OnOverlayAddonFinalize(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        if (overlayNodes.TryGetValue(overlayLayer, out var list)) {
            foreach (var node in list) {
                node.DetachNode();
            }
        }

        UpdateNodeListsForOverlay(overlayLayer);

        addonState[overlayLayer] = OverlayAddonState.None;
    }

    private void OnOverlayAddonUpdate(AddonEvent type, AddonArgs args) {
        var addon = (AtkUnitBase*)args.Addon.Address;
        var overlayLayer = addon->DepthLayer.GetOverlayLayer();

        if (addonState[overlayLayer] is not OverlayAddonState.Ready) return;
        if (!overlayNodes.TryGetValue(overlayLayer, out var list)) return;

        foreach (var node in list) {
            node.Update();
        }
    }

    //
    // Helpers
    //

    private static NativeAddon CreateOverlayAddon(OverlayLayer layer) => new() {
        Title = layer.Description,
        InternalName = layer.Description,
        DepthLayer = layer.DepthLayer,
        IsOverlayAddon = true,
    };

    private void UpdateNodeListsForOverlay(OverlayLayer layer) {
        var overlayAddon = RaptureAtkUnitManager.Instance()->GetAddonByName(layer.Description);
        if (overlayAddon is not null) {
            overlayAddon->UldManager.UpdateDrawNodeList();
            overlayAddon->UpdateCollisionNodeList(false);
        }
    }

    private readonly Dictionary<OverlayLayer, List<OverlayNode>> overlayNodes = [];
    private readonly Dictionary<OverlayLayer, OverlayAddonState> addonState = [];

    private ControllerState controllerState = ControllerState.WaitForNameplate;
}
