using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Dalamud;

namespace KamiToolKit;

public unsafe partial class NativeAddon {
    protected virtual void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) { }
    protected virtual void OnShow(AtkUnitBase* addon) { }
    protected virtual void OnDraw(AtkUnitBase* addon) { }
    protected virtual void OnUpdate(AtkUnitBase* addon) { }
    protected virtual void OnHide(AtkUnitBase* addon) { }
    protected virtual void OnFinalize(AtkUnitBase* addon) { }
    protected virtual void OnRequestedUpdate(AtkUnitBase* addon, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) { }
    protected virtual void OnRefresh(AtkUnitBase* addon, Span<AtkValue> atkValues) { }

    private bool isSetup;

    private void Initialize(AtkUnitBase* thisPtr) {
        Services.Log.Verbose($"[{InternalName}] Initialize");

        AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

        thisPtr->UldManager.InitializeResourceRendererManager();

        InitializeAddon();
    }

    private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
        Services.Log.Verbose($"[{InternalName}] Setup");

        if (!IsOverlayAddon) {
            SetInitialState();
        }
        else {
            ref var screenSize = ref AtkStage.Instance()->ScreenSize;

            addon->SetScale(1.0f / AtkUnitBase.GetGlobalUIScale(), true);
            addon->SetSize((ushort)screenSize.Width, (ushort)screenSize.Height);
            addon->SetPosition(0, 0);
        }

        AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);

        OnSetup(addon, new Span<AtkValue>(values, (int)valueCount));
        isSetup = true;
    }

    private void Show(AtkUnitBase* addon, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
        Services.Log.Verbose($"[{InternalName}] Show");

        OnShow(addon);

        AtkUnitBase.StaticVirtualTablePointer->Show(addon, silenceOpenSoundEffect, unsetShowHideFlags);
    }

    private void Update(AtkUnitBase* addon, float delta) {
        Services.Log.Excessive($"[{InternalName}] Update");

        OnUpdate(addon);

        AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
    }

    private void Draw(AtkUnitBase* addon) {
        Services.Log.Excessive($"[{InternalName}] Draw");

        OnDraw(addon);

        AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
    }

    private void Hide(AtkUnitBase* addon, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
        Services.Log.Verbose($"[{InternalName}] Hide");

        OnHide(addon);
        SaveAddonConfig();

        AtkUnitBase.StaticVirtualTablePointer->Hide(addon, unkBool, callHideCallback, setShowHideFlags);
        AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
    }

    private void Hide2(AtkUnitBase* addon) {
        Services.Log.Verbose($"[{InternalName}] Hide2");

        AtkUnitBase.StaticVirtualTablePointer->Hide2(addon);
    }

    private void Finalizer(AtkUnitBase* addon) {
        Services.Log.Verbose($"[{InternalName}] Finalize");

        OnFinalize(addon);

        if (RememberClosePosition) {
            LastClosePosition = new Vector2(InternalAddon->X, InternalAddon->Y);
        }

        AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
        isSetup = false;
    }

    private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
        Services.Log.Verbose($"[{InternalName}] Destructor");

        var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

        if ((flags & 1) == 1) {
            InternalAddon = null;
            disposeHandle?.Free();
            disposeHandle = null;
            CreatedAddons.Remove(this);

            // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
            NativeMemoryHelper.Free(virtualTable, 0x8 * VirtualTableEntryCount);
        }

        return result;
    }

    private void RequestedUpdate(AtkUnitBase* thisPtr, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) {
        Services.Log.Verbose($"[{InternalName}] RequestedUpdate");

        // Prevent calls to OnRequestedUpdate before Setup is completed. The game will try to call this after Show but before Setup
        if (isSetup) {
            OnRequestedUpdate(thisPtr, numberArrayData, stringArrayData);
        }

        AtkUnitBase.StaticVirtualTablePointer->OnRequestedUpdate(InternalAddon, numberArrayData, stringArrayData);
    }

    private bool Refresh(AtkUnitBase* thisPtr, uint valueCount, AtkValue* values) {
        Services.Log.Verbose($"[{InternalName}] Refresh");

        OnRefresh(thisPtr, new Span<AtkValue>(values, (int)valueCount));

        return AtkUnitBase.StaticVirtualTablePointer->OnRefresh(InternalAddon, valueCount, values);
    }

    private void ScreenSizeChange(AtkUnitBase* thisPtr, int width, int height) {
        Services.Log.Verbose($"[{InternalName}] ScreenSizeChange");

        AtkUnitBase.StaticVirtualTablePointer->OnScreenSizeChange(thisPtr, width, height);

        if (IsOverlayAddon || IgnoreGlobalScale) {
            thisPtr->SetScale(1.0f / AtkUnitBase.GetGlobalUIScale(), true);
        }
    }
}
