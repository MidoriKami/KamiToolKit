﻿using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Addon;

public abstract unsafe partial class NativeAddon {

    protected virtual void OnSetup(AtkUnitBase* addon) { }
    protected virtual void OnShow(AtkUnitBase* addon) { }
    protected virtual void OnDraw(AtkUnitBase* addon) { }
    protected virtual void OnUpdate(AtkUnitBase* addon) { }
    protected virtual void OnHide(AtkUnitBase* addon) { }
    protected virtual void OnFinalize(AtkUnitBase* addon) { }
    protected virtual void OnRequestedUpdate(AtkUnitBase* addon, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) { }
    protected virtual void OnRefresh(AtkUnitBase* addon, Span<AtkValue> atkValues) { }

    private bool isSetup;

    private void Initialize(AtkUnitBase* thisPtr) {
        Log.Verbose($"[{InternalName}] 初始化回调");

        AtkUnitBase.StaticVirtualTablePointer->Initialize(thisPtr);

        thisPtr->UldManager.InitializeResourceRendererManager();

        InitializeAddon();
    }

    private void Setup(AtkUnitBase* addon, uint valueCount, AtkValue* values) {
        Log.Verbose($"[{InternalName}] 进入 Setup 阶段");

        SetInitialState();

        AtkUnitBase.StaticVirtualTablePointer->OnSetup(addon, valueCount, values);

        OnSetup(addon);
        isSetup = true;
    }

    private void Show(AtkUnitBase* addon, bool silenceOpenSoundEffect, uint unsetShowHideFlags) {
        Log.Verbose($"[{InternalName}] 执行显示");

        OnShow(addon);

        AtkUnitBase.StaticVirtualTablePointer->Show(addon, silenceOpenSoundEffect, unsetShowHideFlags);
    }

    private void Update(AtkUnitBase* addon, float delta) {
        Log.Excessive($"[{InternalName}] 执行 Update");

        OnUpdate(addon);

        AtkUnitBase.StaticVirtualTablePointer->Update(addon, delta);
    }

    private void Draw(AtkUnitBase* addon) {
        Log.Excessive($"[{InternalName}] 执行 Draw");

        OnDraw(addon);

        AtkUnitBase.StaticVirtualTablePointer->Draw(addon);
    }

    private void Hide(AtkUnitBase* addon, bool unkBool, bool callHideCallback, uint setShowHideFlags) {
        Log.Verbose($"[{InternalName}] 执行 Hide");

        OnHide(addon);
        SaveAddonConfig();

        AtkUnitBase.StaticVirtualTablePointer->Hide(addon, unkBool, callHideCallback, setShowHideFlags);
        AtkUnitBase.StaticVirtualTablePointer->Close(addon, false);
    }

    private void Hide2(AtkUnitBase* addon) {
        Log.Verbose($"[{InternalName}] 执行 Hide2");

        AtkUnitBase.StaticVirtualTablePointer->Hide2(addon);
    }

    private void Finalizer(AtkUnitBase* addon) {
        Log.Verbose($"[{InternalName}] 执行 Finalize");

        OnFinalize(addon);

        if (RememberClosePosition) {
            Position = new Vector2(InternalAddon->X, InternalAddon->Y);
        }

        AtkUnitBase.StaticVirtualTablePointer->Finalizer(InternalAddon);
        isSetup = false;
    }

    private AtkEventListener* Destructor(AtkUnitBase* addon, byte flags) {
        Log.Verbose($"[{InternalName}] 执行析构");

        var result = AtkUnitBase.StaticVirtualTablePointer->Dtor(addon, flags);

        if ((flags & 1) == 1) {
            InternalAddon = null;
            disposeHandle?.Free();
            disposeHandle = null;
            CreatedAddons.Remove(this);

            // Free our custom virtual table, the game doesn't know this exists and won't clear it on its own.
            NativeMemoryHelper.Free(virtualTable, 0x8 * 100);
        }

        return result;
    }

    private void RequestedUpdate(AtkUnitBase* thisPtr, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) {
        Log.Verbose($"[{InternalName}] 触发 RequestedUpdate");

        // Prevent calls to OnRequestedUpdate before Setup is completed. The game will try to call this after Show but before Setup
        if (isSetup) {
            OnRequestedUpdate(thisPtr, numberArrayData, stringArrayData);
        }
        
        AtkUnitBase.StaticVirtualTablePointer->OnRequestedUpdate(InternalAddon, numberArrayData, stringArrayData);
    }

    private bool Refresh(AtkUnitBase* thisPtr, uint valueCount, AtkValue* values) {
        Log.Verbose($"[{InternalName}] 执行 Refresh");
        
        OnRefresh(thisPtr, new Span<AtkValue>(values, (int)valueCount));
        
        return AtkUnitBase.StaticVirtualTablePointer->OnRefresh(InternalAddon, valueCount, values);
    }
}
