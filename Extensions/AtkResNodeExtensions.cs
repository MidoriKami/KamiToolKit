using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Extensions;

public static unsafe class AtkResNodeExtensions {
    extension(ref AtkResNode node) {
        public Vector2 Position {
            get => new(node.X, node.Y);
            set => node.SetPositionFloat(value.X, value.Y);
        }

        public Vector2 ScreenPosition 
            => new(node.ScreenX, node.ScreenY);

        public Vector2 Size {
            get => new(node.GetWidth(), node.GetHeight());
            set {
                node.SetWidth((ushort) value.X);
                node.SetHeight((ushort) value.Y);
            }
        }

        public Bounds Bounds => new() {
            TopLeft = node.Position,
            BottomRight = node.Position + node.Size,
        };
        
        public Vector2 Center 
            => node.Position + node.Size / 2.0f;
        
        public Vector2 Scale {
            get => new (node.GetScaleX(), node.GetScaleY());
            set => node.SetScale(value.X, value.Y);
        }

        public float RotationDegrees {
            get => node.GetRotationDegrees();
            set => node.SetRotationDegrees(value - (int)(value / 360.0f) * 360.0f);
        }

        public Vector2 Origin {
            get => new(node.OriginX, node.OriginY);
            set => node.SetOrigin(value.X, value.Y);
        }

        public bool Visible {
            get => node.IsVisible();
            set => node.ToggleVisibility(value);
        }

        public Vector4 ColorVector {
            get => node.Color.ToVector4();
            set => node.Color = value.ToByteColor();
        }

        public ColorHelpers.HsvaColor ColorHsva {
            get => ColorHelpers.RgbaToHsv(node.ColorVector);
            set => node.Color = ColorHelpers.HsvToRgb(value).ToByteColor();
        }

        public Vector3 AddColor {
            get => new Vector3(node.AddRed, node.AddGreen, node.AddBlue) / 255.0f;
            set {
                node.AddRed = (short)(value.X * 255);
                node.AddGreen = (short)(value.Y * 255);
                node.AddBlue = (short)(value.Z * 255);
            }
        }

        public ColorHelpers.HsvaColor AddColorHsva {
            get => ColorHelpers.RgbaToHsv(node.AddColor.AsVector4());
            set => node.AddColor = ColorHelpers.HsvToRgb(value).AsVector3();
        }

        public Vector3 MultiplyColor {
            get => new Vector3(node.MultiplyRed, node.MultiplyGreen, node.MultiplyBlue) / 100.0f;
            set {
                node.MultiplyRed = (byte)(value.X * 100.0f);
                node.MultiplyGreen = (byte)(value.Y * 100.0f);
                node.MultiplyBlue = (byte)(value.Z * 100.0f);
            }
        }

        public ColorHelpers.HsvaColor MultiplyColorHsva {
            get => ColorHelpers.RgbaToHsv(node.MultiplyColor.AsVector4());
            set => node.MultiplyColor = ColorHelpers.HsvToRgb(value).AsVector3();
        }

        public void AddFlags(params NodeFlags[] flags) {
            foreach (var flag in flags) {
                node.NodeFlags |= flag;
            }
        }

        public void RemoveFlags(params NodeFlags[] flags) {
            foreach (var flag in flags) {
                node.NodeFlags &= ~flag;
            }
        }

        public void AddDrawFlag(params DrawFlags[] flags) {
            foreach (var flag in flags) {
                node.DrawFlags |= (uint)flag;
            }
        }

        public void RemoveDrawFlag(params DrawFlags[] flags) {
            foreach (var flag in flags) {
                node.DrawFlags &= (uint)flag;
            }
        }

        public bool CheckCollision(short x, short y, bool inclusive = true)
            => node.CheckCollisionAtCoords(x, y, inclusive);

        public bool CheckCollision(Vector2 position, bool inclusive = true)
            => node.CheckCollisionAtCoords((short) position.X, (short) position.Y, inclusive);

        public bool CheckCollision(AtkEventData* eventData, bool inclusive = true)
            => node.CheckCollisionAtCoords(eventData->MouseData.PosX, eventData->MouseData.PosY, inclusive);

        public bool IsActuallyVisible {
            get {
                if (!node.Visible) return false;

                var targetNode = node.ParentNode;
                while (targetNode is not null) {
                    if (!targetNode->Visible) return false;
                    targetNode = targetNode->ParentNode;
                }

                return true;
            }
        }
    }
}
