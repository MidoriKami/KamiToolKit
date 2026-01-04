using System;
using System.ComponentModel;
using Dalamud.Utility;

namespace KamiToolKit.Extensions;

public static class EnumExtensions {
    extension(Enum enumValue) {
        public string Description => enumValue.GetDescription();

        private string GetDescription() {
            var attribute = enumValue.GetAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }
    }
}
