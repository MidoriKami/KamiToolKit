using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Utility;

namespace KamiToolKit.Extensions;

internal static class EnumExtensions {
    extension(Enum enumValue) {
        public string Description => enumValue.GetDescription();

        private string GetDescription() {
            var attribute = enumValue.GetAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }
    }
    
    extension<T>(ref T flagValue) where T : unmanaged, Enum {
        public void SetFlags(params T[] flags) {
            foreach (var flag in flags) {
                flagValue.SetFlag(flag, true);
            }
        }

        public void ClearFlags(params T[] flags) {
            foreach (var flag in flags) {
                flagValue.SetFlag(flag, false);
            }
        }

        private unsafe void SetFlag(T flag, bool enable) {
            switch (sizeof(T)) {
                case 1: flagValue.SetFlag<T, byte>(flag, enable); break;
                case 2: flagValue.SetFlag<T, ushort>(flag, enable); break;
                case 4: flagValue.SetFlag<T, uint>(flag, enable); break;
                case 8: flagValue.SetFlag<T, ulong>(flag, enable); break;
                default: throw new NotSupportedException("Unsupported enum size");
            }
        }

        private void SetFlag<TUnderlying>(T flag, bool enable) where TUnderlying : unmanaged, IBinaryInteger<TUnderlying> {
            ref var value = ref Unsafe.As<T, TUnderlying>(ref flagValue);
            var mask = Unsafe.As<T, TUnderlying>(ref flag);

            if (enable)
                value |= mask;
            else
                value &= ~mask;
        }
    }
}
