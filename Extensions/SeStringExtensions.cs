using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;

namespace KamiToolKit.Extensions;

public static class SeStringExtensions {
    public static byte[] EncodeWithNull(this SeString str) {
        var messageBytes = new List<byte>();
        foreach (var payload in str.Payloads) {
            messageBytes.AddRange(payload.Encode());
        }
        
        // Add Null Terminator
        messageBytes.Add(0);

        return messageBytes.ToArray();
    }
}